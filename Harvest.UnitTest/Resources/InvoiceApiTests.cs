using Harvest.UnitTest.Support;

namespace Harvest.UnitTest.Resources;

public class InvoiceApiTests
{
	// "Retrieve an invoice" example from the Harvest docs.
	private const string InvoiceJson = """
		{"id":13150378,"client_key":"9e97f4a65c5b83b1fc02f54e5a41c9dc7d458542","number":"1000","purchase_order":"1234",
		"amount":10700.0,"due_amount":0.0,"tax":5.0,"tax_amount":500.0,"tax2":2.0,"tax2_amount":200.0,"discount":null,
		"discount_amount":0.0,"subject":"Online Store - Phase 1","notes":"Some notes about the invoice.","state":"paid",
		"period_start":null,"period_end":null,"issue_date":"2017-02-01","due_date":"2017-03-03","payment_term":"custom",
		"sent_at":"2017-02-01T07:00:00Z","paid_at":"2017-02-21T00:00:00Z","paid_date":"2017-02-21","closed_at":null,
		"recurring_invoice_id":null,"created_at":"2017-06-27T16:24:30Z","updated_at":"2017-06-27T16:24:57Z","currency":"USD",
		"payment_options":["credit_card"],"client":{"id":5735776,"name":"123 Industries"},"estimate":{"id":1439814},
		"retainer":null,"creator":{"id":1782884,"name":"Bob Powell"},"line_items":[{"id":53341450,"kind":"Service",
		"description":"50% of Phase 1 of the Online Store","quantity":100.0,"unit_price":100.0,"amount":10000.0,"taxed":true,
		"taxed2":true,"project":{"id":14308069,"name":"Online Store - Phase 1","code":"OS1"}}]}
		""";

	// The first invoice of the "List all invoices" example, with a retainer, closed date and recurring invoice added so that
	// every nullable field is read with a value.
	private const string OpenInvoiceJson = """
		{"id":13150403,"client_key":"21312da13d457947a217da6775477afee8c2eba8","number":"1001","purchase_order":"",
		"amount":288.9,"due_amount":288.9,"tax":5,"tax_amount":13.5,"tax2":2,"tax2_amount":5.4,"discount":10,
		"discount_amount":30,"subject":"Online Store - Phase 1","notes":"Some notes about the invoice.","state":"open",
		"period_start":"2017-03-01","period_end":"2017-03-02","issue_date":"2017-04-01","due_date":"2017-04-01",
		"payment_term":"upon receipt","sent_at":"2017-08-23T22:25:59Z","paid_at":null,"paid_date":null,
		"closed_at":"2017-09-01T10:00:00Z","recurring_invoice_id":42,"created_at":"2017-06-27T16:27:16Z",
		"updated_at":"2017-08-23T22:25:59Z","currency":"EUR","payment_options":["credit_card"],
		"client":{"id":5735776,"name":"123 Industries"},"estimate":null,"retainer":{"id":7},
		"creator":{"id":1782884,"name":"Bob Powell"},"line_items":[{"id":53341602,"kind":"Service",
		"description":"03/01/2017 - Project Management: [9:00am - 11:00am] Planning meetings","quantity":2,"unit_price":100,
		"amount":200,"taxed":true,"taxed2":false,"project":null}]}
		""";

	[Fact]
	public async Task ListAsync_NoOptions_SendsNoQuery()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Invoices.ListAsync(null, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/invoices");
	}

	[Fact]
	public async Task ListAsync_AllFilters_SendsEveryParameter()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Invoices.ListAsync(new InvoiceListOptions
		{
			ClientId = 5735776,
			ProjectId = 14308069,
			From = new DateOnly(2017, 1, 1),
			To = new DateOnly(2017, 12, 31),
			State = InvoiceState.Open,
			UpdatedSince = new DateTimeOffset(2017, 6, 26, 21, 52, 18, TimeSpan.Zero),
			Cursor = "abc",
			Page = 2,
			PerPage = 100
		}, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/invoices",
			"?client_id=5735776&project_id=14308069&from=2017-01-01&to=2017-12-31&state=open&updated_since=2017-06-26T21%3A52%3A18Z&cursor=abc&page=2&per_page=100");
	}

	[Fact]
	public async Task ListAsync_MapsTheInvoicesAndPaging()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.Invoices.ListAsync(null, ct), $$$"""
			{"invoices":[{{{InvoiceJson}}},{{{OpenInvoiceJson}}}],"per_page":100,"total_pages":1,"total_entries":2,"next_page":null,
			"previous_page":null,"page":1,"links":{"first":"https://api.harvestapp.com/v2/invoices?page=1&per_page=100","next":null,
			"previous":null,"last":"https://api.harvestapp.com/v2/invoices?page=1&per_page=100"}}
			""");

		page.Invoices.Should().HaveCount(2);
		ShouldBeTheInvoice(page.Invoices[0]);
		ShouldBeTheOpenInvoice(page.Invoices[1]);
		page.PerPage.Should().Be(100);
		page.TotalEntries.Should().Be(2);
		page.Links!.First.Should().Be("https://api.harvestapp.com/v2/invoices?page=1&per_page=100");
	}

	[Fact]
	public async Task GetAsync_SendsGetAndMapsTheInvoice()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.OK, InvoiceJson);
		using var client = TestClient.Create(stub);

		var invoice = await client.Invoices.GetAsync(13150378, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Subject.ShouldBe(HttpMethod.Get, "/v2/invoices/13150378");
		ShouldBeTheInvoice(invoice);
	}

	[Fact]
	public async Task GetAsync_MinimalInvoice_LeavesOmittedFieldsNull()
	{
		var invoice = await TestClient.ReadAsync((c, ct) => c.Invoices.GetAsync(1, ct), """
			{"id":1,"amount":0,"due_amount":0,"created_at":"2017-06-27T16:34:24Z","updated_at":"2017-06-27T16:34:24Z"}
			""");

		invoice.Id.Should().Be(1);
		invoice.Client.Should().BeNull();
		invoice.LineItems.Should().BeNull();
		invoice.Estimate.Should().BeNull();
		invoice.Retainer.Should().BeNull();
		invoice.Creator.Should().BeNull();
		invoice.Number.Should().BeNull();
		invoice.Tax.Should().BeNull();
		invoice.TaxAmount.Should().BeNull();
		invoice.State.Should().BeNull();
		invoice.IssueDate.Should().BeNull();
		invoice.DueDate.Should().BeNull();
		invoice.PaymentOptions.Should().BeNull();
		invoice.SentAt.Should().BeNull();
	}

	[Fact]
	public async Task CreateAsync_FreeFormRequiredOnly_SendsRequiredFields()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Invoices.CreateAsync(new InvoiceCreationDto
		{
			ClientId = 5735774
		}, ct), InvoiceJson);

		call.ShouldBe(HttpMethod.Post, "/v2/invoices", body: """{"client_id":5735774}""");
	}

	[Fact]
	public async Task CreateAsync_FreeFormDocsExample_SendsTheLineItems()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Invoices.CreateAsync(new InvoiceCreationDto
		{
			ClientId = 5735774,
			Subject = "ABC Project Quote",
			DueDate = new DateOnly(2017, 7, 27),
			LineItems = [new InvoiceLineItemCreationDto { Kind = "Service", Description = "ABC Project", UnitPrice = 5000.0m }]
		}, ct), InvoiceJson);

		call.ShouldBe(HttpMethod.Post, "/v2/invoices", body: """
			{"client_id":5735774,"subject":"ABC Project Quote","due_date":"2017-07-27","line_items":[{"kind":"Service","description":"ABC Project","unit_price":5000.0}]}
			""");
	}

	[Fact]
	public async Task CreateAsync_FreeFormAllFields_SendsEveryField()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Invoices.CreateAsync(new InvoiceCreationDto
		{
			ClientId = 1,
			RetainerId = 2,
			EstimateId = 3,
			Number = "1002",
			PurchaseOrder = "PO-9",
			Tax = 10.0m,
			Tax2 = 2.5m,
			Discount = 5m,
			Subject = "s",
			Notes = "n",
			Currency = "EUR",
			IssueDate = new DateOnly(2025, 1, 2),
			DueDate = new DateOnly(2025, 2, 3),
			PaymentTerm = InvoicePaymentTerm.Custom,
			PaymentOptions = [InvoicePaymentOption.Ach, InvoicePaymentOption.CreditCard, InvoicePaymentOption.Paypal],
			LineItems =
			[
				new InvoiceLineItemCreationDto
				{
					ProjectId = 4,
					Kind = "Product",
					Description = "d",
					Quantity = 2.5m,
					UnitPrice = 10m,
					Taxed = true,
					Taxed2 = false
				}
			]
		}, ct), InvoiceJson);

		call.ShouldBe(HttpMethod.Post, "/v2/invoices", body: """
			{"client_id":1,"retainer_id":2,"estimate_id":3,"number":"1002","purchase_order":"PO-9","tax":10.0,"tax2":2.5,"discount":5,"subject":"s","notes":"n","currency":"EUR","issue_date":"2025-01-02","due_date":"2025-02-03","payment_term":"custom","payment_options":["ach","credit_card","paypal"],"line_items":[{"project_id":4,"kind":"Product","description":"d","quantity":2.5,"unit_price":10,"taxed":true,"taxed2":false}]}
			""");
	}

	[Fact]
	public async Task CreateAsync_ImportDocsExample_SendsTheLineItemsImport()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Invoices.CreateAsync(new InvoiceCreationDto
		{
			ClientId = 5735774,
			Subject = "ABC Project Quote",
			PaymentTerm = InvoicePaymentTerm.UponReceipt,
			LineItemsImport = new InvoiceLineItemsImportCreationDto
			{
				ProjectIds = [14307913],
				Time = new InvoiceTimeImportCreationDto
				{
					SummaryType = InvoiceTimeSummaryType.Task,
					From = new DateOnly(2017, 3, 1),
					To = new DateOnly(2017, 3, 31)
				},
				Expenses = new InvoiceExpenseImportCreationDto { SummaryType = InvoiceExpenseSummaryType.Category }
			}
		}, ct), InvoiceJson);

		call.ShouldBe(HttpMethod.Post, "/v2/invoices", body: """
			{"client_id":5735774,"subject":"ABC Project Quote","payment_term":"upon receipt","line_items_import":{"project_ids":[14307913],"time":{"summary_type":"task","from":"2017-03-01","to":"2017-03-31"},"expenses":{"summary_type":"category"}}}
			""");
	}

	[Fact]
	public async Task CreateAsync_ImportAllFields_SendsEveryImportField()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Invoices.CreateAsync(new InvoiceCreationDto
		{
			ClientId = 1,
			LineItemsImport = new InvoiceLineItemsImportCreationDto
			{
				ProjectIds = [10, 11],
				Time = new InvoiceTimeImportCreationDto { SummaryType = InvoiceTimeSummaryType.Detailed },
				Expenses = new InvoiceExpenseImportCreationDto
				{
					SummaryType = InvoiceExpenseSummaryType.People,
					From = new DateOnly(2025, 1, 1),
					To = new DateOnly(2025, 1, 31),
					AttachReceipts = true
				}
			}
		}, ct), InvoiceJson);

		call.ShouldBe(HttpMethod.Post, "/v2/invoices", body: """
			{"client_id":1,"line_items_import":{"project_ids":[10,11],"time":{"summary_type":"detailed"},"expenses":{"summary_type":"people","from":"2025-01-01","to":"2025-01-31","attach_receipts":true}}}
			""");
	}

	[Fact]
	public async Task CreateAsync_ImportProjectsOnly_SendsOnlyProjectIds()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Invoices.CreateAsync(new InvoiceCreationDto
		{
			ClientId = 1,
			LineItemsImport = new InvoiceLineItemsImportCreationDto { ProjectIds = [10] }
		}, ct), InvoiceJson);

		call.ShouldBe(HttpMethod.Post, "/v2/invoices", body: """{"client_id":1,"line_items_import":{"project_ids":[10]}}""");
	}

	[Fact]
	public async Task UpdateAsync_DocsExample_SendsPurchaseOrder()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Invoices.UpdateAsync(13150453, new InvoicePatchDto { PurchaseOrder = "2345" }, ct), InvoiceJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/invoices/13150453", body: """{"purchase_order":"2345"}""");
	}

	[Fact]
	public async Task UpdateAsync_AllFields_SendsPatch()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Invoices.UpdateAsync(9, new InvoicePatchDto
		{
			ClientId = 1,
			RetainerId = 2,
			EstimateId = 3,
			Number = "n1",
			PurchaseOrder = "po",
			Tax = 1m,
			Tax2 = 2m,
			Discount = 3m,
			Subject = "s",
			Notes = "n",
			Currency = "GBP",
			IssueDate = new DateOnly(2025, 3, 4),
			DueDate = new DateOnly(2025, 4, 5),
			PaymentTerm = InvoicePaymentTerm.Net30,
			PaymentOptions = [InvoicePaymentOption.Paypal],
			LineItems =
			[
				new InvoiceLineItemPatchDto
				{
					Id = 5,
					ProjectId = 6,
					Kind = "Service",
					Description = "d",
					Quantity = 7m,
					UnitPrice = 8m,
					Taxed = false,
					Taxed2 = true,
					Destroy = false
				}
			]
		}, ct), InvoiceJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/invoices/9", body: """
			{"client_id":1,"retainer_id":2,"estimate_id":3,"number":"n1","purchase_order":"po","tax":1,"tax2":2,"discount":3,"subject":"s","notes":"n","currency":"GBP","issue_date":"2025-03-04","due_date":"2025-04-05","payment_term":"net 30","payment_options":["paypal"],"line_items":[{"id":5,"project_id":6,"kind":"Service","description":"d","quantity":7,"unit_price":8,"taxed":false,"taxed2":true,"_destroy":false}]}
			""");
	}

	[Fact]
	public async Task UpdateAsync_AddLineItem_SendsLineItemWithoutId()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Invoices.UpdateAsync(13150403, new InvoicePatchDto
		{
			LineItems = [new InvoiceLineItemPatchDto { Kind = "Service", Description = "DEF Project", UnitPrice = 1000.0m }]
		}, ct), InvoiceJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/invoices/13150403",
			body: """{"line_items":[{"kind":"Service","description":"DEF Project","unit_price":1000.0}]}""");
	}

	[Fact]
	public async Task UpdateAsync_UpdateLineItem_SendsLineItemWithId()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Invoices.UpdateAsync(13150403, new InvoicePatchDto
		{
			LineItems = [new InvoiceLineItemPatchDto { Id = 53341928, Description = "ABC Project Phase 2", UnitPrice = 5000.0m }]
		}, ct), InvoiceJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/invoices/13150403",
			body: """{"line_items":[{"id":53341928,"description":"ABC Project Phase 2","unit_price":5000.0}]}""");
	}

	[Fact]
	public async Task UpdateAsync_DeleteLineItem_SendsDestroy()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Invoices.UpdateAsync(13150403, new InvoicePatchDto
		{
			LineItems = [new InvoiceLineItemPatchDto { Id = 53341928, Destroy = true }]
		}, ct), InvoiceJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/invoices/13150403", body: """{"line_items":[{"id":53341928,"_destroy":true}]}""");
	}

	[Fact]
	public async Task UpdateAsync_EmptyPatch_SendsEmptyObject()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Invoices.UpdateAsync(9, new InvoicePatchDto(), ct), InvoiceJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/invoices/9", body: "{}");
	}

	[Fact]
	public async Task DeleteAsync_SendsDelete()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Invoices.DeleteAsync(13150453, ct), "");

		call.ShouldBe(HttpMethod.Delete, "/v2/invoices/13150453");
	}

	private static void ShouldBeTheInvoice(Invoice invoice)
	{
		invoice.Id.Should().Be(13150378);
		invoice.ClientKey.Should().Be("9e97f4a65c5b83b1fc02f54e5a41c9dc7d458542");
		invoice.Number.Should().Be("1000");
		invoice.PurchaseOrder.Should().Be("1234");
		invoice.Amount.Should().Be(10700m);
		invoice.DueAmount.Should().Be(0m);
		invoice.Tax.Should().Be(5m);
		invoice.TaxAmount.Should().Be(500m);
		invoice.Tax2.Should().Be(2m);
		invoice.Tax2Amount.Should().Be(200m);
		invoice.Discount.Should().BeNull();
		invoice.DiscountAmount.Should().Be(0m);
		invoice.Subject.Should().Be("Online Store - Phase 1");
		invoice.Notes.Should().Be("Some notes about the invoice.");
		invoice.State.Should().Be(InvoiceState.Paid);
		invoice.PeriodStart.Should().BeNull();
		invoice.PeriodEnd.Should().BeNull();
		invoice.IssueDate.Should().Be(new DateOnly(2017, 2, 1));
		invoice.DueDate.Should().Be(new DateOnly(2017, 3, 3));
		invoice.PaymentTerm.Should().Be(InvoicePaymentTerm.Custom);
		invoice.SentAt.Should().Be(new DateTime(2017, 2, 1, 7, 0, 0, DateTimeKind.Utc));
		invoice.PaidAt.Should().Be(new DateTime(2017, 2, 21, 0, 0, 0, DateTimeKind.Utc));
		invoice.PaidDate.Should().Be(new DateOnly(2017, 2, 21));
		invoice.ClosedAt.Should().BeNull();
		invoice.RecurringInvoiceId.Should().BeNull();
		invoice.CreatedAt.Should().Be(new DateTime(2017, 6, 27, 16, 24, 30, DateTimeKind.Utc));
		invoice.UpdatedAt.Should().Be(new DateTime(2017, 6, 27, 16, 24, 57, DateTimeKind.Utc));
		invoice.Currency.Should().Be("USD");
		invoice.PaymentOptions.Should().Equal(InvoicePaymentOption.CreditCard);
		invoice.Client!.Id.Should().Be(5735776);
		invoice.Client.Name.Should().Be("123 Industries");
		invoice.Estimate!.Id.Should().Be(1439814);
		invoice.Retainer.Should().BeNull();
		invoice.Creator!.Id.Should().Be(1782884);
		invoice.Creator.Name.Should().Be("Bob Powell");
		var lineItem = invoice.LineItems.Should().ContainSingle().Subject;
		lineItem.Id.Should().Be(53341450);
		lineItem.Kind.Should().Be("Service");
		lineItem.Description.Should().Be("50% of Phase 1 of the Online Store");
		lineItem.Quantity.Should().Be(100m);
		lineItem.UnitPrice.Should().Be(100m);
		lineItem.Amount.Should().Be(10000m);
		lineItem.Taxed.Should().BeTrue();
		lineItem.Taxed2.Should().BeTrue();
		lineItem.Project!.Id.Should().Be(14308069);
		lineItem.Project.Name.Should().Be("Online Store - Phase 1");
		lineItem.Project.Code.Should().Be("OS1");
	}

	private static void ShouldBeTheOpenInvoice(Invoice invoice)
	{
		invoice.Id.Should().Be(13150403);
		invoice.PurchaseOrder.Should().BeEmpty();
		invoice.Discount.Should().Be(10m);
		invoice.DiscountAmount.Should().Be(30m);
		invoice.State.Should().Be(InvoiceState.Open);
		invoice.PeriodStart.Should().Be(new DateOnly(2017, 3, 1));
		invoice.PeriodEnd.Should().Be(new DateOnly(2017, 3, 2));
		invoice.PaymentTerm.Should().Be(InvoicePaymentTerm.UponReceipt);
		invoice.PaidAt.Should().BeNull();
		invoice.PaidDate.Should().BeNull();
		invoice.ClosedAt.Should().Be(new DateTime(2017, 9, 1, 10, 0, 0, DateTimeKind.Utc));
		invoice.RecurringInvoiceId.Should().Be(42);
		invoice.Estimate.Should().BeNull();
		invoice.Retainer!.Id.Should().Be(7);
		var lineItem = invoice.LineItems.Should().ContainSingle().Subject;
		lineItem.Taxed2.Should().BeFalse();
		lineItem.Project.Should().BeNull();
	}
}