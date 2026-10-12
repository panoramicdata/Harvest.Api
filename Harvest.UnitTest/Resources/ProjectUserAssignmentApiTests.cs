using Harvest.UnitTest.Support;

namespace Harvest.UnitTest.Resources;

public class ProjectUserAssignmentApiTests
{
	private const string AssignmentJson = """
		{"id":125068554,"is_project_manager":true,"is_active":true,"use_default_rates":true,"budget":null,
		"created_at":"2017-06-26T22:32:52Z","updated_at":"2017-06-26T22:32:52Z","hourly_rate":100.0,
		"project":{"id":14308069,"name":"Online Store - Phase 1","code":"OS1"},"user":{"id":1782959,"name":"Kim Allen"}}
		""";

	private const string CreatedJson = """
		{"id":125068758,"is_project_manager":false,"is_active":true,"use_default_rates":false,"budget":null,
		"created_at":"2017-06-26T22:36:01Z","updated_at":"2017-06-26T22:36:01Z","hourly_rate":75.5,
		"project":{"id":14308069,"name":"Online Store - Phase 1","code":"OS1"},"user":{"id":1782974,"name":"Jim Allen"}}
		""";

	private static readonly UserAssignmentListOptions EveryOption = new()
	{
		UserId = 1782959,
		IsActive = true,
		UpdatedSince = new DateTimeOffset(2017, 6, 26, 21, 52, 18, TimeSpan.Zero),
		Cursor = "abc",
		Page = 2,
		PerPage = 100
	};

	private const string EveryOptionQuery =
		"?user_id=1782959&is_active=true&updated_since=2017-06-26T21%3A52%3A18Z&cursor=abc&page=2&per_page=100";

	[Fact]
	public async Task ListAsync_AllProjects_NullOptions_SendsNoQuery()
	{
		var call = await TestClient.SendAsync((c, ct) => c.ProjectUserAssignments.ListAsync(null, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/user_assignments");
	}

	[Fact]
	public async Task ListAsync_AllProjects_SendsEveryOption()
	{
		var call = await TestClient.SendAsync((c, ct) => c.ProjectUserAssignments.ListAsync(EveryOption, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/user_assignments", EveryOptionQuery);
	}

	[Fact]
	public async Task ListAsync_AllProjects_ReadsTheUserAssignmentsKey()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.ProjectUserAssignments.ListAsync(null, ct), $$$"""
			{"user_assignments":[{{{AssignmentJson}}},
			{"id":125066109,"is_project_manager":true,"is_active":true,"use_default_rates":false,"budget":null,
			"created_at":"2017-06-26T21:52:18Z","updated_at":"2017-06-26T21:52:18Z","hourly_rate":100.0,
			"project":{"id":14308069,"name":"Online Store - Phase 1","code":"OS1"},"user":{"id":1782884,"name":"Jeremy Israelsen"}}],
			"per_page":2000,"total_pages":1,"total_entries":2,"next_page":null,"previous_page":null,"page":1,
			"links":{"first":"https://api.harvestapp.com/v2/user_assignments?page=1&per_page=2000","next":null,"previous":null,
			"last":"https://api.harvestapp.com/v2/user_assignments?page=1&per_page=2000"}}
			""");

		page.UserAssignments.Should().HaveCount(2);
		ShouldBeTheAssignment(page.UserAssignments[0]);
		page.UserAssignments[1].User!.Name.Should().Be("Jeremy Israelsen");
		page.UserAssignments[1].UseDefaultRates.Should().BeFalse();
		page.TotalEntries.Should().Be(2);
	}

	[Fact]
	public async Task ListAsync_ForProject_NullOptions_SendsNoQuery()
	{
		var call = await TestClient.SendAsync((c, ct) => c.ProjectUserAssignments.ListAsync(14308069, null, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/projects/14308069/user_assignments");
	}

	[Fact]
	public async Task ListAsync_ForProject_SendsEveryOption()
	{
		var call = await TestClient.SendAsync((c, ct) => c.ProjectUserAssignments.ListAsync(14308069, EveryOption, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/projects/14308069/user_assignments", EveryOptionQuery);
	}

	[Fact]
	public async Task ListAsync_ForProject_MapsTheAssignments()
	{
		var page = await TestClient.ReadAsync(
			(c, ct) => c.ProjectUserAssignments.ListAsync(14308069, null, ct),
			$$$"""{"user_assignments":[{{{AssignmentJson}}}],"per_page":2000,"total_pages":1,"total_entries":1,"page":1}""");

		ShouldBeTheAssignment(page.UserAssignments.Should().ContainSingle().Subject);
	}

	[Fact]
	public async Task GetAsync_SendsGetAndMapsTheAssignment()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.OK, AssignmentJson);
		using var client = TestClient.Create(stub);

		var assignment = await client.ProjectUserAssignments.GetAsync(14308069, 125068554, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Subject.ShouldBe(HttpMethod.Get, "/v2/projects/14308069/user_assignments/125068554");
		ShouldBeTheAssignment(assignment);
	}

	[Fact]
	public async Task CreateAsync_RequiredOnly_SendsUserId()
	{
		var call = await TestClient.SendAsync(
			(c, ct) => c.ProjectUserAssignments.CreateAsync(14308069, new UserAssignmentCreationDto { UserId = 1782974 }, ct),
			CreatedJson);

		call.ShouldBe(HttpMethod.Post, "/v2/projects/14308069/user_assignments", body: """{"user_id":1782974}""");
	}

	[Fact]
	public async Task CreateAsync_AllFields_SendsEveryFieldAndMapsTheResponse()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.Created, CreatedJson);
		using var client = TestClient.Create(stub);

		var assignment = await client.ProjectUserAssignments.CreateAsync(14308069, new UserAssignmentCreationDto
		{
			UserId = 1782974,
			IsActive = true,
			IsProjectManager = false,
			UseDefaultRates = false,
			HourlyRate = 75.5m,
			Budget = 120m
		}, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Subject.ShouldBe(HttpMethod.Post, "/v2/projects/14308069/user_assignments",
			body: """{"user_id":1782974,"is_active":true,"is_project_manager":false,"use_default_rates":false,"hourly_rate":75.5,"budget":120}""");
		assignment.Id.Should().Be(125068758);
		assignment.IsProjectManager.Should().BeFalse();
		assignment.UseDefaultRates.Should().BeFalse();
		assignment.HourlyRate.Should().Be(75.5m);
		assignment.User!.Id.Should().Be(1782974);
	}

	[Fact]
	public async Task UpdateAsync_OneField_SendsOnlyThatField()
	{
		var call = await TestClient.SendAsync(
			(c, ct) => c.ProjectUserAssignments.UpdateAsync(14308069, 125068758, new UserAssignmentPatchDto { Budget = 120m }, ct),
			CreatedJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/projects/14308069/user_assignments/125068758", body: """{"budget":120}""");
	}

	[Fact]
	public async Task UpdateAsync_AllFields_SendsEveryField()
	{
		var call = await TestClient.SendAsync((c, ct) => c.ProjectUserAssignments.UpdateAsync(14308069, 125068758, new UserAssignmentPatchDto
		{
			IsActive = false,
			IsProjectManager = true,
			UseDefaultRates = true,
			HourlyRate = 1m,
			Budget = 2m
		}, ct), CreatedJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/projects/14308069/user_assignments/125068758",
			body: """{"is_active":false,"is_project_manager":true,"use_default_rates":true,"hourly_rate":1,"budget":2}""");
	}

	[Fact]
	public async Task DeleteAsync_SendsDelete()
	{
		var call = await TestClient.SendAsync((c, ct) => c.ProjectUserAssignments.DeleteAsync(14308069, 125068758, ct), "");

		call.ShouldBe(HttpMethod.Delete, "/v2/projects/14308069/user_assignments/125068758");
	}

	private static void ShouldBeTheAssignment(UserAssignment assignment)
	{
		assignment.Id.Should().Be(125068554);
		assignment.IsProjectManager.Should().BeTrue();
		assignment.IsActive.Should().BeTrue();
		assignment.UseDefaultRates.Should().BeTrue();
		assignment.Budget.Should().BeNull();
		assignment.HourlyRate.Should().Be(100m);
		assignment.CreatedAt.Should().Be(new DateTime(2017, 6, 26, 22, 32, 52, DateTimeKind.Utc));
		assignment.UpdatedAt.Should().Be(new DateTime(2017, 6, 26, 22, 32, 52, DateTimeKind.Utc));
		assignment.Project!.Id.Should().Be(14308069);
		assignment.Project.Name.Should().Be("Online Store - Phase 1");
		assignment.Project.Code.Should().Be("OS1");
		assignment.User!.Id.Should().Be(1782959);
		assignment.User.Name.Should().Be("Kim Allen");
	}
}