using Harvest.UnitTest.Support;

namespace Harvest.UnitTest.Resources;

public class ProjectApiTests
{
	private const string ProjectJson = """
		{"id":14308069,"name":"Online Store - Phase 1","code":"OS1","is_active":true,"bill_by":"Project","budget":200.0,
		"budget_by":"project","budget_is_monthly":true,"notify_when_over_budget":true,"over_budget_notification_percentage":80.0,
		"over_budget_notification_date":"2017-07-01","show_budget_to_all":false,"created_at":"2017-06-26T21:52:18Z",
		"updated_at":"2017-06-26T21:54:06Z","starts_on":"2017-06-01","ends_on":null,"is_billable":true,"is_fixed_fee":false,
		"notes":"Notes","client":{"id":5735776,"name":"123 Industries","currency":"EUR"},"cost_budget":null,
		"cost_budget_include_expenses":false,"hourly_rate":100.0,"fee":null}
		""";

	[Fact]
	public async Task ListAsync_NoOptions_SendsNoQuery()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Projects.ListAsync(null, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/projects");
	}

	[Fact]
	public async Task ListAsync_ClientFilter_SendsClientId()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Projects.ListAsync(new ProjectListOptions { ClientId = 5735776, IsActive = false }, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/projects", "?client_id=5735776&is_active=false");
	}

	[Fact]
	public async Task ListAsync_MapsTheProjects()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.Projects.ListAsync(null, ct), $$$"""{"projects":[{{{ProjectJson}}}]}""");

		ShouldBeTheProject(page.Projects.Should().ContainSingle().Subject);
	}

	[Fact]
	public async Task GetAsync_SendsGetAndMapsTheProject()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.OK, ProjectJson);
		using var client = TestClient.Create(stub);

		var project = await client.Projects.GetAsync(14308069, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Subject.ShouldBe(HttpMethod.Get, "/v2/projects/14308069");
		ShouldBeTheProject(project);
	}

	[Fact]
	public async Task CreateAsync_RequiredOnly_SendsRequiredFields()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Projects.CreateAsync(new ProjectCreationDto
		{
			ClientId = 5735776,
			Name = "Store",
			IsBillable = true,
			BillBy = "Project",
			BudgetBy = "project"
		}, ct), ProjectJson);

		call.ShouldBe(HttpMethod.Post, "/v2/projects",
			body: """{"client_id":5735776,"name":"Store","is_billable":true,"bill_by":"Project","budget_by":"project"}""");
	}

	[Fact]
	public async Task CreateAsync_AllFields_SendsEveryField()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Projects.CreateAsync(new ProjectCreationDto
		{
			ClientId = 1,
			Name = "Store",
			Code = "ST",
			IsActive = true,
			IsBillable = false,
			IsFixedFee = true,
			BillBy = "none",
			HourlyRate = 100m,
			BudgetBy = "project_cost",
			BudgetIsMonthly = true,
			Budget = 10.5m,
			CostBudget = 2000m,
			CostBudgetIncludeExpenses = true,
			NotifyWhenOverBudget = true,
			OverBudgetNotificationPercentage = 80m,
			ShowBudgetToAll = false,
			Fee = 5000m,
			Notes = "n",
			StartsOn = new DateOnly(2025, 1, 2),
			EndsOn = new DateOnly(2025, 12, 31)
		}, ct), ProjectJson);

		call.ShouldBe(HttpMethod.Post, "/v2/projects", body: """
			{"client_id":1,"name":"Store","code":"ST","is_active":true,"is_billable":false,"is_fixed_fee":true,"bill_by":"none","hourly_rate":100,"budget_by":"project_cost","budget_is_monthly":true,"budget":10.5,"cost_budget":2000,"cost_budget_include_expenses":true,"notify_when_over_budget":true,"over_budget_notification_percentage":80,"show_budget_to_all":false,"fee":5000,"notes":"n","starts_on":"2025-01-02","ends_on":"2025-12-31"}
			""");
	}

	[Fact]
	public async Task UpdateAsync_AllFields_SendsPatch()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Projects.UpdateAsync(9, new ProjectPatchDto
		{
			ClientId = 1,
			Name = "Store",
			Code = "ST",
			IsActive = false,
			IsBillable = true,
			IsFixedFee = false,
			BillBy = "Tasks",
			HourlyRate = 1m,
			BudgetBy = "task",
			BudgetIsMonthly = false,
			Budget = 2m,
			CostBudget = 3m,
			CostBudgetIncludeExpenses = false,
			NotifyWhenOverBudget = false,
			OverBudgetNotificationPercentage = 4m,
			ShowBudgetToAll = true,
			Fee = 5m,
			Notes = "x",
			StartsOn = new DateOnly(2025, 3, 4),
			EndsOn = new DateOnly(2025, 5, 6)
		}, ct), ProjectJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/projects/9", body: """
			{"client_id":1,"name":"Store","code":"ST","is_active":false,"is_billable":true,"is_fixed_fee":false,"bill_by":"Tasks","hourly_rate":1,"budget_by":"task","budget_is_monthly":false,"budget":2,"cost_budget":3,"cost_budget_include_expenses":false,"notify_when_over_budget":false,"over_budget_notification_percentage":4,"show_budget_to_all":true,"fee":5,"notes":"x","starts_on":"2025-03-04","ends_on":"2025-05-06"}
			""");
	}

	[Fact]
	public async Task UpdateAsync_EmptyPatch_SendsEmptyObject()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Projects.UpdateAsync(9, new ProjectPatchDto(), ct), ProjectJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/projects/9", body: "{}");
	}

	[Fact]
	public async Task DeleteAsync_SendsDelete()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Projects.DeleteAsync(9, ct), "");

		call.ShouldBe(HttpMethod.Delete, "/v2/projects/9");
	}

	private static void ShouldBeTheProject(Project project)
	{
		project.Id.Should().Be(14308069);
		project.Name.Should().Be("Online Store - Phase 1");
		project.Code.Should().Be("OS1");
		project.IsActive.Should().BeTrue();
		project.IsBillable.Should().BeTrue();
		project.IsFixedFee.Should().BeFalse();
		project.BillBy.Should().Be("Project");
		project.HourlyRate.Should().Be(100m);
		project.BudgetBy.Should().Be("project");
		project.BudgetIsMonthly.Should().BeTrue();
		project.Budget.Should().Be(200m);
		project.CostBudget.Should().BeNull();
		project.CostBudgetIncludeExpenses.Should().BeFalse();
		project.NotifyWhenOverBudget.Should().BeTrue();
		project.OverBudgetNotificationPercentage.Should().Be(80m);
		project.OverBudgetNotificationDate.Should().Be(new DateOnly(2017, 7, 1));
		project.ShowBudgetToAll.Should().BeFalse();
		project.Fee.Should().BeNull();
		project.Notes.Should().Be("Notes");
		project.StartsOn.Should().Be(new DateOnly(2017, 6, 1));
		project.EndsOn.Should().BeNull();
		project.CreatedAt.Should().Be(new DateTime(2017, 6, 26, 21, 52, 18, DateTimeKind.Utc));
		project.UpdatedAt.Should().Be(new DateTime(2017, 6, 26, 21, 54, 6, DateTimeKind.Utc));
		project.Client!.Id.Should().Be(5735776);
		project.Client.Name.Should().Be("123 Industries");
		project.Client.Currency.Should().Be("EUR");
	}
}