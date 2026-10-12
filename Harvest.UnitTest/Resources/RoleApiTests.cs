using Harvest.UnitTest.Support;

namespace Harvest.UnitTest.Resources;

public class RoleApiTests
{
	private const string RoleJson = """
		{"id":617630,"name":"Sales","created_at":"2020-04-16T16:59:59Z","updated_at":"2020-04-16T16:59:59Z",
		"user_ids":[2084359,3122373,3122374]}
		""";

	[Fact]
	public async Task ListAsync_NullOptions_SendsNoQuery()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Roles.ListAsync(null, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/roles");
	}

	[Fact]
	public async Task ListAsync_SendsPagingOptions()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Roles.ListAsync(new ListOptions { Cursor = "abc", Page = 2, PerPage = 100 }, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/roles", "?cursor=abc&page=2&per_page=100");
	}

	[Fact]
	public async Task ListAsync_ReadsTheRolesKey()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.Roles.ListAsync(null, ct), $$$"""
			{"roles":[{"id":618100,"name":"Designer","created_at":"2020-04-17T10:09:41Z","updated_at":"2020-04-17T10:09:41Z","user_ids":[]},
			{{{RoleJson}}}],
			"per_page":2000,"total_pages":1,"total_entries":2,"next_page":null,"previous_page":null,"page":1,
			"links":{"first":"https://api.harvestapp.com/v2/roles?page=1&per_page=2000","next":null,"previous":null,
			"last":"https://api.harvestapp.com/v2/roles?page=1&per_page=2000"}}
			""");

		page.Roles.Should().HaveCount(2);
		page.Roles[0].Name.Should().Be("Designer");
		page.Roles[0].UserIds.Should().BeEmpty();
		ShouldBeTheRole(page.Roles[1]);
		page.TotalEntries.Should().Be(2);
	}

	[Fact]
	public async Task GetAsync_SendsGetAndMapsTheRole()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.OK, RoleJson);
		using var client = TestClient.Create(stub);

		var role = await client.Roles.GetAsync(617630, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Subject.ShouldBe(HttpMethod.Get, "/v2/roles/617630");
		ShouldBeTheRole(role);
	}

	[Fact]
	public async Task GetAsync_ToleratesMissingFields()
	{
		var role = await TestClient.ReadAsync((c, ct) => c.Roles.GetAsync(1, ct), """{"id":1}""");

		role.Name.Should().BeNull();
		role.UserIds.Should().BeNull();
	}

	[Fact]
	public async Task CreateAsync_RequiredOnly_SendsName()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Roles.CreateAsync(new RoleCreationDto { Name = "Marketing" }, ct), RoleJson);

		call.ShouldBe(HttpMethod.Post, "/v2/roles", body: """{"name":"Marketing"}""");
	}

	[Fact]
	public async Task CreateAsync_AllFields_SendsEveryField()
	{
		var call = await TestClient.SendAsync(
			(c, ct) => c.Roles.CreateAsync(new RoleCreationDto { Name = "Marketing", UserIds = [3122374, 3122373, 2084359] }, ct),
			RoleJson);

		call.ShouldBe(HttpMethod.Post, "/v2/roles", body: """{"name":"Marketing","user_ids":[3122374,3122373,2084359]}""");
	}

	[Fact]
	public async Task UpdateAsync_AllFields_SendsPatch()
	{
		var call = await TestClient.SendAsync(
			(c, ct) => c.Roles.UpdateAsync(618099, new RolePatchDto { Name = "HR", UserIds = [2084359] }, ct),
			RoleJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/roles/618099", body: """{"name":"HR","user_ids":[2084359]}""");
	}

	[Fact]
	public async Task UpdateAsync_EmptyUserIds_SendsEmptyArrayToUnassignEveryone()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Roles.UpdateAsync(618099, new RolePatchDto { UserIds = [] }, ct), RoleJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/roles/618099", body: """{"user_ids":[]}""");
	}

	[Fact]
	public async Task UpdateAsync_NoFields_SendsEmptyObject()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Roles.UpdateAsync(618099, new RolePatchDto(), ct), RoleJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/roles/618099", body: "{}");
	}

	[Fact]
	public async Task DeleteAsync_SendsDelete()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Roles.DeleteAsync(617631, ct), "");

		call.ShouldBe(HttpMethod.Delete, "/v2/roles/617631");
	}

	private static void ShouldBeTheRole(Role role)
	{
		role.Id.Should().Be(617630);
		role.Name.Should().Be("Sales");
		role.UserIds.Should().Equal(2084359, 3122373, 3122374);
		role.CreatedAt.Should().Be(new DateTime(2020, 4, 16, 16, 59, 59, DateTimeKind.Utc));
		role.UpdatedAt.Should().Be(new DateTime(2020, 4, 16, 16, 59, 59, DateTimeKind.Utc));
	}
}