using Xunit;
using Xunit.Abstractions;
using System.Linq;
using Harvest.Models;

namespace Harvest.Test
{
	public class TimeEntriesTests : HarvestTest
	{
		public TimeEntriesTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}

		[Fact]
		public async Task ListCreateGetPatchDelete()
		{
			// Get the user
			var user = await HarvestClient.Users.GetMeAsync().ConfigureAwait(false);
			Assert.NotNull(user);
			var userId = user.Id;

			// Get User project assignments
			var userProjectAssignments = (await HarvestClient.UserProjectAssignments.ListAllMineAsync().ConfigureAwait(false)).ProjectAssignments;
			Assert.NotEmpty(userProjectAssignments);

			// Get the test project
			var userProjectAssignment = userProjectAssignments.SingleOrDefault(p => p.Project.Name == "Panoramic Data Limited Test Project");
			Assert.NotNull(userProjectAssignment);

			// Get the task assignments
			var taskAssignments = userProjectAssignment.TaskAssignments;
			Assert.NotNull(taskAssignments);
			Assert.NotEmpty(taskAssignments);
			var taskId = taskAssignments[0].Task.Id;

			// Get the projectId
			var projectId = userProjectAssignment.Project.Id;

			// List existing
			var timeEntriesContainer = await HarvestClient.TimeEntries.ListAllAsync(
				projectId: projectId,
				userId: userId,
				from: "2018-01-24",
				to: "2018-05-24",
				page: 1,
				perPage: 100
			).ConfigureAwait(false);
			Assert.NotNull(timeEntriesContainer);
			Assert.NotEqual(0, timeEntriesContainer.TotalEntries);

			// Create a time entry
			var newTimeEntry = await HarvestClient.TimeEntries.CreateAsync(new TimeEntryCreationDto
			{
				UserId = userId,
				ProjectId = projectId,
				Notes = "Woo!",
				SpentDate = "2018-06-20",
				TaskId = taskId,
				Hours = 1
			}
			).ConfigureAwait(false);
			Assert.NotNull(newTimeEntry);

			// Get (Re-fetch) it
			var refetchedTimeEntry = await HarvestClient.TimeEntries.GetAsync(newTimeEntry.Id).ConfigureAwait(false);
			Assert.NotNull(refetchedTimeEntry);

			// Patch it
			await HarvestClient.TimeEntries.PatchAsync(refetchedTimeEntry.Id, new TimeEntryPatchDto
			{
				Notes = "Yay!"
			}).ConfigureAwait(false);

			// Delete
			await HarvestClient.TimeEntries.DeleteAsync(refetchedTimeEntry.Id).ConfigureAwait(false);
		}
	}
}
