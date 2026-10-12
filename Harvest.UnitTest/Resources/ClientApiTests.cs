using Harvest.UnitTest.Support;

namespace Harvest.UnitTest.Resources;

public class ClientApiTests
{
	private const string ClientJson = """
		{"id":5735776,"name":"123 Industries","is_active":true,"address":"123 Main St.\r\nAnytown, LA 71223",
		"statement_key":"0a39d3e33c8058cf7c3f8097d854c64e","created_at":"2017-06-26T21:02:12Z","updated_at":"2017-06-26T21:34:11Z","currency":"EUR"}
		""";

	[Fact]
	public async Task ListAsync_NoOptions_SendsNoQuery()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Clients.ListAsync(null, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/clients");
	}

	[Fact]
	public async Task ListAsync_AllOptions_SendsEveryParameter()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Clients.ListAsync(new ListOptions
		{
			IsActive = true,
			UpdatedSince = new DateTimeOffset(2017, 6, 26, 21, 52, 18, TimeSpan.Zero),
			Cursor = "abc",
			Page = 2,
			PerPage = 50
		}, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/clients", "?is_active=true&updated_since=2017-06-26T21%3A52%3A18Z&cursor=abc&page=2&per_page=50");
	}

	[Fact]
	public async Task ListAsync_MapsTheClientsAndPaging()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.Clients.ListAsync(null, ct), $$$"""
			{"clients":[{{{ClientJson}}}],"per_page":2000,"total_pages":1,"total_entries":1,"next_page":null,"previous_page":null,"page":1,
			"links":{"first":"https://api.harvestapp.com/v2/clients?page=1&per_page=2000","next":null,"previous":null,"last":"https://api.harvestapp.com/v2/clients?page=1&per_page=2000"}}
			""");

		page.PerPage.Should().Be(2000);
		page.TotalPages.Should().Be(1);
		page.TotalEntries.Should().Be(1);
		page.Page.Should().Be(1);
		page.NextPage.Should().BeNull();
		page.Links!.First.Should().Be("https://api.harvestapp.com/v2/clients?page=1&per_page=2000");
		ShouldBeTheClient(page.Clients.Should().ContainSingle().Subject);
	}

	[Fact]
	public async Task GetAsync_SendsGetAndMapsTheClient()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.OK, ClientJson);
		using var client = TestClient.Create(stub);

		var result = await client.Clients.GetAsync(5735776, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Subject.ShouldBe(HttpMethod.Get, "/v2/clients/5735776");
		ShouldBeTheClient(result);
	}

	[Fact]
	public async Task CreateAsync_RequiredOnly_OmitsUnsetFields()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Clients.CreateAsync(new ClientCreationDto { Name = "Acme" }, ct), ClientJson);

		call.ShouldBe(HttpMethod.Post, "/v2/clients", body: """{"name":"Acme"}""");
	}

	[Fact]
	public async Task CreateAsync_AllFields_SendsSnakeCaseBody()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Clients.CreateAsync(new ClientCreationDto
		{
			Name = "Acme",
			IsActive = false,
			Address = "1 Road",
			Currency = "GBP"
		}, ct), ClientJson);

		call.ShouldBe(HttpMethod.Post, "/v2/clients", body: """{"name":"Acme","is_active":false,"address":"1 Road","currency":"GBP"}""");
	}

	[Fact]
	public async Task UpdateAsync_SendsPatchWithOnlySetFields()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Clients.UpdateAsync(7, new ClientPatchDto
		{
			Name = "New",
			IsActive = true,
			Address = "2 Road",
			Currency = "USD"
		}, ct), ClientJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/clients/7", body: """{"name":"New","is_active":true,"address":"2 Road","currency":"USD"}""");
	}

	[Fact]
	public async Task UpdateAsync_EmptyPatch_SendsEmptyObject()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Clients.UpdateAsync(7, new ClientPatchDto(), ct), ClientJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/clients/7", body: "{}");
	}

	[Fact]
	public async Task DeleteAsync_SendsDelete()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Clients.DeleteAsync(7, ct), "");

		call.ShouldBe(HttpMethod.Delete, "/v2/clients/7");
	}

	private static void ShouldBeTheClient(Client client)
	{
		client.Id.Should().Be(5735776);
		client.Name.Should().Be("123 Industries");
		client.IsActive.Should().BeTrue();
		client.Address.Should().Be("123 Main St.\r\nAnytown, LA 71223");
		client.StatementKey.Should().Be("0a39d3e33c8058cf7c3f8097d854c64e");
		client.Currency.Should().Be("EUR");
		client.CreatedAt.Should().Be(new DateTime(2017, 6, 26, 21, 2, 12, DateTimeKind.Utc));
		client.UpdatedAt.Should().Be(new DateTime(2017, 6, 26, 21, 34, 11, DateTimeKind.Utc));
	}
}