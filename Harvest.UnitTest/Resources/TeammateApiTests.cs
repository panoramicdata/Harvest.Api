using Harvest.UnitTest.Support;

namespace Harvest.UnitTest.Resources;

public class TeammateApiTests
{
	private const string ListJson = """
		{"teammates":[{"id":3230547,"first_name":"Jim","last_name":"Allen","email":"jimallen@example.com"},
		{"id":1782884,"first_name":"Bob","last_name":"Powell","email":"bobpowell@example.com"}],
		"per_page":100,"total_pages":1,"total_entries":2,"next_page":null,"previous_page":null,"page":1,
		"links":{"first":"https://api.harvestapp.com/v2/users/1782959/teammates?page=1&per_page=100","next":null,"previous":null,
		"last":"https://api.harvestapp.com/v2/users/1782959/teammates?page=1&per_page=100"}}
		""";

	private const string UpdateJson = """
		{"teammates":[{"id":3230547,"first_name":"Jim","last_name":"Allen","email":"jimallen@example.com"},
		{"id":3230575,"first_name":"Gary","last_name":"Brookes","email":"gary@example.com"}]}
		""";

	[Fact]
	public async Task ListAsync_NullOptions_SendsNoQuery()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Teammates.ListAsync(1782959, null, ct), ListJson);

		call.ShouldBe(HttpMethod.Get, "/v2/users/1782959/teammates");
	}

	[Fact]
	public async Task ListAsync_SendsPagingOptions()
	{
		var call = await TestClient.SendAsync(
			(c, ct) => c.Teammates.ListAsync(1782959, new ListOptions { Cursor = "abc", Page = 2, PerPage = 100 }, ct),
			ListJson);

		call.ShouldBe(HttpMethod.Get, "/v2/users/1782959/teammates", "?cursor=abc&page=2&per_page=100");
	}

	[Fact]
	public async Task ListAsync_MapsTheTeammates()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.Teammates.ListAsync(1782959, null, ct), ListJson);

		page.Teammates.Should().HaveCount(2);
		var jim = page.Teammates[0];
		jim.Id.Should().Be(3230547);
		jim.FirstName.Should().Be("Jim");
		jim.LastName.Should().Be("Allen");
		jim.Email.Should().Be("jimallen@example.com");
		page.Teammates[1].Id.Should().Be(1782884);
		page.PerPage.Should().Be(100);
		page.TotalEntries.Should().Be(2);
	}

	[Fact]
	public async Task UpdateAsync_SendsPatchWithTheFullList()
	{
		var call = await TestClient.SendAsync(
			(c, ct) => c.Teammates.UpdateAsync(1782959, new TeammatePatchDto { TeammateIds = [3230547, 3230575] }, ct),
			UpdateJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/users/1782959/teammates", body: """{"teammate_ids":[3230547,3230575]}""");
	}

	[Fact]
	public async Task UpdateAsync_EmptyList_SendsEmptyArrayToUnassignEveryone()
	{
		var call = await TestClient.SendAsync(
			(c, ct) => c.Teammates.UpdateAsync(1782959, new TeammatePatchDto { TeammateIds = [] }, ct),
			"""{"teammates":[]}""");

		call.ShouldBe(HttpMethod.Patch, "/v2/users/1782959/teammates", body: """{"teammate_ids":[]}""");
	}

	[Fact]
	public async Task UpdateAsync_MapsTheResponseWithoutPagingFields()
	{
		var result = await TestClient.ReadAsync(
			(c, ct) => c.Teammates.UpdateAsync(1782959, new TeammatePatchDto { TeammateIds = [3230547, 3230575] }, ct),
			UpdateJson);

		result.Teammates.Select(t => t.Id).Should().Equal(3230547, 3230575);
		result.Teammates[1].FirstName.Should().Be("Gary");
		result.Teammates[1].LastName.Should().Be("Brookes");
		result.Teammates[1].Email.Should().Be("gary@example.com");
		result.Links.Should().BeNull();
		result.Page.Should().BeNull();
	}
}