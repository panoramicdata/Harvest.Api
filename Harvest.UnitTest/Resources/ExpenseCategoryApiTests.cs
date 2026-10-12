using Harvest.UnitTest.Support;

namespace Harvest.UnitTest.Resources;

public class ExpenseCategoryApiTests
{
	private const string MileageJson = """
		{"id":4195930,"name":"Mileage","unit_name":"mile","unit_price":0.535,"is_active":true,
		"created_at":"2017-06-26T20:41:00Z","updated_at":"2017-06-26T20:41:00Z"}
		""";

	private const string LodgingJson = """
		{"id":4197501,"name":"Lodging","unit_name":null,"unit_price":null,"is_active":true,
		"created_at":"2017-06-27T15:01:32Z","updated_at":"2017-06-27T15:01:32Z"}
		""";

	[Fact]
	public async Task ListAsync_NoOptions_SendsNoQuery()
	{
		var call = await TestClient.SendAsync((c, ct) => c.ExpenseCategories.ListAsync(null, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/expense_categories");
	}

	[Fact]
	public async Task ListAsync_AllOptions_SendsEveryParameter()
	{
		var call = await TestClient.SendAsync((c, ct) => c.ExpenseCategories.ListAsync(new ListOptions
		{
			IsActive = true,
			UpdatedSince = new DateTimeOffset(2017, 6, 26, 21, 52, 18, TimeSpan.Zero),
			Cursor = "abc",
			Page = 2,
			PerPage = 100
		}, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/expense_categories",
			"?is_active=true&updated_since=2017-06-26T21%3A52%3A18Z&cursor=abc&page=2&per_page=100");
	}

	[Fact]
	public async Task ListAsync_MapsTheExpenseCategories()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.ExpenseCategories.ListAsync(null, ct), $$$"""
			{"expense_categories":[{{{LodgingJson}}},{{{MileageJson}}}],"per_page":2000,"total_pages":1,"total_entries":2,
			"next_page":null,"previous_page":null,"page":1,"links":{"first":"https://api.harvestapp.com/v2/expense_categories?page=1&per_page=2000",
			"next":null,"previous":null,"last":"https://api.harvestapp.com/v2/expense_categories?page=1&per_page=2000"}}
			""");

		page.ExpenseCategories.Should().HaveCount(2);
		page.TotalEntries.Should().Be(2);
		var lodging = page.ExpenseCategories[0];
		lodging.Id.Should().Be(4197501);
		lodging.Name.Should().Be("Lodging");
		lodging.UnitName.Should().BeNull();
		lodging.UnitPrice.Should().BeNull();
		lodging.IsActive.Should().BeTrue();
		ShouldBeMileage(page.ExpenseCategories[1]);
	}

	[Fact]
	public async Task GetAsync_SendsGetAndMapsTheExpenseCategory()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.OK, MileageJson);
		using var client = TestClient.Create(stub);

		var category = await client.ExpenseCategories.GetAsync(4195930, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Subject.ShouldBe(HttpMethod.Get, "/v2/expense_categories/4195930");
		ShouldBeMileage(category);
	}

	[Fact]
	public async Task CreateAsync_RequiredOnly_SendsName()
	{
		var call = await TestClient.SendAsync((c, ct) => c.ExpenseCategories.CreateAsync(new ExpenseCategoryCreationDto
		{
			Name = "Other"
		}, ct), LodgingJson);

		call.ShouldBe(HttpMethod.Post, "/v2/expense_categories", body: """{"name":"Other"}""");
	}

	[Fact]
	public async Task CreateAsync_AllFields_SendsEveryField()
	{
		var call = await TestClient.SendAsync((c, ct) => c.ExpenseCategories.CreateAsync(new ExpenseCategoryCreationDto
		{
			Name = "Mileage",
			UnitName = "mile",
			UnitPrice = 0.535m,
			IsActive = false
		}, ct), MileageJson);

		call.ShouldBe(HttpMethod.Post, "/v2/expense_categories",
			body: """{"name":"Mileage","unit_name":"mile","unit_price":0.535,"is_active":false}""");
	}

	[Fact]
	public async Task UpdateAsync_AllFields_SendsPatch()
	{
		var call = await TestClient.SendAsync((c, ct) => c.ExpenseCategories.UpdateAsync(4197514, new ExpenseCategoryPatchDto
		{
			Name = "Km",
			UnitName = "km",
			UnitPrice = 0.3m,
			IsActive = true
		}, ct), MileageJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/expense_categories/4197514",
			body: """{"name":"Km","unit_name":"km","unit_price":0.3,"is_active":true}""");
	}

	[Fact]
	public async Task UpdateAsync_EmptyPatch_SendsEmptyObject()
	{
		var call = await TestClient.SendAsync((c, ct) => c.ExpenseCategories.UpdateAsync(4197514, new ExpenseCategoryPatchDto(), ct), MileageJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/expense_categories/4197514", body: "{}");
	}

	[Fact]
	public async Task DeleteAsync_SendsDelete()
	{
		var call = await TestClient.SendAsync((c, ct) => c.ExpenseCategories.DeleteAsync(4197514, ct), "");

		call.ShouldBe(HttpMethod.Delete, "/v2/expense_categories/4197514");
	}

	private static void ShouldBeMileage(ExpenseCategory category)
	{
		category.Id.Should().Be(4195930);
		category.Name.Should().Be("Mileage");
		category.UnitName.Should().Be("mile");
		category.UnitPrice.Should().Be(0.535m);
		category.IsActive.Should().BeTrue();
		category.CreatedAt.Should().Be(new DateTime(2017, 6, 26, 20, 41, 0, DateTimeKind.Utc));
		category.UpdatedAt.Should().Be(new DateTime(2017, 6, 26, 20, 41, 0, DateTimeKind.Utc));
	}
}