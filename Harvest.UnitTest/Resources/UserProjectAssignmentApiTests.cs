using Harvest.UnitTest.Support;

namespace Harvest.UnitTest.Resources;

public class UserProjectAssignmentApiTests
{
	private const string PageJson = """
		{"project_assignments":[{"id":125066109,"is_project_manager":true,"is_active":true,"use_default_rates":true,"budget":null,
		"created_at":"2017-06-26T21:52:18Z","updated_at":"2017-06-26T21:52:18Z","hourly_rate":100.0,
		"project":{"id":14308069,"name":"Online Store - Phase 1","code":"OS1"},"client":{"id":5735776,"name":"123 Industries"},
		"task_assignments":[{"id":155505013,"billable":true,"is_active":true,"created_at":"2017-06-26T21:52:18Z","updated_at":"2017-06-26T21:52:18Z",
		"hourly_rate":100.0,"budget":null,"task":{"id":8083365,"name":"Graphic Design"}}]}],
		"per_page":2000,"total_pages":1,"total_entries":1,"next_page":null,"previous_page":null,"page":1,
		"links":{"first":"https://api.harvestapp.com/v2/users/1782959/project_assignments?page=1&per_page=2000","next":null,"previous":null,
		"last":"https://api.harvestapp.com/v2/users/1782959/project_assignments?page=1&per_page=2000"}}
		""";

	[Fact]
	public async Task ListAsync_SendsGetForTheUser()
	{
		var call = await TestClient.SendAsync((c, ct) => c.UserProjectAssignments.ListAsync(1782959, new ListOptions { PerPage = 10 }, ct), PageJson);

		call.ShouldBe(HttpMethod.Get, "/v2/users/1782959/project_assignments", "?per_page=10");
	}

	[Fact]
	public async Task ListMineAsync_SendsGetForMe()
	{
		var call = await TestClient.SendAsync((c, ct) => c.UserProjectAssignments.ListMineAsync(null, ct), PageJson);

		call.ShouldBe(HttpMethod.Get, "/v2/users/me/project_assignments");
	}

	[Fact]
	public async Task ListAsync_MapsTheAssignments()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.UserProjectAssignments.ListMineAsync(null, ct), PageJson);

		var assignment = page.ProjectAssignments.Should().ContainSingle().Subject;
		assignment.Id.Should().Be(125066109);
		assignment.IsProjectManager.Should().BeTrue();
		assignment.IsActive.Should().BeTrue();
		assignment.UseDefaultRates.Should().BeTrue();
		assignment.Budget.Should().BeNull();
		assignment.HourlyRate.Should().Be(100m);
		assignment.CreatedAt.Should().Be(new DateTime(2017, 6, 26, 21, 52, 18, DateTimeKind.Utc));
		assignment.UpdatedAt.Should().Be(new DateTime(2017, 6, 26, 21, 52, 18, DateTimeKind.Utc));
		assignment.Project!.Code.Should().Be("OS1");
		assignment.Client!.Name.Should().Be("123 Industries");
		var taskAssignment = assignment.TaskAssignments.Should().ContainSingle().Subject;
		taskAssignment.Task!.Name.Should().Be("Graphic Design");
		page.Links!.Next.Should().BeNull();
		page.Links.Last.Should().EndWith("page=1&per_page=2000");
	}
}