namespace Harvest.Test;

public class TimeEntriesTests(ITestOutputHelper testOutputHelper) : HarvestTest(testOutputHelper)
{
	[Fact]
	public async System.Threading.Tasks.Task ListCreateGetPatchDelete()
	{
		try
		{
			// Get the user
			var user = await HarvestClient.Users.GetMeAsync(CancellationToken);
			user.Should().NotBeNull();
			var userId = user.Id;

			// Ensure we have test project and task
			var project = await TestDataManager.EnsureTestProjectAsync("Time Entry Test Project");
			var task = await TestDataManager.EnsureTestTaskAsync("Time Entry Test Task");

			// List existing time entries
			var timeEntriesContainer = await HarvestClient.TimeEntries.ListAllAsync(
				projectId: project.Id,
				userId: userId,
				from: "2018-01-24",
				toDate: "2018-05-24",
				page: 1,
				perPage: 100,
				cancellationToken: CancellationToken
			);

			timeEntriesContainer.Should().NotBeNull();

			// Create a time entry
			var newTimeEntry = await HarvestClient.TimeEntries.CreateAsync(new TimeEntryCreationDto
			{
				UserId = userId,
				ProjectId = project.Id,
				Notes = $"{Configuration.TestSettings.TestDataPrefix}Test time entry",
				SpentDate = "2018-06-20",
				TaskId = task.Id,
				Hours = 1
			},
			CancellationToken
			);
			newTimeEntry.Should().NotBeNull();
			TestDataManager.TrackTimeEntry(newTimeEntry.Id); // Track for cleanup

			// Get (Re-fetch) it
			var refetchedTimeEntry = await HarvestClient.TimeEntries.GetAsync(newTimeEntry.Id, CancellationToken);
			refetchedTimeEntry.Should().NotBeNull();

			// Patch it
			await HarvestClient.TimeEntries.PatchAsync(refetchedTimeEntry.Id, new TimeEntryPatchDto
			{
				Notes = $"{Configuration.TestSettings.TestDataPrefix}Updated test time entry"
			}, CancellationToken);

			// Verify the update
			var updatedEntry = await HarvestClient.TimeEntries.GetAsync(newTimeEntry.Id, CancellationToken);
			updatedEntry.Notes.Should().Be($"{Configuration.TestSettings.TestDataPrefix}Updated test time entry");

			// Delete
			await HarvestClient.TimeEntries.DeleteAsync(refetchedTimeEntry.Id, CancellationToken);
			TestDataManager.UntrackTimeEntry(newTimeEntry.Id); // Remove from tracking since we deleted it
		}
		finally
		{
			await TestDataManager.CleanupAsync();
		}
	}

	[Fact]
	public async System.Threading.Tasks.Task ListAllTimeEntries()
	{
		var timeEntriesContainer = await HarvestClient.TimeEntries.ListAllAsync(
			page: 1,
			perPage: 10,
			cancellationToken: CancellationToken
		);

		timeEntriesContainer.Should().NotBeNull();
		timeEntriesContainer.TimeEntries.Should().NotBeNull();
	}

	[Fact]
	public async System.Threading.Tasks.Task GetTimeEntry()
	{
		try
		{
			// Create a test time entry
			var timeEntry = await TestDataManager.CreateTestTimeEntryAsync("Get test entry", 2.0m);

			// Get the time entry
			var retrievedEntry = await HarvestClient.TimeEntries.GetAsync(timeEntry.Id, CancellationToken);
			retrievedEntry.Should().NotBeNull();
			retrievedEntry.Id.Should().Be(timeEntry.Id);
			retrievedEntry.Notes.Should().Be($"{Configuration.TestSettings.TestDataPrefix}Get test entry");
			retrievedEntry.Hours.Should().Be(2.0m);
		}
		finally
		{
			await TestDataManager.CleanupAsync();
		}
	}
}
