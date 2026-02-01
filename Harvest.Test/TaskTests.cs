namespace Harvest.Test;

public class TaskTests(ITestOutputHelper testOutputHelper) : HarvestTest(testOutputHelper)
{
	[Fact]
	public async System.Threading.Tasks.Task ListAllTasks()
	{
		var tasksContainer = await HarvestClient.Tasks.ListAllAsync(
			page: 1,
			perPage: 10,
			cancellationToken: CancellationToken
		);

		tasksContainer.Should().NotBeNull();
		tasksContainer.Tasks.Should().NotBeNull();
	}

	[Fact]
	public async System.Threading.Tasks.Task GetTask()
	{
		// First get a task from the list
		var tasksContainer = await HarvestClient.Tasks.ListAllAsync(
			page: 1,
			perPage: 1,
			cancellationToken: CancellationToken
		);

		if (tasksContainer.Tasks.Count == 0)
		{
			// Skip if no tasks exist
			return;
		}

		var taskId = tasksContainer.Tasks[0].Id;
		var task = await HarvestClient.Tasks.GetAsync(taskId, CancellationToken);

		task.Should().NotBeNull();
		task.Id.Should().Be(taskId);
		task.Name.Should().NotBeNullOrEmpty();
	}

	[Fact]
	public async System.Threading.Tasks.Task CreateUpdateDeleteTask()
	{
		if (!Configuration.TestSettings.CreateTestData)
		{
			// Skip if test data creation is disabled
			Logger.LogInformation("Test data creation is disabled");
			return;
		}

		try
		{
			// Create a new task
			var creationDto = new TaskCreationDto
			{
				Name = $"{Configuration.TestSettings.TestDataPrefix}CRUD Test Task {DateTime.UtcNow:yyyyMMddHHmmss}",
				BillableByDefault = true,
				DefaultHourlyRate = 50.00m,
				IsDefault = false,
				IsActive = true
			};

			var createdTask = await HarvestClient.Tasks.CreateAsync(creationDto, CancellationToken);
			createdTask.Should().NotBeNull();
			createdTask.Id.Should().BePositive();
			createdTask.Name.Should().Be(creationDto.Name);

			// Update the task
			var updateDto = new TaskPatchDto
			{
				Name = $"{creationDto.Name} - Updated",
				DefaultHourlyRate = 75.00m
			};

			var updatedTask = await HarvestClient.Tasks.UpdateAsync(createdTask.Id, updateDto, CancellationToken);
			updatedTask.Should().NotBeNull();
			updatedTask.Id.Should().Be(createdTask.Id);
			updatedTask.Name.Should().Be(updateDto.Name);
			updatedTask.DefaultHourlyRate.Should().Be(updateDto.DefaultHourlyRate);

			// Delete the task
			await HarvestClient.Tasks.DeleteAsync(createdTask.Id, CancellationToken);
			TestDataManager.UntrackTask(createdTask.Id); // Remove from tracking since we deleted it
		}
		catch (Refit.ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
		{
			// Skip if the API doesn't allow these operations (common in sandbox environments)
			Logger.LogInformation("CRUD operations not permitted in this environment");
		}
		finally
		{
			await TestDataManager.CleanupAsync();
		}
	}
}
