namespace Harvest.Test;

public class ProjectTaskAssignmentTests(ITestOutputHelper testOutputHelper) : HarvestTest(testOutputHelper)
{
	[Fact]
	public async System.Threading.Tasks.Task ListAllProjectTaskAssignments()
	{
		if (await GetFirstProjectAsync() is not { } project)
		{
			return;
		}

		var taskAssignmentsContainer = await HarvestClient.ProjectTaskAssignments.ListAsync(
			project.Id,
			new() { Page = 1, PerPage = 10 },
			CancellationToken);

		taskAssignmentsContainer.Should().NotBeNull();
		taskAssignmentsContainer.TaskAssignments.Should().NotBeNull();
	}

	[Fact]
	public async System.Threading.Tasks.Task GetProjectTaskAssignment()
	{
		if (await GetFirstProjectAsync() is not { } project)
		{
			return;
		}

		var taskAssignmentsContainer = await HarvestClient.ProjectTaskAssignments.ListAsync(
			project.Id,
			new() { Page = 1, PerPage = 1 },
			CancellationToken);
		if (FirstOrLog(taskAssignmentsContainer.TaskAssignments, "task assignments") is not { } first)
		{
			return;
		}

		var assignment = await HarvestClient.ProjectTaskAssignments.GetAsync(project.Id, first.Id, CancellationToken);

		assignment.Should().NotBeNull();
		assignment.Id.Should().Be(first.Id);
		assignment.Task.Should().NotBeNull();
	}

	[Fact]
	public System.Threading.Tasks.Task CreateUpdateDeleteProjectTaskAssignment()
		=> RunCrudScenarioAsync(async () =>
		{
			var client = await CreateTestClientAsync("TaskAssignment", address: null);
			var project = await CreateTestProjectAsync(client.Id, "TaskAssignment", notes: null);
			var task = await CreateTestTaskAsync("TaskAssignment", defaultHourlyRate: 100.00m);

			var createdAssignment = await CreateTestTaskAssignmentAsync(project.Id, task.Id, hourlyRate: 150.00m);
			createdAssignment.Id.Should().BePositive();
			createdAssignment.Task.Should().NotBeNull();
			createdAssignment.Task!.Id.Should().Be(task.Id);
			createdAssignment.Billable.Should().BeTrue();

			var retrievedAssignment = await HarvestClient.ProjectTaskAssignments.GetAsync(project.Id, createdAssignment.Id, CancellationToken);
			retrievedAssignment.Id.Should().Be(createdAssignment.Id);
			Log.Retrieved(Logger, "task assignment", retrievedAssignment.Id);

			var updateDto = new ProjectTaskAssignmentPatchDto
			{
				HourlyRate = 175.00m,
				Billable = false
			};
			var updatedAssignment = await HarvestClient.ProjectTaskAssignments.UpdateAsync(project.Id, createdAssignment.Id, updateDto, CancellationToken);
			// HourlyRate may be null if the project does not bill by task, so only Billable is asserted.
			updatedAssignment.Billable.Should().BeFalse();
			Log.Updated(Logger, "task assignment", updatedAssignment.Id);

			await DeleteTrackedAsync("task assignment", createdAssignment.Id);
		});

	private async Task<Project?> GetFirstProjectAsync()
	{
		var projectsContainer = await HarvestClient.Projects.ListAsync(new() { Page = 1, PerPage = 1 }, CancellationToken);
		return FirstOrLog(projectsContainer.Projects, "projects");
	}
}