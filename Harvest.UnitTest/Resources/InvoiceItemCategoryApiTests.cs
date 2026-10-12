using Harvest.UnitTest.Support;

namespace Harvest.UnitTest.Resources;

public class InvoiceItemCategoryApiTests
{
	private const string CategoryJson = """
		{"id":1466293,"name":"Product","use_as_service":false,"use_as_expense":true,"created_at":"2017-06-26T20:41:00Z",
		"updated_at":"2017-06-26T20:41:30Z"}
		""";

	[Fact]
	public async Task ListAsync_NoOptions_SendsNoQuery()
	{
		var call = await TestClient.SendAsync((c, ct) => c.InvoiceItemCategories.ListAsync(null, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/invoice_item_categories");
	}

	[Fact]
	public async Task ListAsync_AllOptions_SendsEveryParameter()
	{
		var call = await TestClient.SendAsync((c, ct) => c.InvoiceItemCategories.ListAsync(new ListOptions
		{
			UpdatedSince = new DateTimeOffset(2017, 6, 26, 21, 52, 18, TimeSpan.Zero),
			Cursor = "abc",
			Page = 2,
			PerPage = 50
		}, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/invoice_item_categories",
			"?updated_since=2017-06-26T21%3A52%3A18Z&cursor=abc&page=2&per_page=50");
	}

	[Fact]
	public async Task ListAsync_MapsTheCategoriesAndPaging()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.InvoiceItemCategories.ListAsync(null, ct), $$$"""
			{"invoice_item_categories":[{{{CategoryJson}}}],"per_page":2000,"total_pages":1,"total_entries":1,"next_page":null,
			"previous_page":null,"page":1,"links":{"first":"https://api.harvestapp.com/v2/invoice_item_categories?page=1&per_page=2000",
			"next":null,"previous":null,"last":"https://api.harvestapp.com/v2/invoice_item_categories?page=1&per_page=2000"}}
			""");

		ShouldBeTheCategory(page.InvoiceItemCategories.Should().ContainSingle().Subject);
		page.PerPage.Should().Be(2000);
		page.TotalEntries.Should().Be(1);
		page.Links!.Last.Should().Be("https://api.harvestapp.com/v2/invoice_item_categories?page=1&per_page=2000");
	}

	[Fact]
	public async Task GetAsync_SendsGetAndMapsTheCategory()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.OK, CategoryJson);
		using var client = TestClient.Create(stub);

		var category = await client.InvoiceItemCategories.GetAsync(1466293, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Subject.ShouldBe(HttpMethod.Get, "/v2/invoice_item_categories/1466293");
		ShouldBeTheCategory(category);
	}

	[Fact]
	public async Task CreateAsync_SendsTheName()
	{
		var call = await TestClient.SendAsync((c, ct) => c.InvoiceItemCategories.CreateAsync(new InvoiceItemCategoryCreationDto
		{
			Name = "Hosting"
		}, ct), CategoryJson);

		call.ShouldBe(HttpMethod.Post, "/v2/invoice_item_categories", body: """{"name":"Hosting"}""");
	}

	[Fact]
	public async Task UpdateAsync_AllFields_SendsPatch()
	{
		var call = await TestClient.SendAsync((c, ct) => c.InvoiceItemCategories.UpdateAsync(1467098, new InvoiceItemCategoryPatchDto
		{
			Name = "Expense"
		}, ct), CategoryJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/invoice_item_categories/1467098", body: """{"name":"Expense"}""");
	}

	[Fact]
	public async Task UpdateAsync_EmptyPatch_SendsEmptyObject()
	{
		var call = await TestClient.SendAsync((c, ct) => c.InvoiceItemCategories.UpdateAsync(1467098, new InvoiceItemCategoryPatchDto(), ct), CategoryJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/invoice_item_categories/1467098", body: "{}");
	}

	[Fact]
	public async Task DeleteAsync_SendsDelete()
	{
		var call = await TestClient.SendAsync((c, ct) => c.InvoiceItemCategories.DeleteAsync(1467098, ct), "");

		call.ShouldBe(HttpMethod.Delete, "/v2/invoice_item_categories/1467098");
	}

	private static void ShouldBeTheCategory(InvoiceItemCategory category)
	{
		category.Id.Should().Be(1466293);
		category.Name.Should().Be("Product");
		category.UseAsService.Should().BeFalse();
		category.UseAsExpense.Should().BeTrue();
		category.CreatedAt.Should().Be(new DateTime(2017, 6, 26, 20, 41, 0, DateTimeKind.Utc));
		category.UpdatedAt.Should().Be(new DateTime(2017, 6, 26, 20, 41, 30, DateTimeKind.Utc));
	}
}