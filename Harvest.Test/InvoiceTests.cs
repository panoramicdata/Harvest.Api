namespace Harvest.Test;

public class InvoiceTests(ITestOutputHelper testOutputHelper) : HarvestTest(testOutputHelper)
{
	[Fact]
	public async System.Threading.Tasks.Task ListInvoices()
	{
		var container = await HarvestClient.Invoices.ListAsync(new() { Page = 1, PerPage = 10 }, CancellationToken);

		container.Should().NotBeNull();
		container.Invoices.Should().NotBeNull();
	}

	[Fact]
	public async System.Threading.Tasks.Task ListInvoiceItemCategories()
	{
		var container = await HarvestClient.InvoiceItemCategories.ListAsync(new() { Page = 1, PerPage = 10 }, CancellationToken);

		container.InvoiceItemCategories.Should().NotBeEmpty();
		container.InvoiceItemCategories.Should().OnlyContain(category => category.Id > 0 && !string.IsNullOrEmpty(category.Name));
	}

	[Fact]
	public System.Threading.Tasks.Task CreateReadUpdateDeleteInvoiceItemCategory()
		=> RunStrictScenarioAsync(async () =>
		{
			var name = TestName("CRUD", "Item Category");
			var created = await HarvestClient.InvoiceItemCategories.CreateAsync(new InvoiceItemCategoryCreationDto { Name = name }, CancellationToken);
			Track("invoice item category", created.Id, created.Name, id => HarvestClient.InvoiceItemCategories.DeleteAsync(id, CancellationToken));
			created.Name.Should().Be(name);

			var retrieved = await HarvestClient.InvoiceItemCategories.GetAsync(created.Id, CancellationToken);
			retrieved.Id.Should().Be(created.Id);

			var updatedName = name + " Updated";
			var updated = await HarvestClient.InvoiceItemCategories.UpdateAsync(created.Id, new InvoiceItemCategoryPatchDto { Name = updatedName }, CancellationToken);
			updated.Name.Should().Be(updatedName);

			await DeleteTrackedAsync("invoice item category", created.Id);
		});

	[Fact]
	public System.Threading.Tasks.Task CreateReadUpdateDeleteInvoice()
		=> RunStrictScenarioAsync(async () =>
		{
			var invoice = await CreateTestInvoiceAsync("CRUD");
			invoice.Amount.Should().Be(200m);
			invoice.LineItems.Should().ContainSingle();
			invoice.LineItems![0].Quantity.Should().Be(2m);
			invoice.LineItems[0].UnitPrice.Should().Be(100m);
			invoice.State.Should().Be("draft");

			var retrieved = await HarvestClient.Invoices.GetAsync(invoice.Id, CancellationToken);
			retrieved.Id.Should().Be(invoice.Id);
			retrieved.Client!.Id.Should().Be(invoice.Client!.Id);

			var listed = await HarvestClient.Invoices.ListAsync(new() { ClientId = invoice.Client.Id }, CancellationToken);
			listed.Invoices.Should().Contain(i => i.Id == invoice.Id);

			var updated = await HarvestClient.Invoices.UpdateAsync(invoice.Id, new InvoicePatchDto
			{
				Notes = $"{Prefix}Updated invoice notes",
				Discount = 10m
			}, CancellationToken);
			updated.Notes.Should().Be($"{Prefix}Updated invoice notes");
			updated.Discount.Should().Be(10m);
			updated.Amount.Should().Be(180m);

			await DeleteTrackedAsync("invoice", invoice.Id);
		});

	[Fact]
	public System.Threading.Tasks.Task InvoiceMessagesWithoutSending()
		=> RunStrictScenarioAsync(async () =>
		{
			var invoice = await CreateTestInvoiceAsync("Messages");

			var messages = await HarvestClient.InvoiceMessages.ListAsync(invoice.Id, new() { Page = 1, PerPage = 10 }, CancellationToken);
			messages.InvoiceMessages.Should().BeEmpty();

			var template = await HarvestClient.InvoiceMessages.GetTemplateAsync(invoice.Id, options: null, CancellationToken);
			template.InvoiceId.Should().Be(invoice.Id);
			template.Subject.Should().NotBeNullOrEmpty();
			template.Body.Should().NotBeNullOrEmpty();

			var reminder = await HarvestClient.InvoiceMessages.GetTemplateAsync(invoice.Id, new() { Reminder = true }, CancellationToken);
			reminder.Subject.Should().NotBe(template.Subject);

			var thankYou = await HarvestClient.InvoiceMessages.GetTemplateAsync(invoice.Id, new() { ThankYou = true }, CancellationToken);
			thankYou.Subject.Should().NotBe(template.Subject);

			// Events that change state but mail no one: send (marks as sent, no recipients), close, re-open, draft.
			var sent = await RaiseEventAsync(invoice.Id, InvoiceMessageEventType.Send);
			sent.EventType.Should().Be("send");
			sent.Recipients.Should().BeNullOrEmpty();
			(await HarvestClient.Invoices.GetAsync(invoice.Id, CancellationToken)).State.Should().Be("open");

			_ = await RaiseEventAsync(invoice.Id, InvoiceMessageEventType.Close);
			(await HarvestClient.Invoices.GetAsync(invoice.Id, CancellationToken)).State.Should().Be("closed");

			_ = await RaiseEventAsync(invoice.Id, InvoiceMessageEventType.ReOpen);
			(await HarvestClient.Invoices.GetAsync(invoice.Id, CancellationToken)).State.Should().Be("open");

			var drafted = await RaiseEventAsync(invoice.Id, InvoiceMessageEventType.Draft);
			(await HarvestClient.Invoices.GetAsync(invoice.Id, CancellationToken)).State.Should().Be("draft");

			var all = await HarvestClient.InvoiceMessages.ListAsync(invoice.Id, options: null, CancellationToken);
			all.InvoiceMessages.Should().HaveCountGreaterThanOrEqualTo(4);

			await DeleteTrackedAsync("invoice message", drafted.Id);
		});

	[Fact]
	public System.Threading.Tasks.Task InvoicePayments()
		=> RunStrictScenarioAsync(async () =>
		{
			var invoice = await CreateTestInvoiceAsync("Payments");

			// A payment needs an open invoice; "send" marks a draft as sent without mailing anyone.
			_ = await RaiseEventAsync(invoice.Id, InvoiceMessageEventType.Send);

			var payment = await HarvestClient.InvoicePayments.CreateAsync(invoice.Id, new InvoicePaymentCreationDto
			{
				Amount = 50m,
				PaidDate = DateOnly.FromDateTime(DateTime.UtcNow),
				Notes = $"{Prefix}Payment",
				SendThankYou = false
			}, CancellationToken);
			Track("invoice payment", payment.Id, payment.Notes, id => HarvestClient.InvoicePayments.DeleteAsync(invoice.Id, id, CancellationToken));
			payment.Amount.Should().Be(50m);
			payment.Notes.Should().Be($"{Prefix}Payment");

			var payments = await HarvestClient.InvoicePayments.ListAsync(invoice.Id, new() { Page = 1, PerPage = 10 }, CancellationToken);
			payments.InvoicePayments.Should().Contain(p => p.Id == payment.Id);

			await DeleteTrackedAsync("invoice payment", payment.Id);
		});

	/// <summary>Raises an event on an invoice (no recipients, so no mail) and tracks the resulting message.</summary>
	private async Task<InvoiceMessage> RaiseEventAsync(long invoiceId, string eventType)
	{
		var message = await HarvestClient.InvoiceMessages.CreateAsync(invoiceId, new InvoiceMessageCreationDto { EventType = eventType }, CancellationToken);
		Track("invoice message", message.Id, message.EventType, id => HarvestClient.InvoiceMessages.DeleteAsync(invoiceId, id, CancellationToken));
		return message;
	}

	/// <summary>Creates a prefixed client and a draft invoice for it (one line item: 2 x 100), tracking both.</summary>
	private async Task<Invoice> CreateTestInvoiceAsync(string purpose)
	{
		var client = await CreateTestClientAsync(purpose, address: null);
		var invoice = await HarvestClient.Invoices.CreateAsync(new InvoiceCreationDto
		{
			ClientId = client.Id,
			Subject = $"{Prefix}{purpose} Test Invoice",
			Currency = "USD",
			IssueDate = DateOnly.FromDateTime(DateTime.UtcNow),
			LineItems =
			[
				new InvoiceLineItemCreationDto { Kind = "Service", Description = $"{Prefix}line", Quantity = 2m, UnitPrice = 100m }
			]
		}, CancellationToken);
		Track("invoice", invoice.Id, invoice.Subject, id => HarvestClient.Invoices.DeleteAsync(id, CancellationToken));
		return invoice;
	}
}