using Harvest.UnitTest.Support;

namespace Harvest.UnitTest.Resources;

public class TaskApiTests
{
	private const string TaskJson = """
		{"id":8083800,"name":"Business Development","billable_by_default":false,"default_hourly_rate":0.0,"is_default":false,
		"is_active":true,"created_at":"2017-06-26T22:08:25Z","updated_at":"2017-06-26T22:08:25Z"}
		""";

	[Fact]
	public async Task ListAsync_SendsGetWithOptions()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Tasks.ListAsync(new ListOptions { IsActive = true }, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/tasks", "?is_active=true");
	}

	[Fact]
	public async Task ListAsync_MapsTheTasks()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.Tasks.ListAsync(null, ct), $$$"""{"tasks":[{{{TaskJson}}}]}""");

		ShouldBeTheTask(page.Tasks.Should().ContainSingle().Subject);
	}

	[Fact]
	public async Task GetAsync_SendsGetAndMapsTheTask()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.OK, TaskJson);
		using var client = TestClient.Create(stub);

		var task = await client.Tasks.GetAsync(8083800, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Subject.ShouldBe(HttpMethod.Get, "/v2/tasks/8083800");
		ShouldBeTheTask(task);
	}

	[Fact]
	public async Task CreateAsync_RequiredOnly_LeavesDefaultsToHarvest()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Tasks.CreateAsync(new TaskCreationDto { Name = "Research" }, ct), TaskJson);

		call.ShouldBe(HttpMethod.Post, "/v2/tasks", body: """{"name":"Research"}""");
	}

	[Fact]
	public async Task CreateAsync_AllFields_SendsEveryField()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Tasks.CreateAsync(new TaskCreationDto
		{
			Name = "Research",
			BillableByDefault = false,
			DefaultHourlyRate = 50m,
			IsDefault = true,
			IsActive = false
		}, ct), TaskJson);

		call.ShouldBe(HttpMethod.Post, "/v2/tasks",
			body: """{"name":"Research","billable_by_default":false,"default_hourly_rate":50,"is_default":true,"is_active":false}""");
	}

	[Fact]
	public async Task UpdateAsync_SendsPatch()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Tasks.UpdateAsync(8083800, new TaskPatchDto
		{
			Name = "R",
			BillableByDefault = true,
			DefaultHourlyRate = 1m,
			IsDefault = false,
			IsActive = true
		}, ct), TaskJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/tasks/8083800",
			body: """{"name":"R","billable_by_default":true,"default_hourly_rate":1,"is_default":false,"is_active":true}""");
	}

	[Fact]
	public async Task DeleteAsync_SendsDelete()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Tasks.DeleteAsync(8083800, ct), "");

		call.ShouldBe(HttpMethod.Delete, "/v2/tasks/8083800");
	}

	private static void ShouldBeTheTask(Models.Task task)
	{
		task.Id.Should().Be(8083800);
		task.Name.Should().Be("Business Development");
		task.BillableByDefault.Should().BeFalse();
		task.DefaultHourlyRate.Should().Be(0m);
		task.IsDefault.Should().BeFalse();
		task.IsActive.Should().BeTrue();
		task.CreatedAt.Should().Be(new DateTime(2017, 6, 26, 22, 8, 25, DateTimeKind.Utc));
		task.UpdatedAt.Should().Be(new DateTime(2017, 6, 26, 22, 8, 25, DateTimeKind.Utc));
	}
}