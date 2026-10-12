using Harvest.UnitTest.Support;

namespace Harvest.UnitTest.Resources;

public class AccountApiTests
{
	private const string AccountsJson = """
		{"user":{"id":1,"first_name":"Albert","last_name":"Llop","email":"albert@example.com"},
		"accounts":[{"id":10254,"name":"Sterling Cooper Advertising Agency","product":"harvest"},
		{"id":88888,"name":"Iridesco","product":"forecast"},
		{"id":88903,"name":"Sterling Cooper Advertising Agency","product":"forecast"}]}
		""";

	[Fact]
	public async Task ListAsync_SendsGetToHarvestIdWithoutTheAccountHeader()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Accounts.ListAsync(ct), AccountsJson);

		call.ShouldBe(HttpMethod.Get, "/api/v2/accounts");
		call.Uri.GetLeftPart(UriPartial.Authority).Should().Be("https://id.getharvest.com");
		HarvestClient.IdBaseAddress.Should().Be(new Uri("https://id.getharvest.com"));
		call.Headers.Contains("Harvest-Account-Id").Should().BeFalse();
		call.Headers.Authorization!.Scheme.Should().Be("Bearer");
		call.Headers.Authorization.Parameter.Should().Be(TestClient.AccessToken);
		string.Join(" ", call.Headers.GetValues("User-Agent")).Should().Be(HarvestClient.DefaultUserAgent);
		call.Headers.Accept.Should().ContainSingle().Which.MediaType.Should().Be("application/json");
	}

	[Fact]
	public async Task ListAsync_MapsTheUserAndAccounts()
	{
		var response = await TestClient.ReadAsync((c, ct) => c.Accounts.ListAsync(ct), AccountsJson);

		response.User!.Id.Should().Be(1);
		response.User.FirstName.Should().Be("Albert");
		response.User.LastName.Should().Be("Llop");
		response.User.Email.Should().Be("albert@example.com");
		response.Accounts.Should().HaveCount(3);
		response.Accounts[0].Id.Should().Be(10254);
		response.Accounts[0].Name.Should().Be("Sterling Cooper Advertising Agency");
		response.Accounts[0].Product.Should().Be(AccountProduct.Harvest);
		response.Accounts[1].Id.Should().Be(88888);
		response.Accounts[1].Name.Should().Be("Iridesco");
		response.Accounts[1].Product.Should().Be(AccountProduct.Forecast);
	}

	[Fact]
	public async Task ListAsync_ToleratesMissingFields()
	{
		var response = await TestClient.ReadAsync((c, ct) => c.Accounts.ListAsync(ct), "{}");

		response.User.Should().BeNull();
		response.Accounts.Should().BeEmpty();
	}

	[Fact]
	public async Task ListAsync_SharesTheRetryPipeline()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.ServiceUnavailable).Enqueue(HttpStatusCode.OK, AccountsJson);
		var options = new HarvestClientOptions
		{
			AccountId = TestClient.AccountId,
			AccessToken = TestClient.AccessToken,
			MaxRetries = 1,
			RetryBaseDelay = TimeSpan.Zero
		};
		using var client = TestClient.Create(stub, options);

		var response = await client.Accounts.ListAsync(TestClient.Ct);

		stub.Calls.Should().HaveCount(2);
		response.Accounts.Should().HaveCount(3);
	}

	[Fact]
	public async Task ApiRequests_StillCarryTheAccountHeaderAfterAnAccountsRequest()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.OK, AccountsJson).Enqueue(HttpStatusCode.OK);
		using var client = TestClient.Create(stub);

		_ = await client.Accounts.ListAsync(TestClient.Ct);
		_ = await client.Companies.GetAsync(TestClient.Ct);

		stub.Calls[1].Uri.GetLeftPart(UriPartial.Authority).Should().Be("https://api.harvestapp.com");
		stub.Calls[1].Headers.GetValues("Harvest-Account-Id").Should().Equal("12345");
	}

	[Fact]
	public async Task Dispose_DisposesTheHarvestIdClient()
	{
		var client = TestClient.Create(new StubHandler());

		client.Dispose();

		var act = () => client.Accounts.ListAsync(TestClient.Ct);
		await act.Should().ThrowAsync<ObjectDisposedException>();
	}
}