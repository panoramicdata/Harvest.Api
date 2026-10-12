using Harvest.UnitTest.Support;

namespace Harvest.UnitTest.Resources;

public class UserApiTests
{
	private const string UserJson = """
		{"id":3230547,"first_name":"Jim","last_name":"Allen","email":"jim@example.com","telephone":"555","timezone":"Mountain Time (US & Canada)",
		"has_access_to_all_future_projects":false,"is_contractor":true,"is_active":true,"saml_exempt":true,"created_at":"2020-05-01T22:34:41Z",
		"updated_at":"2020-05-01T22:34:52Z","weekly_capacity":126000,"default_hourly_rate":100.0,"cost_rate":50.0,"roles":["Developer"],
		"access_roles":["member"],"permissions_profile":"Member","avatar_url":"https://cache.harvestapp.com/avatar.png"}
		""";

	[Fact]
	public async Task ListAsync_SendsGetWithOptions()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Users.ListAsync(new ListOptions { IsActive = true, Page = 2 }, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/users", "?is_active=true&page=2");
	}

	[Fact]
	public async Task ListAsync_MapsTheUsers()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.Users.ListAsync(null, ct), $$$"""{"users":[{{{UserJson}}}]}""");

		ShouldBeTheUser(page.Users.Should().ContainSingle().Subject);
	}

	[Fact]
	public async Task GetAsync_SendsGetAndMapsTheUser()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.OK, UserJson);
		using var client = TestClient.Create(stub);

		var user = await client.Users.GetAsync(3230547, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Subject.ShouldBe(HttpMethod.Get, "/v2/users/3230547");
		ShouldBeTheUser(user);
	}

	[Fact]
	public async Task GetMeAsync_SendsGetToMe()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Users.GetMeAsync(ct), UserJson);

		call.ShouldBe(HttpMethod.Get, "/v2/users/me");
	}

	[Fact]
	public async Task CreateAsync_RequiredOnly_LeavesDefaultsToHarvest()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Users.CreateAsync(new UserCreationDto
		{
			FirstName = "Jim",
			LastName = "Allen",
			Email = "jim@example.com"
		}, ct), UserJson);

		call.ShouldBe(HttpMethod.Post, "/v2/users", body: """{"first_name":"Jim","last_name":"Allen","email":"jim@example.com"}""");
	}

	[Fact]
	public async Task CreateAsync_AllFields_SendsEveryField()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Users.CreateAsync(new UserCreationDto
		{
			FirstName = "Jim",
			LastName = "Allen",
			Email = "jim@example.com",
			Timezone = "London",
			HasAccessToAllFutureProjects = true,
			IsContractor = false,
			IsActive = true,
			SamlExempt = false,
			WeeklyCapacity = 126000,
			DefaultHourlyRate = 120m,
			CostRate = 60m,
			Roles = ["Developer"],
			AccessRoles = ["manager", "project_creator"],
			PermissionsProfile = "Manager"
		}, ct), UserJson);

		call.ShouldBe(HttpMethod.Post, "/v2/users", body: """
			{"first_name":"Jim","last_name":"Allen","email":"jim@example.com","timezone":"London","has_access_to_all_future_projects":true,"is_contractor":false,"is_active":true,"saml_exempt":false,"weekly_capacity":126000,"default_hourly_rate":120,"cost_rate":60,"roles":["Developer"],"access_roles":["manager","project_creator"],"permissions_profile":"Manager"}
			""");
	}

	[Fact]
	public async Task UpdateAsync_AllFields_SendsPatch()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Users.UpdateAsync(3230547, new UserPatchDto
		{
			FirstName = "J",
			LastName = "A",
			Email = "j@example.com",
			Timezone = "UTC",
			HasAccessToAllFutureProjects = false,
			IsContractor = true,
			IsActive = false,
			SamlExempt = true,
			WeeklyCapacity = 1,
			DefaultHourlyRate = 2m,
			CostRate = 3m,
			Roles = [],
			AccessRoles = ["member"],
			PermissionsProfile = "Member"
		}, ct), UserJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/users/3230547", body: """
			{"first_name":"J","last_name":"A","email":"j@example.com","timezone":"UTC","has_access_to_all_future_projects":false,"is_contractor":true,"is_active":false,"saml_exempt":true,"weekly_capacity":1,"default_hourly_rate":2,"cost_rate":3,"roles":[],"access_roles":["member"],"permissions_profile":"Member"}
			""");
	}

	[Fact]
	public async Task DeleteAsync_SendsDelete()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Users.DeleteAsync(3230547, ct), "");

		call.ShouldBe(HttpMethod.Delete, "/v2/users/3230547");
	}

	private static void ShouldBeTheUser(User user)
	{
		user.Id.Should().Be(3230547);
		user.FirstName.Should().Be("Jim");
		user.LastName.Should().Be("Allen");
		user.Email.Should().Be("jim@example.com");
		user.Telephone.Should().Be("555");
		user.Timezone.Should().Be("Mountain Time (US & Canada)");
		user.HasAccessToAllFutureProjects.Should().BeFalse();
		user.IsContractor.Should().BeTrue();
		user.IsActive.Should().BeTrue();
		user.SamlExempt.Should().BeTrue();
		user.WeeklyCapacity.Should().Be(126000);
		user.DefaultHourlyRate.Should().Be(100m);
		user.CostRate.Should().Be(50m);
		user.Roles.Should().Equal("Developer");
		user.AccessRoles.Should().Equal("member");
		user.PermissionsProfile.Should().Be("Member");
		user.AvatarUrl.Should().Be("https://cache.harvestapp.com/avatar.png");
		user.CreatedAt.Should().Be(new DateTime(2020, 5, 1, 22, 34, 41, DateTimeKind.Utc));
		user.UpdatedAt.Should().Be(new DateTime(2020, 5, 1, 22, 34, 52, DateTimeKind.Utc));
	}
}