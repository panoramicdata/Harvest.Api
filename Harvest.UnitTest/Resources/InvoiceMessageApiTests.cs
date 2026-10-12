using Harvest.UnitTest.Support;

namespace Harvest.UnitTest.Resources;

public class InvoiceMessageApiTests
{
	// "Create and send an invoice message" example from the Harvest docs (the docs' e-mail addresses are obfuscated, so
	// example.com ones stand in).
	private const string MessageJson = """
		{"id":27835324,"sent_by":"Bob Powell","sent_by_email":"bob@example.com","sent_from":"Bob Powell",
		"sent_from_email":"bob@example.com","include_link_to_client_invoice":false,"send_me_a_copy":true,"thank_you":false,
		"reminder":false,"send_reminder_on":null,"created_at":"2017-08-23T22:25:59Z","updated_at":"2017-08-23T22:26:10Z",
		"attach_pdf":true,"event_type":null,"recipients":[{"name":"Richard Roe","email":"richardroe@example.com"},
		{"name":"Bob Powell","email":"bob@example.com"}],"subject":"Invoice #1001","body":"The invoice is attached below."}
		""";

	// "Mark a draft invoice as sent" example, with the reminder fields set so that they are read with values.
	private const string EventMessageJson = """
		{"id":27835325,"sent_by":"Bob Powell","sent_by_email":"bob@example.com","sent_from":"Bob Powell",
		"sent_from_email":"bob@example.com","include_link_to_client_invoice":true,"send_me_a_copy":false,"thank_you":true,
		"reminder":true,"send_reminder_on":"2017-09-01","created_at":"2017-08-23T22:25:59Z","updated_at":"2017-08-23T22:25:59Z",
		"attach_pdf":false,"event_type":"send","recipients":[],"subject":null,"body":null}
		""";

	[Fact]
	public async Task ListAsync_NoOptions_SendsNoQuery()
	{
		var call = await TestClient.SendAsync((c, ct) => c.InvoiceMessages.ListAsync(13150403, null, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/invoices/13150403/messages");
	}

	[Fact]
	public async Task ListAsync_AllOptions_SendsEveryParameter()
	{
		var call = await TestClient.SendAsync((c, ct) => c.InvoiceMessages.ListAsync(13150403, new ListOptions
		{
			UpdatedSince = new DateTimeOffset(2017, 8, 23, 22, 0, 0, TimeSpan.Zero),
			Cursor = "c1",
			Page = 3,
			PerPage = 10
		}, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/invoices/13150403/messages",
			"?updated_since=2017-08-23T22%3A00%3A00Z&cursor=c1&page=3&per_page=10");
	}

	[Fact]
	public async Task ListAsync_MapsTheMessagesAndPaging()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.InvoiceMessages.ListAsync(13150403, null, ct), $$$"""
			{"invoice_messages":[{{{MessageJson}}},{{{EventMessageJson}}}],"per_page":2000,"total_pages":1,"total_entries":2,
			"next_page":null,"previous_page":null,"page":1,"links":{"first":"https://api.harvestapp.com/api/v2/invoices/13150403/messages?page=1&per_page=2000",
			"next":null,"previous":null,"last":"https://api.harvestapp.com/v2/invoices/13150403/messages?page=1&per_page=2000"}}
			""");

		page.InvoiceMessages.Should().HaveCount(2);
		ShouldBeTheMessage(page.InvoiceMessages[0]);
		ShouldBeTheEventMessage(page.InvoiceMessages[1]);
		page.TotalEntries.Should().Be(2);
		page.Links!.Last.Should().Be("https://api.harvestapp.com/v2/invoices/13150403/messages?page=1&per_page=2000");
	}

	[Fact]
	public async Task GetTemplateAsync_NoOptions_SendsNoQuery()
	{
		var call = await TestClient.SendAsync((c, ct) => c.InvoiceMessages.GetTemplateAsync(13150403, null, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/invoices/13150403/messages/new");
	}

	[Fact]
	public async Task GetTemplateAsync_AllOptions_SendsEveryParameter()
	{
		var call = await TestClient.SendAsync((c, ct) => c.InvoiceMessages.GetTemplateAsync(13150403,
			new InvoiceMessageTemplateOptions { ThankYou = false, Reminder = true }, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/invoices/13150403/messages/new", "?thank_you=false&reminder=true");
	}

	[Fact]
	public async Task GetTemplateAsync_MapsTheTemplate()
	{
		var template = await TestClient.ReadAsync((c, ct) => c.InvoiceMessages.GetTemplateAsync(13150403,
			new InvoiceMessageTemplateOptions { Reminder = true }, ct), """
			{"invoice_id":13150403,"subject":"Past due invoice reminder: #1002 from API Examples",
			"body":"Dear Customer,\n\nThis is a friendly reminder.\n","reminder":true,"thank_you":false}
			""");

		template.InvoiceId.Should().Be(13150403);
		template.Subject.Should().Be("Past due invoice reminder: #1002 from API Examples");
		template.Body.Should().Be("Dear Customer,\n\nThis is a friendly reminder.\n");
		template.Reminder.Should().BeTrue();
		template.ThankYou.Should().BeFalse();
	}

	[Fact]
	public async Task CreateAsync_DocsExample_SendsTheMessageAndMapsIt()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.Created, MessageJson);
		using var client = TestClient.Create(stub);

		var message = await client.InvoiceMessages.CreateAsync(13150403, new InvoiceMessageCreationDto
		{
			Subject = "Invoice #1001",
			Body = "The invoice is attached below.",
			AttachPdf = true,
			SendMeACopy = true,
			Recipients = [new MessageRecipientCreationDto { Name = "Richard Roe", Email = "richardroe@example.com" }]
		}, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Subject.ShouldBe(HttpMethod.Post, "/v2/invoices/13150403/messages", body: """
			{"recipients":[{"name":"Richard Roe","email":"richardroe@example.com"}],"subject":"Invoice #1001","body":"The invoice is attached below.","attach_pdf":true,"send_me_a_copy":true}
			""");
		ShouldBeTheMessage(message);
	}

	[Fact]
	public async Task CreateAsync_AllFields_SendsEveryField()
	{
		var call = await TestClient.SendAsync((c, ct) => c.InvoiceMessages.CreateAsync(13150403, new InvoiceMessageCreationDto
		{
			Recipients =
			[
				new MessageRecipientCreationDto { Name = "Richard Roe", Email = "richardroe@example.com" },
				new MessageRecipientCreationDto { Email = "accounts@example.com" }
			],
			Subject = "s",
			Body = "b",
			IncludeLinkToClientInvoice = false,
			AttachPdf = false,
			SendMeACopy = false,
			ThankYou = true,
			EventType = null
		}, ct), MessageJson);

		call.ShouldBe(HttpMethod.Post, "/v2/invoices/13150403/messages", body: """
			{"recipients":[{"name":"Richard Roe","email":"richardroe@example.com"},{"email":"accounts@example.com"}],"subject":"s","body":"b","include_link_to_client_invoice":false,"attach_pdf":false,"send_me_a_copy":false,"thank_you":true}
			""");
	}

	public static TheoryData<string, string> EventTypes => new()
	{
		{ InvoiceMessageEventType.Send, "send" },
		{ InvoiceMessageEventType.Close, "close" },
		{ InvoiceMessageEventType.ReOpen, "re-open" },
		{ InvoiceMessageEventType.Draft, "draft" }
	};

	[Theory]
	[MemberData(nameof(EventTypes))]
	public async Task CreateAsync_EventType_SendsOnlyTheEventType(string eventType, string wireValue)
	{
		var call = await TestClient.SendAsync((c, ct) => c.InvoiceMessages.CreateAsync(13150403,
			new InvoiceMessageCreationDto { EventType = eventType }, ct), EventMessageJson);

		call.ShouldBe(HttpMethod.Post, "/v2/invoices/13150403/messages", body: $$"""{"event_type":"{{wireValue}}"}""");
	}

	[Fact]
	public async Task CreateAsync_Empty_SendsEmptyObject()
	{
		var call = await TestClient.SendAsync((c, ct) => c.InvoiceMessages.CreateAsync(1, new InvoiceMessageCreationDto(), ct), MessageJson);

		call.ShouldBe(HttpMethod.Post, "/v2/invoices/1/messages", body: "{}");
	}

	[Fact]
	public async Task DeleteAsync_SendsDelete()
	{
		var call = await TestClient.SendAsync((c, ct) => c.InvoiceMessages.DeleteAsync(13150403, 27835324, ct), "");

		call.ShouldBe(HttpMethod.Delete, "/v2/invoices/13150403/messages/27835324");
	}

	private static void ShouldBeTheMessage(InvoiceMessage message)
	{
		message.Id.Should().Be(27835324);
		message.SentBy.Should().Be("Bob Powell");
		message.SentByEmail.Should().Be("bob@example.com");
		message.SentFrom.Should().Be("Bob Powell");
		message.SentFromEmail.Should().Be("bob@example.com");
		message.IncludeLinkToClientInvoice.Should().BeFalse();
		message.SendMeACopy.Should().BeTrue();
		message.ThankYou.Should().BeFalse();
		message.Reminder.Should().BeFalse();
		message.SendReminderOn.Should().BeNull();
		message.CreatedAt.Should().Be(new DateTime(2017, 8, 23, 22, 25, 59, DateTimeKind.Utc));
		message.UpdatedAt.Should().Be(new DateTime(2017, 8, 23, 22, 26, 10, DateTimeKind.Utc));
		message.AttachPdf.Should().BeTrue();
		message.EventType.Should().BeNull();
		message.Recipients.Should().HaveCount(2);
		message.Recipients[0].Name.Should().Be("Richard Roe");
		message.Recipients[0].Email.Should().Be("richardroe@example.com");
		message.Recipients[1].Name.Should().Be("Bob Powell");
		message.Recipients[1].Email.Should().Be("bob@example.com");
		message.Subject.Should().Be("Invoice #1001");
		message.Body.Should().Be("The invoice is attached below.");
	}

	private static void ShouldBeTheEventMessage(InvoiceMessage message)
	{
		message.Id.Should().Be(27835325);
		message.IncludeLinkToClientInvoice.Should().BeTrue();
		message.ThankYou.Should().BeTrue();
		message.Reminder.Should().BeTrue();
		message.SendReminderOn.Should().Be(new DateOnly(2017, 9, 1));
		message.AttachPdf.Should().BeFalse();
		message.EventType.Should().Be(InvoiceMessageEventType.Send);
		message.Recipients.Should().BeEmpty();
		message.Subject.Should().BeNull();
		message.Body.Should().BeNull();
	}
}