namespace Harvest.Test;

public class TaskTests(ITestOutputHelper testOutputHelper) : HarvestTest(testOutputHelper)
{
	[Fact]
	public async System.Threading.Tasks.Task ListAllTasks()
	{
		var tasksContainer = await HarvestClient.Tasks.ListAsync(new() { Page = 1, PerPage = 10 }, CancellationToken);

		tasksContainer.Should().NotBeNull();
		tasksContainer.Tasks.Should().NotBeNull();
	}

	[Fact]
	public async System.Threading.Tasks.Task GetTask()
	{
		var tasksContainer = await HarvestClient.Tasks.ListAsync(new() { Page = 1, PerPage = 1 }, CancellationToken);
		if (FirstOrLog(tasksContainer.Tasks, "tasks") is not { } first)
		{
			return;
		}

		var task = await HarvestClient.Tasks.GetAsync(first.Id, CancellationToken);

		task.Should().NotBeNull();
		task.Id.Should().Be(first.Id);
		task.Name.Should().NotBeNullOrEmpty();
	}

	[Fact]
	public System.Threading.Tasks.Task CreateUpdateDeleteTask()
		=> RunCrudScenarioAsync(async () =>
		{
			var createdTask = await CreateTestTaskAsync("CRUD", defaultHourlyRate: 50.00m);
			createdTask.Id.Should().BePositive();
			createdTask.Name.Should().StartWith($"{Prefix}CRUD Test Task");
			createdTask.BillableByDefault.Should().BeTrue();
			createdTask.DefaultHourlyRate.Should().Be(50.00m);

			var retrievedTask = await HarvestClient.Tasks.GetAsync(createdTask.Id, CancellationToken);
			retrievedTask.Id.Should().Be(createdTask.Id);
			retrievedTask.Name.Should().Be(createdTask.Name);
			Log.Retrieved(Logger, "task", retrievedTask.Id);

			var updateDto = new TaskPatchDto
			{
				Name = $"{createdTask.Name} - Updated",
				DefaultHourlyRate = 75.00m
			};
			var updatedTask = await HarvestClient.Tasks.UpdateAsync(createdTask.Id, updateDto, CancellationToken);
			updatedTask.Id.Should().Be(createdTask.Id);
			updatedTask.Name.Should().Be(updateDto.Name);
			updatedTask.DefaultHourlyRate.Should().Be(75.00m);
			Log.Updated(Logger, "task", updatedTask.Id);

			await DeleteTrackedAsync("task", createdTask.Id);
		});
}