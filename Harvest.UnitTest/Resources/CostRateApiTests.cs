using Harvest.UnitTest.Support;

namespace Harvest.UnitTest.Resources;

public class CostRateApiTests
{
	private const string RateJson = """
		{"id":825301,"amount":9.25,"start_date":"2019-01-01","end_date":"2019-05-31",
		"created_at":"2020-05-01T13:19:09Z","updated_at":"2020-05-01T13:19:17Z"}
		""";

	[Fact]
	public async Task ListAsync_NullOptions_SendsNoQuery()
	{
		var call = await TestClient.SendAsync((c, ct) => c.CostRates.ListAsync(3226125, null, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/users/3226125/cost_rates");
	}

	[Fact]
	public async Task ListAsync_SendsPagingOptions()
	{
		var call = await TestClient.SendAsync(
			(c, ct) => c.CostRates.ListAsync(3226125, new ListOptions { Cursor = "abc", Page = 2, PerPage = 100 }, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/users/3226125/cost_rates", "?cursor=abc&page=2&per_page=100");
	}

	[Fact]
	public async Task ListAsync_ReadsTheCostRatesKey()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.CostRates.ListAsync(3226125, null, ct), $$$"""
			{"cost_rates":[{{{RateJson}}},
			{"id":825304,"amount":15.25,"start_date":"2020-05-01","end_date":null,"created_at":"2020-05-01T13:19:31Z","updated_at":"2020-05-01T13:19:31Z"}],
			"per_page":2000,"total_pages":1,"total_entries":2,"next_page":null,"previous_page":null,"page":1,
			"links":{"first":"https://api.harvestapp.com/v2/users/3226125/cost_rates?page=1&per_page=2000","next":null,"previous":null,
			"last":"https://api.harvestapp.com/v2/users/3226125/cost_rates?page=1&per_page=2000"}}
			""");

		page.CostRates.Should().HaveCount(2);
		ShouldBeTheRate(page.CostRates[0]);
		page.CostRates[1].Amount.Should().Be(15.25m);
		page.CostRates[1].EndDate.Should().BeNull();
		page.TotalEntries.Should().Be(2);
	}

	[Fact]
	public async Task GetAsync_SendsGetAndMapsTheRate()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.OK, RateJson);
		using var client = TestClient.Create(stub);

		var rate = await client.CostRates.GetAsync(3226125, 825301, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Subject.ShouldBe(HttpMethod.Get, "/v2/users/3226125/cost_rates/825301");
		ShouldBeTheRate(rate);
	}

	[Fact]
	public async Task GetAsync_ToleratesMissingFields()
	{
		var rate = await TestClient.ReadAsync((c, ct) => c.CostRates.GetAsync(1, 2, ct), """{"id":2}""");

		rate.Amount.Should().BeNull();
		rate.StartDate.Should().BeNull();
		rate.EndDate.Should().BeNull();
	}

	[Fact]
	public async Task CreateAsync_RequiredOnly_SendsAmount()
	{
		var call = await TestClient.SendAsync(
			(c, ct) => c.CostRates.CreateAsync(3226125, new CostRateCreationDto { Amount = 5m }, ct),
			RateJson);

		call.ShouldBe(HttpMethod.Post, "/v2/users/3226125/cost_rates", body: """{"amount":5}""");
	}

	[Fact]
	public async Task CreateAsync_AllFields_SendsEveryField()
	{
		var call = await TestClient.SendAsync(
			(c, ct) => c.CostRates.CreateAsync(3226125, new CostRateCreationDto { Amount = 5.5m, StartDate = new DateOnly(2020, 5, 5) }, ct),
			RateJson);

		call.ShouldBe(HttpMethod.Post, "/v2/users/3226125/cost_rates", body: """{"amount":5.5,"start_date":"2020-05-05"}""");
	}

	private static void ShouldBeTheRate(CostRate rate)
	{
		rate.Id.Should().Be(825301);
		rate.Amount.Should().Be(9.25m);
		rate.StartDate.Should().Be(new DateOnly(2019, 1, 1));
		rate.EndDate.Should().Be(new DateOnly(2019, 5, 31));
		rate.CreatedAt.Should().Be(new DateTime(2020, 5, 1, 13, 19, 9, DateTimeKind.Utc));
		rate.UpdatedAt.Should().Be(new DateTime(2020, 5, 1, 13, 19, 17, DateTimeKind.Utc));
	}
}