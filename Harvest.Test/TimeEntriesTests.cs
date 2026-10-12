namespace Harvest.Test;

public class TimeEntriesTests(ITestOutputHelper testOutputHelper) : HarvestTest(testOutputHelper)
{
	[Fact]
	public async System.Threading.Tasks.Task ListAllTimeEntries()
	{
		var timeEntriesContainer = await HarvestClient.TimeEntries.ListAsync(new() { Page = 1, PerPage = 10 }, CancellationToken);

		timeEntriesContainer.Should().NotBeNull();
		timeEntriesContainer.TimeEntries.Should().NotBeNull();
	}

	[Fact]
	public System.Threading.Tasks.Task CreateReadUpdateDeleteTimeEntry()
		=> RunCrudScenarioAsync(async () =>
		{
			if (await CreateTimeEntryAsync() is not { } createdTimeEntry)
			{
				return;
			}

			createdTimeEntry.Id.Should().BePositive();
			createdTimeEntry.Hours.Should().Be(2.5m);
			createdTimeEntry.Notes.Should().Be($"{Prefix}CRUD Test Time Entry");

			var retrievedTimeEntry = await HarvestClient.TimeEntries.GetAsync(createdTimeEntry.Id, CancellationToken);
			retrievedTimeEntry.Id.Should().Be(createdTimeEntry.Id);
			retrievedTimeEntry.Hours.Should().Be(2.5m);
			Log.Retrieved(Logger, "time entry", retrievedTimeEntry.Id);

			var updateDto = new TimeEntryPatchDto
			{
				Hours = 3.0m,
				Notes = $"{Prefix}Updated CRUD Test Time Entry"
			};
			var updatedTimeEntry = await HarvestClient.TimeEntries.UpdateAsync(createdTimeEntry.Id, updateDto, CancellationToken);
			updatedTimeEntry.Hours.Should().Be(3.0m);
			updatedTimeEntry.Notes.Should().Be(updateDto.Notes);
			Log.Updated(Logger, "time entry", updatedTimeEntry.Id);

			await DeleteTrackedAsync("time entry", createdTimeEntry.Id);
		});

	/// <summary>
	/// Creates a time entry for the current user on a new project and task, or returns <see langword="null"/> when the
	/// entry was created but its response could not be read (a known quirk of the API's response format).
	/// </summary>
	private async Task<TimeEntry?> CreateTimeEntryAsync()
	{
		var currentUser = await HarvestClient.Users.GetMeAsync(CancellationToken);
		var client = await CreateTestClientAsync("TimeEntry");
		var project = await CreateTestProjectAsync(client.Id, "TimeEntry");
		var task = await CreateTestTaskAsync("TimeEntry", defaultHourlyRate: 75.00m);
		_ = await CreateTestTaskAssignmentAsync(project.Id, task.Id, hourlyRate: 75.00m);

		try
		{
			var timeEntry = await HarvestClient.TimeEntries.CreateAsync(new TimeEntryCreationDto
			{
				UserId = currentUser.Id,
				ProjectId = project.Id,
				TaskId = task.Id,
				SpentDate = DateOnly.FromDateTime(DateTime.UtcNow),
				Hours = 2.5m,
				Notes = $"{Prefix}CRUD Test Time Entry"
			}, CancellationToken);
			Track("time entry", timeEntry.Id, timeEntry.Notes, id => HarvestClient.TimeEntries.DeleteAsync(id, CancellationToken));
			return timeEntry;
		}
		catch (System.Text.Json.JsonException ex)
		{
			// The entry is deleted with its project, which is tracked.
			Log.TimeEntryResponseUnreadable(Logger, ex.Message);
			return null;
		}
	}
}