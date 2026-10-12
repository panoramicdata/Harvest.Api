namespace Harvest.Test;

public class UserProjectAssignmentTests(ITestOutputHelper testOutputHelper) : HarvestTest(testOutputHelper)
{
	[Fact]
	public async System.Threading.Tasks.Task ListMine()
	{
		var userProjectAssignmentsContainer = await HarvestClient
			.UserProjectAssignments
			.ListMineAsync(
				new() { Page = 1, PerPage = 10 }, CancellationToken
			);

		userProjectAssignmentsContainer.Should().NotBeNull();
		userProjectAssignmentsContainer.ProjectAssignments.Should().NotBeNull();
	}

	[Fact]
	public async System.Threading.Tasks.Task ListForUser()
	{
		// Get current user first
		var user = await HarvestClient.Users.GetMeAsync(CancellationToken);
		user.Should().NotBeNull();

		var userProjectAssignmentsContainer = await HarvestClient.UserProjectAssignments.ListAsync(
			user.Id,
			new() { Page = 1, PerPage = 10 }, CancellationToken
		);

		userProjectAssignmentsContainer.Should().NotBeNull();
		userProjectAssignmentsContainer.ProjectAssignments.Should().NotBeNull();
	}
}