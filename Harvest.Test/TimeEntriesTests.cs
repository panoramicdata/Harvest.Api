using Harvest.Models;
using System.Linq;

namespace Harvest.Test;

public class TimeEntriesTests(ITestOutputHelper testOutputHelper) : HarvestTest(testOutputHelper)
{
	[Fact]
	public async System.Threading.Tasks.Task ListCreateGetPatchDelete()
	{
		// Get the user
		var user = await HarvestClient.Users.GetMeAsync(CancellationToken);
		user.Should().NotBeNull();
		var userId = user.Id;

		// Get User project assignments
		var userProjectAssignments = (await HarvestClient.UserProjectAssignments.ListAllMineAsync(cancellationToken: CancellationToken)).ProjectAssignments;
		userProjectAssignments.Should().NotBeNullOrEmpty();

		// Get the test project
		var userProjectAssignment = userProjectAssignments.SingleOrDefault(p => p.Project?.Name == "Panoramic Data Limited Test Project");
		userProjectAssignment.Should().NotBeNull();

		// Get the task assignments
		var taskAssignments = userProjectAssignment.TaskAssignments;
		taskAssignments.Should().NotBeNullOrEmpty();
		var taskId = taskAssignments[0].Task!.Id;

		// Get the projectId
		var projectId = userProjectAssignment.Project!.Id;

		// List existing
		var timeEntriesContainer = await HarvestClient.TimeEntries.ListAllAsync(
			projectId: projectId,
			userId: userId,
			from: "2018-01-24",
			toDate: "2018-05-24",
			page: 1,
			perPage: 100,
			cancellationToken: CancellationToken
		);

		timeEntriesContainer.Should().NotBeNull();
		timeEntriesContainer.TotalEntries.Should().Be(0);

		// Create a time entry
		var newTimeEntry = await HarvestClient.TimeEntries.CreateAsync(new TimeEntryCreationDto
		{
			UserId = userId,
			ProjectId = projectId,
			Notes = "Woo!",
			SpentDate = "2018-06-20",
			TaskId = taskId,
			Hours = 1
		},
		CancellationToken
		);
		newTimeEntry.Should().NotBeNull();

		// Get (Re-fetch) it
		var refetchedTimeEntry = await HarvestClient.TimeEntries.GetAsync(newTimeEntry.Id, CancellationToken);
		refetchedTimeEntry.Should().NotBeNull();

		// Patch it
		await HarvestClient.TimeEntries.PatchAsync(refetchedTimeEntry.Id, new TimeEntryPatchDto
		{
			Notes = "Yay!"
		}, CancellationToken);

		// Delete
		await HarvestClient.TimeEntries.DeleteAsync(refetchedTimeEntry.Id, CancellationToken);
	}
}
