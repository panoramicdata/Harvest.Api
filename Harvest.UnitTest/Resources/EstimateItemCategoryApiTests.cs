using Harvest.UnitTest.Support;

namespace Harvest.UnitTest.Resources;

public class EstimateItemCategoryApiTests
{
	private const string CategoryJson = """
		{"id":1378704,"name":"Product","created_at":"2017-06-26T20:41:00Z","updated_at":"2017-06-26T20:42:00Z"}
		""";

	[Fact]
	public async Task ListAsync_NoOptions_SendsNoQuery()
	{
		var call = await TestClient.SendAsync((c, ct) => c.EstimateItemCategories.ListAsync(null, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/estimate_item_categories");
	}

	[Fact]
	public async Task ListAsync_AllOptions_SendsEveryParameter()
	{
		var call = await TestClient.SendAsync((c, ct) => c.EstimateItemCategories.ListAsync(new ListOptions
		{
			UpdatedSince = new DateTimeOffset(2017, 6, 26, 21, 52, 18, TimeSpan.Zero),
			Cursor = "abc",
			Page = 2,
			PerPage = 50
		}, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/estimate_item_categories",
			"?updated_since=2017-06-26T21%3A52%3A18Z&cursor=abc&page=2&per_page=50");
	}

	[Fact]
	public async Task ListAsync_MapsTheCategoriesAndPaging()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.EstimateItemCategories.ListAsync(null, ct), $$$"""
			{"estimate_item_categories":[{{{CategoryJson}}},{"id":1378703,"name":"Service","created_at":"2017-06-26T20:41:00Z","updated_at":"2017-06-26T20:41:00Z"}],
			"per_page":2000,"total_pages":1,"total_entries":2,"next_page":null,"previous_page":null,"page":1,
			"links":{"first":"https://api.harvestapp.com/v2/estimate_item_categories?page=1&per_page=2000","next":null,"previous":null,"last":"https://api.harvestapp.com/v2/estimate_item_categories?page=1&per_page=2000"}}
			""");

		page.EstimateItemCategories.Should().HaveCount(2);
		ShouldBeTheCategory(page.EstimateItemCategories[0]);
		page.EstimateItemCategories[1].Name.Should().Be("Service");
		page.PerPage.Should().Be(2000);
		page.TotalEntries.Should().Be(2);
		page.Page.Should().Be(1);
	}

	[Fact]
	public async Task GetAsync_SendsGetAndMapsTheCategory()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.OK, CategoryJson);
		using var client = TestClient.Create(stub);

		var category = await client.EstimateItemCategories.GetAsync(1378704, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Subject.ShouldBe(HttpMethod.Get, "/v2/estimate_item_categories/1378704");
		ShouldBeTheCategory(category);
	}

	[Fact]
	public async Task CreateAsync_SendsName()
	{
		var call = await TestClient.SendAsync((c, ct) => c.EstimateItemCategories.CreateAsync(new EstimateItemCategoryCreationDto { Name = "Hosting" }, ct), CategoryJson);

		call.ShouldBe(HttpMethod.Post, "/v2/estimate_item_categories", body: """{"name":"Hosting"}""");
	}

	[Fact]
	public async Task UpdateAsync_AllFields_SendsPatch()
	{
		var call = await TestClient.SendAsync((c, ct) => c.EstimateItemCategories.UpdateAsync(1379244, new EstimateItemCategoryPatchDto { Name = "Transportation" }, ct), CategoryJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/estimate_item_categories/1379244", body: """{"name":"Transportation"}""");
	}

	[Fact]
	public async Task UpdateAsync_EmptyPatch_SendsEmptyObject()
	{
		var call = await TestClient.SendAsync((c, ct) => c.EstimateItemCategories.UpdateAsync(1379244, new EstimateItemCategoryPatchDto(), ct), CategoryJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/estimate_item_categories/1379244", body: "{}");
	}

	[Fact]
	public async Task DeleteAsync_SendsDelete()
	{
		var call = await TestClient.SendAsync((c, ct) => c.EstimateItemCategories.DeleteAsync(1379244, ct), "");

		call.ShouldBe(HttpMethod.Delete, "/v2/estimate_item_categories/1379244");
	}

	private static void ShouldBeTheCategory(EstimateItemCategory category)
	{
		category.Id.Should().Be(1378704);
		category.Name.Should().Be("Product");
		category.CreatedAt.Should().Be(new DateTime(2017, 6, 26, 20, 41, 0, DateTimeKind.Utc));
		category.UpdatedAt.Should().Be(new DateTime(2017, 6, 26, 20, 42, 0, DateTimeKind.Utc));
	}
}