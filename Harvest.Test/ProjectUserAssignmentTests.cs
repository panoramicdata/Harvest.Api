namespace Harvest.Test;

public class ProjectUserAssignmentTests(ITestOutputHelper testOutputHelper) : HarvestTest(testOutputHelper)
{
	[Fact]
	public async System.Threading.Tasks.Task ListAllUserAssignments()
	{
		var container = await HarvestClient.ProjectUserAssignments.ListAsync(new UserAssignmentListOptions { Page = 1, PerPage = 10 }, CancellationToken);

		container.UserAssignments.Should().NotBeNull();
	}

	[Fact]
	public System.Threading.Tasks.Task CreateReadUpdateDeleteProjectUserAssignment()
		=> RunStrictScenarioAsync(async () =>
		{
			var me = await HarvestClient.Users.GetMeAsync(CancellationToken);
			var client = await CreateTestClientAsync("UserAssignment", address: null);
			var project = await CreateTestProjectAsync(client.Id, "UserAssignment", notes: null);

			var created = await HarvestClient.ProjectUserAssignments.CreateAsync(project.Id, new UserAssignmentCreationDto
			{
				UserId = me.Id,
				IsActive = true,
				IsProjectManager = false,
				UseDefaultRates = false,
				HourlyRate = 60m
			}, CancellationToken);
			Track("user assignment", created.Id, null, id => HarvestClient.ProjectUserAssignments.DeleteAsync(project.Id, id, CancellationToken));
			created.User!.Id.Should().Be(me.Id);
			created.Project!.Id.Should().Be(project.Id);

			var retrieved = await HarvestClient.ProjectUserAssignments.GetAsync(project.Id, created.Id, CancellationToken);
			retrieved.Id.Should().Be(created.Id);

			var forProject = await HarvestClient.ProjectUserAssignments.ListAsync(project.Id, new UserAssignmentListOptions { UserId = me.Id }, CancellationToken);
			forProject.UserAssignments.Should().Contain(assignment => assignment.Id == created.Id);

			var everywhere = await HarvestClient.ProjectUserAssignments.ListAsync(new UserAssignmentListOptions { UserId = me.Id }, CancellationToken);
			everywhere.UserAssignments.Should().Contain(assignment => assignment.Id == created.Id);

			var updated = await HarvestClient.ProjectUserAssignments.UpdateAsync(project.Id, created.Id, new UserAssignmentPatchDto
			{
				IsProjectManager = true,
				HourlyRate = 70m,
				Budget = 10m
			}, CancellationToken);
			updated.Id.Should().Be(created.Id);
			updated.IsProjectManager.Should().BeTrue();

			await DeleteTrackedAsync("user assignment", created.Id);
		});
}