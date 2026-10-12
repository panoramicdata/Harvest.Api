using Harvest.UnitTest.Support;

namespace Harvest.UnitTest.Resources;

public class EstimateApiTests
{
	private const string EstimateJson = """
		{"id":1439818,"client_key":"13dc088aa7d51ec687f186b146730c3c75dc7423","number":"1001","purchase_order":"5678",
		"amount":9630.0,"tax":5.0,"tax_amount":450.0,"tax2":2.0,"tax2_amount":180.0,"discount":10.0,"discount_amount":1000.0,
		"subject":"Online Store - Phase 2","notes":"Some notes about the estimate","state":"sent","issue_date":"2017-06-01",
		"sent_at":"2017-06-27T16:11:33Z","created_at":"2017-06-27T16:11:24Z","updated_at":"2017-06-27T16:13:56Z",
		"accepted_at":"2017-06-28T09:00:00Z","declined_at":"2017-06-29T10:30:00Z","currency":"USD",
		"client":{"id":5735776,"name":"123 Industries"},"creator":{"id":1782884,"name":"Bob Powell"},
		"line_items":[{"id":53334195,"kind":"Service","description":"Phase 2 of the Online Store","quantity":100.0,
		"unit_price":100.0,"amount":10000.0,"taxed":true,"taxed2":true}]}
		""";

	// The create example: a draft with no tax, discount, purchase order, notes or issue date.
	private const string DraftEstimateJson = """
		{"id":1439827,"client_key":"ddd4504a68fb7339138d0c2ea89ba05a3cf12aa8","number":"1002","purchase_order":null,
		"amount":5000.0,"tax":null,"tax_amount":0.0,"tax2":null,"tax2_amount":0.0,"discount":null,"discount_amount":0.0,
		"subject":"Project Quote","notes":null,"state":"draft","issue_date":null,"sent_at":null,
		"created_at":"2017-06-27T16:16:24Z","updated_at":"2017-06-27T16:16:24Z","accepted_at":null,"declined_at":null,
		"currency":"USD","client":{"id":5735774,"name":"ABC Corp"},"creator":{"id":1782884,"name":"Bob Powell"},
		"line_items":[{"id":53339199,"kind":"Service","description":"Project Description","quantity":1.0,
		"unit_price":5000.0,"amount":5000.0,"taxed":false,"taxed2":false}]}
		""";

	[Fact]
	public async Task ListAsync_NoOptions_SendsNoQuery()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Estimates.ListAsync(null, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/estimates");
	}

	[Fact]
	public async Task ListAsync_AllFilters_SendsEveryParameter()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Estimates.ListAsync(new EstimateListOptions
		{
			ClientId = 5735776,
			From = new DateOnly(2017, 1, 1),
			To = new DateOnly(2017, 6, 30),
			State = EstimateState.Sent,
			UpdatedSince = new DateTimeOffset(2017, 6, 26, 21, 52, 18, TimeSpan.Zero),
			Cursor = "c1",
			Page = 2,
			PerPage = 100
		}, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/estimates",
			"?client_id=5735776&from=2017-01-01&to=2017-06-30&state=sent&updated_since=2017-06-26T21%3A52%3A18Z&cursor=c1&page=2&per_page=100");
	}

	[Theory]
	[InlineData(EstimateState.Draft, "draft")]
	[InlineData(EstimateState.Sent, "sent")]
	[InlineData(EstimateState.Accepted, "accepted")]
	[InlineData(EstimateState.Declined, "declined")]
	public async Task ListAsync_StateFilter_SendsTheState(string state, string expected)
	{
		var call = await TestClient.SendAsync((c, ct) => c.Estimates.ListAsync(new EstimateListOptions { State = state }, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/estimates", $"?state={expected}");
	}

	[Fact]
	public async Task ListAsync_MapsTheEstimatesAndPaging()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.Estimates.ListAsync(null, ct), $$$"""
			{"estimates":[{{{EstimateJson}}},{{{DraftEstimateJson}}}],"per_page":2000,"total_pages":1,"total_entries":2,
			"next_page":null,"previous_page":null,"page":1,
			"links":{"first":"https://api.harvestapp.com/v2/estimates?page=1&per_page=2000","next":null,"previous":null,"last":"https://api.harvestapp.com/v2/estimates?page=1&per_page=2000"}}
			""");

		page.Estimates.Should().HaveCount(2);
		ShouldBeTheEstimate(page.Estimates[0]);
		ShouldBeTheDraftEstimate(page.Estimates[1]);
		page.PerPage.Should().Be(2000);
		page.TotalPages.Should().Be(1);
		page.TotalEntries.Should().Be(2);
		page.NextPage.Should().BeNull();
		page.Links!.Next.Should().BeNull();
	}

	[Fact]
	public async Task GetAsync_SendsGetAndMapsTheEstimate()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.OK, EstimateJson);
		using var client = TestClient.Create(stub);

		var estimate = await client.Estimates.GetAsync(1439818, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Subject.ShouldBe(HttpMethod.Get, "/v2/estimates/1439818");
		ShouldBeTheEstimate(estimate);
	}

	[Fact]
	public async Task GetAsync_ToleratesMissingOptionalFields()
	{
		var estimate = await TestClient.ReadAsync((c, ct) => c.Estimates.GetAsync(1, ct),
			"""{"id":1,"created_at":"2017-06-27T16:16:24Z","updated_at":"2017-06-27T16:16:24Z"}""");

		estimate.Id.Should().Be(1);
		estimate.Client.Should().BeNull();
		estimate.Creator.Should().BeNull();
		estimate.LineItems.Should().BeNull();
		estimate.Amount.Should().BeNull();
		estimate.State.Should().BeNull();
	}

	[Fact]
	public async Task CreateAsync_RequiredOnly_SendsClientId()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Estimates.CreateAsync(new EstimateCreationDto { ClientId = 5735774 }, ct), DraftEstimateJson);

		call.ShouldBe(HttpMethod.Post, "/v2/estimates", body: """{"client_id":5735774}""");
	}

	[Fact]
	public async Task CreateAsync_DocsExample_SendsSubjectAndLineItem()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.Created, DraftEstimateJson);
		using var client = TestClient.Create(stub);

		var estimate = await client.Estimates.CreateAsync(new EstimateCreationDto
		{
			ClientId = 5735774,
			Subject = "ABC Project Quote",
			LineItems = [new EstimateLineItemCreationDto { Kind = "Service", Description = "ABC Project Quote", UnitPrice = 5000.0m }]
		}, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Subject.ShouldBe(HttpMethod.Post, "/v2/estimates", body: """
			{"client_id":5735774,"subject":"ABC Project Quote","line_items":[{"kind":"Service","description":"ABC Project Quote","unit_price":5000.0}]}
			""");
		ShouldBeTheDraftEstimate(estimate);
	}

	[Fact]
	public async Task CreateAsync_AllFields_SendsEveryField()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Estimates.CreateAsync(new EstimateCreationDto
		{
			ClientId = 1,
			Number = "1003",
			PurchaseOrder = "PO-9",
			Tax = 5m,
			Tax2 = 2.5m,
			Discount = 10m,
			Subject = "Quote",
			Notes = "n",
			Currency = "EUR",
			IssueDate = new DateOnly(2025, 2, 3),
			LineItems =
			[
				new EstimateLineItemCreationDto
				{
					Kind = "Product",
					Description = "d",
					Quantity = 3m,
					UnitPrice = 12.5m,
					Taxed = true,
					Taxed2 = false
				},
				new EstimateLineItemCreationDto { Kind = "Service", UnitPrice = 100m }
			]
		}, ct), DraftEstimateJson);

		call.ShouldBe(HttpMethod.Post, "/v2/estimates", body: """
			{"client_id":1,"number":"1003","purchase_order":"PO-9","tax":5,"tax2":2.5,"discount":10,"subject":"Quote","notes":"n","currency":"EUR","issue_date":"2025-02-03","line_items":[{"kind":"Product","description":"d","quantity":3,"unit_price":12.5,"taxed":true,"taxed2":false},{"kind":"Service","unit_price":100}]}
			""");
	}

	[Fact]
	public async Task UpdateAsync_DocsExample_SendsPurchaseOrder()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Estimates.UpdateAsync(1439827, new EstimatePatchDto { PurchaseOrder = "2345" }, ct), DraftEstimateJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/estimates/1439827", body: """{"purchase_order":"2345"}""");
	}

	[Fact]
	public async Task UpdateAsync_AllFields_SendsPatch()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Estimates.UpdateAsync(9, new EstimatePatchDto
		{
			ClientId = 2,
			Number = "N",
			PurchaseOrder = "P",
			Tax = 1m,
			Tax2 = 2m,
			Discount = 3m,
			Subject = "S",
			Notes = "x",
			Currency = "GBP",
			IssueDate = new DateOnly(2025, 3, 4),
			LineItems =
			[
				new EstimateLineItemPatchDto
				{
					Id = 7,
					Kind = "Service",
					Description = "d",
					Quantity = 1.5m,
					UnitPrice = 4m,
					Taxed = false,
					Taxed2 = true,
					Destroy = false
				}
			]
		}, ct), DraftEstimateJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/estimates/9", body: """
			{"client_id":2,"number":"N","purchase_order":"P","tax":1,"tax2":2,"discount":3,"subject":"S","notes":"x","currency":"GBP","issue_date":"2025-03-04","line_items":[{"id":7,"kind":"Service","description":"d","quantity":1.5,"unit_price":4,"taxed":false,"taxed2":true,"_destroy":false}]}
			""");
	}

	[Fact]
	public async Task UpdateAsync_EmptyPatch_SendsEmptyObject()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Estimates.UpdateAsync(9, new EstimatePatchDto(), ct), DraftEstimateJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/estimates/9", body: "{}");
	}

	[Fact]
	public async Task UpdateAsync_AddLineItem_SendsLineItemWithoutId()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.OK, """
			{"id":1439827,"amount":6000.0,"state":"draft","created_at":"2017-06-27T16:16:24Z","updated_at":"2017-06-27T16:17:06Z",
			"line_items":[{"id":53339199,"kind":"Service","description":"Project Description","quantity":1.0,"unit_price":5000.0,"amount":5000.0,"taxed":false,"taxed2":false},
			{"id":53339200,"kind":"Service","description":"Another Project","quantity":1.0,"unit_price":1000.0,"amount":1000.0,"taxed":false,"taxed2":false}]}
			""");
		using var client = TestClient.Create(stub);

		var estimate = await client.Estimates.UpdateAsync(1439827, new EstimatePatchDto
		{
			LineItems = [new EstimateLineItemPatchDto { Kind = "Service", Description = "Another Project", UnitPrice = 1000.0m }]
		}, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Subject.ShouldBe(HttpMethod.Patch, "/v2/estimates/1439827",
			body: """{"line_items":[{"kind":"Service","description":"Another Project","unit_price":1000.0}]}""");
		estimate.Amount.Should().Be(6000m);
		estimate.LineItems.Should().HaveCount(2);
		estimate.LineItems[1].Id.Should().Be(53339200);
		estimate.LineItems[1].Description.Should().Be("Another Project");
	}

	[Fact]
	public async Task UpdateAsync_UpdateLineItem_SendsLineItemWithId()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Estimates.UpdateAsync(1439827, new EstimatePatchDto
		{
			LineItems = [new EstimateLineItemPatchDto { Id = 53339199, Description = "Project Phase 2", UnitPrice = 5000.0m }]
		}, ct), DraftEstimateJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/estimates/1439827",
			body: """{"line_items":[{"id":53339199,"description":"Project Phase 2","unit_price":5000.0}]}""");
	}

	[Fact]
	public async Task UpdateAsync_DeleteLineItem_SendsDestroy()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Estimates.UpdateAsync(1439827, new EstimatePatchDto
		{
			LineItems = [EstimateLineItemPatchDto.Destroying(53339199)]
		}, ct), DraftEstimateJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/estimates/1439827", body: """{"line_items":[{"id":53339199,"_destroy":true}]}""");
	}

	[Fact]
	public void Destroying_SetsIdAndDestroyOnly()
	{
		var change = EstimateLineItemPatchDto.Destroying(5);

		change.Id.Should().Be(5);
		change.Destroy.Should().BeTrue();
		change.Kind.Should().BeNull();
		change.UnitPrice.Should().BeNull();
	}

	[Fact]
	public async Task DeleteAsync_SendsDelete()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Estimates.DeleteAsync(1439827, ct), "");

		call.ShouldBe(HttpMethod.Delete, "/v2/estimates/1439827");
	}

	private static void ShouldBeTheEstimate(Estimate estimate)
	{
		estimate.Id.Should().Be(1439818);
		estimate.ClientKey.Should().Be("13dc088aa7d51ec687f186b146730c3c75dc7423");
		estimate.Number.Should().Be("1001");
		estimate.PurchaseOrder.Should().Be("5678");
		estimate.Amount.Should().Be(9630m);
		estimate.Tax.Should().Be(5m);
		estimate.TaxAmount.Should().Be(450m);
		estimate.Tax2.Should().Be(2m);
		estimate.Tax2Amount.Should().Be(180m);
		estimate.Discount.Should().Be(10m);
		estimate.DiscountAmount.Should().Be(1000m);
		estimate.Subject.Should().Be("Online Store - Phase 2");
		estimate.Notes.Should().Be("Some notes about the estimate");
		estimate.State.Should().Be(EstimateState.Sent);
		estimate.IssueDate.Should().Be(new DateOnly(2017, 6, 1));
		estimate.SentAt.Should().Be(new DateTime(2017, 6, 27, 16, 11, 33, DateTimeKind.Utc));
		estimate.CreatedAt.Should().Be(new DateTime(2017, 6, 27, 16, 11, 24, DateTimeKind.Utc));
		estimate.UpdatedAt.Should().Be(new DateTime(2017, 6, 27, 16, 13, 56, DateTimeKind.Utc));
		estimate.AcceptedAt.Should().Be(new DateTime(2017, 6, 28, 9, 0, 0, DateTimeKind.Utc));
		estimate.DeclinedAt.Should().Be(new DateTime(2017, 6, 29, 10, 30, 0, DateTimeKind.Utc));
		estimate.Currency.Should().Be("USD");
		estimate.Client!.Id.Should().Be(5735776);
		estimate.Client.Name.Should().Be("123 Industries");
		estimate.Creator!.Id.Should().Be(1782884);
		estimate.Creator.Name.Should().Be("Bob Powell");
		var lineItem = estimate.LineItems.Should().ContainSingle().Subject;
		lineItem.Id.Should().Be(53334195);
		lineItem.Kind.Should().Be("Service");
		lineItem.Description.Should().Be("Phase 2 of the Online Store");
		lineItem.Quantity.Should().Be(100m);
		lineItem.UnitPrice.Should().Be(100m);
		lineItem.Amount.Should().Be(10000m);
		lineItem.Taxed.Should().BeTrue();
		lineItem.Taxed2.Should().BeTrue();
	}

	private static void ShouldBeTheDraftEstimate(Estimate estimate)
	{
		estimate.Id.Should().Be(1439827);
		estimate.PurchaseOrder.Should().BeNull();
		estimate.Amount.Should().Be(5000m);
		estimate.Tax.Should().BeNull();
		estimate.TaxAmount.Should().Be(0m);
		estimate.Tax2.Should().BeNull();
		estimate.Discount.Should().BeNull();
		estimate.Notes.Should().BeNull();
		estimate.State.Should().Be(EstimateState.Draft);
		estimate.IssueDate.Should().BeNull();
		estimate.SentAt.Should().BeNull();
		estimate.AcceptedAt.Should().BeNull();
		estimate.DeclinedAt.Should().BeNull();
		estimate.Client!.Name.Should().Be("ABC Corp");
		var lineItem = estimate.LineItems.Should().ContainSingle().Subject;
		lineItem.Quantity.Should().Be(1m);
		lineItem.Taxed.Should().BeFalse();
		lineItem.Taxed2.Should().BeFalse();
	}
}