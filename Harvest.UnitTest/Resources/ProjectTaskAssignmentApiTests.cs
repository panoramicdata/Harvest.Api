using Harvest.UnitTest.Support;

namespace Harvest.UnitTest.Resources;

public class ProjectTaskAssignmentApiTests
{
	private const string AssignmentJson = """
		{"id":155505016,"billable":false,"is_active":true,"created_at":"2017-06-26T21:52:18Z","updated_at":"2017-06-26T21:54:06Z",
		"hourly_rate":100.0,"budget":null,"project":{"id":14308069,"name":"Online Store - Phase 1","code":"OS1"},
		"task":{"id":8083369,"name":"Research"}}
		""";

	private static readonly ListOptions _everyOption = new()
	{
		IsActive = true,
		UpdatedSince = new DateTimeOffset(2017, 6, 26, 21, 52, 18, TimeSpan.Zero),
		Cursor = "abc",
		Page = 2,
		PerPage = 100
	};

	private const string EveryOptionQuery = "?is_active=true&updated_since=2017-06-26T21%3A52%3A18Z&cursor=abc&page=2&per_page=100";

	[Fact]
	public async Task ListAsync_SendsGetWithOptions()
	{
		var call = await TestClient.SendAsync((c, ct) => c.ProjectTaskAssignments.ListAsync(14308069, new ListOptions { IsActive = true, PerPage = 5 }, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/projects/14308069/task_assignments", "?is_active=true&per_page=5");
	}

	[Fact]
	public async Task ListAsync_ForProject_NullOptions_SendsNoQuery()
	{
		var call = await TestClient.SendAsync((c, ct) => c.ProjectTaskAssignments.ListAsync(14308069, null, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/projects/14308069/task_assignments");
	}

	[Fact]
	public async Task ListAsync_ForProject_SendsEveryOption()
	{
		var call = await TestClient.SendAsync((c, ct) => c.ProjectTaskAssignments.ListAsync(14308069, _everyOption, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/projects/14308069/task_assignments", EveryOptionQuery);
	}

	[Fact]
	public async Task ListAsync_AllProjects_NullOptions_SendsNoQuery()
	{
		var call = await TestClient.SendAsync((c, ct) => c.ProjectTaskAssignments.ListAsync(null, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/task_assignments");
	}

	[Fact]
	public async Task ListAsync_AllProjects_SendsEveryOption()
	{
		var call = await TestClient.SendAsync((c, ct) => c.ProjectTaskAssignments.ListAsync(_everyOption, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/task_assignments", EveryOptionQuery);
	}

	[Fact]
	public async Task ListAsync_AllProjects_ReadsTheTaskAssignmentsKey()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.ProjectTaskAssignments.ListAsync(null, ct), $$$"""
			{"task_assignments":[{{{AssignmentJson}}},
			{"id":160726647,"billable":false,"is_active":true,"created_at":"2017-08-22T17:36:54Z","updated_at":"2017-08-22T17:36:54Z",
			"hourly_rate":100.0,"budget":null,"project":{"id":14808188,"name":"Task Force","code":"TF"},"task":{"id":8083369,"name":"Research"}}],
			"per_page":2000,"total_pages":1,"total_entries":2,"next_page":null,"previous_page":null,"page":1,
			"links":{"first":"https://api.harvestapp.com/v2/task_assignments?page=1&per_page=2000","next":null,"previous":null,
			"last":"https://api.harvestapp.com/v2/task_assignments?page=1&per_page=2000"}}
			""");

		page.TaskAssignments.Should().HaveCount(2);
		ShouldBeTheAssignment(page.TaskAssignments[0]);
		page.TaskAssignments[1].Project!.Name.Should().Be("Task Force");
		page.TotalEntries.Should().Be(2);
	}

	[Fact]
	public async Task ListAsync_ReadsTheTaskAssignmentsKey()
	{
		// Harvest names the array "task_assignments"; before 2.5 the property snake-cased to "project_task_assignments"
		// and the list was always empty.
		var page = await TestClient.ReadAsync(
			(c, ct) => c.ProjectTaskAssignments.ListAsync(14308069, null, ct),
			$$$"""{"task_assignments":[{{{AssignmentJson}}}],"per_page":2000,"total_pages":1,"total_entries":1,"page":1}""");

		ShouldBeTheAssignment(page.TaskAssignments.Should().ContainSingle().Subject);
	}

	[Fact]
	public async Task GetAsync_SendsGetAndMapsTheAssignment()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.OK, AssignmentJson);
		using var client = TestClient.Create(stub);

		var assignment = await client.ProjectTaskAssignments.GetAsync(14308069, 155505016, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Subject.ShouldBe(HttpMethod.Get, "/v2/projects/14308069/task_assignments/155505016");
		ShouldBeTheAssignment(assignment);
	}

	[Fact]
	public async Task CreateAsync_RequiredOnly_LeavesBillableToHarvest()
	{
		var call = await TestClient.SendAsync(
			(c, ct) => c.ProjectTaskAssignments.CreateAsync(14308069, new ProjectTaskAssignmentCreationDto { TaskId = 8083369 }, ct),
			AssignmentJson);

		call.ShouldBe(HttpMethod.Post, "/v2/projects/14308069/task_assignments", body: """{"task_id":8083369}""");
	}

	[Fact]
	public async Task CreateAsync_AllFields_SendsEveryField()
	{
		var call = await TestClient.SendAsync((c, ct) => c.ProjectTaskAssignments.CreateAsync(14308069, new ProjectTaskAssignmentCreationDto
		{
			TaskId = 8083369,
			IsActive = true,
			Billable = false,
			HourlyRate = 75.5m,
			Budget = 10m
		}, ct), AssignmentJson);

		call.ShouldBe(HttpMethod.Post, "/v2/projects/14308069/task_assignments",
			body: """{"task_id":8083369,"is_active":true,"billable":false,"hourly_rate":75.5,"budget":10}""");
	}

	[Fact]
	public async Task UpdateAsync_SendsPatch()
	{
		var call = await TestClient.SendAsync((c, ct) => c.ProjectTaskAssignments.UpdateAsync(14308069, 155505016, new ProjectTaskAssignmentPatchDto
		{
			IsActive = false,
			Billable = true,
			HourlyRate = 1m,
			Budget = 2m
		}, ct), AssignmentJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/projects/14308069/task_assignments/155505016",
			body: """{"is_active":false,"billable":true,"hourly_rate":1,"budget":2}""");
	}

	[Fact]
	public async Task DeleteAsync_SendsDelete()
	{
		var call = await TestClient.SendAsync((c, ct) => c.ProjectTaskAssignments.DeleteAsync(14308069, 155505016, ct), "");

		call.ShouldBe(HttpMethod.Delete, "/v2/projects/14308069/task_assignments/155505016");
	}

	private static void ShouldBeTheAssignment(ProjectTaskAssignment assignment)
	{
		assignment.Id.Should().Be(155505016);
		assignment.Billable.Should().BeFalse();
		assignment.IsActive.Should().BeTrue();
		assignment.HourlyRate.Should().Be(100m);
		assignment.Budget.Should().BeNull();
		assignment.CreatedAt.Should().Be(new DateTime(2017, 6, 26, 21, 52, 18, DateTimeKind.Utc));
		assignment.UpdatedAt.Should().Be(new DateTime(2017, 6, 26, 21, 54, 6, DateTimeKind.Utc));
		assignment.Project!.Id.Should().Be(14308069);
		assignment.Project.Name.Should().Be("Online Store - Phase 1");
		assignment.Project.Code.Should().Be("OS1");
		assignment.Task!.Id.Should().Be(8083369);
		assignment.Task.Name.Should().Be("Research");
	}
}