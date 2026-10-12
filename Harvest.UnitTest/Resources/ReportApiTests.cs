using Harvest.UnitTest.Support;

namespace Harvest.UnitTest.Resources;

public class ReportApiTests
{
	private static readonly DateOnly _from = new(2017, 1, 1);
	private static readonly DateOnly _to = new(2017, 12, 31);

	// The example responses on https://help.getharvest.com/api-v2/reports-api/reports/time-reports/.
	private const string ClientTimeJson = """
		{"results":[{"client_id":5735776,"client_name":"123 Industries","total_hours":4.5,"billable_hours":3.5,"currency":"EUR","billable_amount":350},
		{"client_id":5735774,"client_name":"ABC Corp","total_hours":2,"billable_hours":2,"currency":"USD","billable_amount":200}],
		"per_page":2000,"total_pages":1,"total_entries":2,"next_page":null,"previous_page":null,"page":1,
		"links":{"first":"https://api.harvestapp.com/v2/reports/time/clients?from=20170101&page=1&per_page=2000&to=20171231","next":null,"previous":null,
		"last":"https://api.harvestapp.com/v2/reports/time/clients?from=20170101&page=1&per_page=2000&to=20171231"}}
		""";

	private const string ProjectTimeJson = """
		{"results":[{"project_id":14307913,"project_name":"[MW] Marketing Website","client_id":5735774,"client_name":"ABC Corp","total_hours":2,"billable_hours":2,"currency":"USD","billable_amount":200,"scheduled_hours":40.0},
		{"project_id":14308069,"project_name":"[OS1] Online Store - Phase 1","client_id":5735776,"client_name":"123 Industries","total_hours":4,"billable_hours":3,"currency":"EUR","billable_amount":300,"scheduled_hours":60.0},
		{"project_id":14808188,"project_name":"[TF] Task Force","client_id":5735776,"client_name":"123 Industries","total_hours":0.5,"billable_hours":0.5,"currency":"EUR","billable_amount":50,"scheduled_hours":null}],
		"per_page":2000,"total_pages":1,"total_entries":3,"next_page":null,"previous_page":null,"page":1,"links":{"first":null,"next":null,"previous":null,"last":null}}
		""";

	private const string TaskTimeJson = """
		{"results":[{"task_id":8083365,"task_name":"Graphic Design","total_hours":2,"billable_hours":2,"currency":"USD","billable_amount":200},
		{"task_id":8083366,"task_name":"Programming","total_hours":1.5,"billable_hours":1.5,"currency":"EUR","billable_amount":150},
		{"task_id":8083368,"task_name":"Project Management","total_hours":1.5,"billable_hours":1.5,"currency":"EUR","billable_amount":150},
		{"task_id":8083368,"task_name":"Project Management","total_hours":0.5,"billable_hours":0.5,"currency":"USD","billable_amount":50},
		{"task_id":8083369,"task_name":"Research","total_hours":1,"billable_hours":0,"currency":"EUR","billable_amount":0}],
		"per_page":2000,"total_pages":1,"total_entries":5,"next_page":null,"previous_page":null,"page":1}
		""";

	private const string TeamTimeJson = """
		{"results":[{"user_id":1795925,"user_name":"Jane Smith","is_contractor":false,"total_hours":0.5,"billable_hours":0.5,"currency":"EUR","billable_amount":50,"weekly_capacity":126000,
		"avatar_url":"https://cache.harvestapp.com/assets/profile_images/abraj_albait_towers.png?1498516481","scheduled_hours":35.0},
		{"user_id":1782959,"user_name":"Kim Allen","is_contractor":false,"total_hours":4,"billable_hours":3,"currency":"EUR","billable_amount":300,"weekly_capacity":126000,
		"avatar_url":"https://cache.harvestapp.com/assets/profile_images/cornell_clock_tower.png?1498515345","scheduled_hours":70.0},
		{"user_id":1782959,"user_name":"Kim Allen","is_contractor":false,"total_hours":2,"billable_hours":2,"currency":"USD","billable_amount":200,"weekly_capacity":126000,
		"avatar_url":"https://cache.harvestapp.com/assets/profile_images/allen_bradley_clock_tower.png?1498509661","scheduled_hours":null}],
		"per_page":2000,"total_pages":1,"total_entries":3,"next_page":null,"previous_page":null,"page":1}
		""";

	// The example responses on https://help.getharvest.com/api-v2/reports-api/reports/expense-reports/.
	private const string ClientExpenseJson = """
		{"results":[{"client_id":5735776,"client_name":"123 Industries","total_amount":100,"billable_amount":100,"currency":"EUR"},
		{"client_id":5735774,"client_name":"ABC Corp","total_amount":133.35,"billable_amount":133.35,"currency":"USD"}],
		"per_page":2000,"total_pages":1,"total_entries":2,"next_page":null,"previous_page":null,"page":1}
		""";

	private const string ProjectExpenseJson = """
		{"results":[{"client_id":5735774,"client_name":"ABC Corp","project_id":14307913,"project_name":"[MW] Marketing Website","total_amount":133.35,"billable_amount":133.35,"currency":"USD"},
		{"client_id":5735776,"client_name":"123 Industries","project_id":14308069,"project_name":"[OS1] Online Store - Phase 1","total_amount":100,"billable_amount":100,"currency":"EUR"}],
		"per_page":2000,"total_pages":1,"total_entries":2,"next_page":null,"previous_page":null,"page":1}
		""";

	private const string CategoryExpenseJson = """
		{"results":[{"expense_category_id":4197501,"expense_category_name":"Lodging","total_amount":100,"billable_amount":100,"currency":"EUR"},
		{"expense_category_id":4195926,"expense_category_name":"Meals","total_amount":100,"billable_amount":100,"currency":"EUR"},
		{"expense_category_id":4195926,"expense_category_name":"Meals","total_amount":33.35,"billable_amount":33.35,"currency":"USD"}],
		"per_page":2000,"total_pages":1,"total_entries":3,"next_page":null,"previous_page":null,"page":1}
		""";

	private const string TeamExpenseJson = """
		{"results":[{"user_id":1782884,"user_name":"Bob Powell","is_contractor":false,"total_amount":100,"billable_amount":100,"currency":"USD"},
		{"user_id":1782959,"user_name":"Kim Allen","is_contractor":false,"total_amount":100,"billable_amount":100,"currency":"EUR"},
		{"user_id":1782959,"user_name":"Kim Allen","is_contractor":false,"total_amount":33.35,"billable_amount":33.35,"currency":"USD"}],
		"per_page":2000,"total_pages":1,"total_entries":3,"next_page":null,"previous_page":null,"page":1}
		""";

	// The example response on https://help.getharvest.com/api-v2/reports-api/reports/uninvoiced-report/.
	private const string UninvoicedJson = """
		{"results":[{"client_id":5735776,"client_name":"123 Industries","project_id":14308069,"project_name":"Online Store - Phase 1","currency":"EUR","total_hours":4,"uninvoiced_hours":0,"uninvoiced_expenses":100,"uninvoiced_amount":100},
		{"client_id":5735776,"client_name":"123 Industries","project_id":14808188,"project_name":"Task Force","currency":"EUR","total_hours":0.5,"uninvoiced_hours":0.5,"uninvoiced_expenses":0,"uninvoiced_amount":50},
		{"client_id":5735774,"client_name":"ABC Corp","project_id":14307913,"project_name":"Marketing Website","currency":"USD","total_hours":2,"uninvoiced_hours":0,"uninvoiced_expenses":0,"uninvoiced_amount":0}],
		"per_page":2000,"total_pages":1,"total_entries":3,"next_page":null,"previous_page":null,"page":1}
		""";

	// The example response on https://help.getharvest.com/api-v2/reports-api/reports/project-budget-report/.
	private const string ProjectBudgetJson = """
		{"results":[{"project_id":14308069,"project_name":"Online Store - Phase 1","client_id":5735776,"client_name":"123 Industries","budget_is_monthly":false,"budget_by":"project","is_active":true,"budget":200,"budget_spent":4,"budget_remaining":196},
		{"project_id":14307913,"project_name":"Marketing Website","client_id":5735774,"client_name":"ABC Corp","budget_is_monthly":false,"budget_by":"project","is_active":true,"budget":50,"budget_spent":2,"budget_remaining":48}],
		"per_page":2000,"total_pages":1,"total_entries":2,"next_page":null,"previous_page":null,"page":1,
		"links":{"first":"https://api.harvestapp.com/v2/reports/project_budget?page=1&per_page=2000","next":null,"previous":null,"last":"https://api.harvestapp.com/v2/reports/project_budget?page=1&per_page=2000"}}
		""";

	// ---- Requests: the timeframe is required and sent as YYYYMMDD, as in Harvest's examples (from=20170101&to=20171231).

	[Fact]
	public async Task GetClientTimeReportAsync_RequiredOnly_SendsTheTimeframeAsYyyyMmDd()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Reports.GetClientTimeReportAsync(new TimeReportOptions { From = _from, To = _to }, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/reports/time/clients", "?from=20170101&to=20171231");
	}

	[Fact]
	public async Task GetClientTimeReportAsync_AllOptions_SendsEveryParameter()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Reports.GetClientTimeReportAsync(
			new TimeReportOptions { From = _from, To = _to, IncludeFixedFee = true, Page = 2, PerPage = 100 }, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/reports/time/clients", "?include_fixed_fee=true&from=20170101&to=20171231&page=2&per_page=100");
	}

	[Fact]
	public async Task GetProjectTimeReportAsync_RequiredOnly_SendsTheTimeframe()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Reports.GetProjectTimeReportAsync(new TimeReportWithForecastOptions { From = _from, To = _to }, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/reports/time/projects", "?from=20170101&to=20171231");
	}

	[Fact]
	public async Task GetProjectTimeReportAsync_AllOptions_SendsEveryParameter()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Reports.GetProjectTimeReportAsync(
			new TimeReportWithForecastOptions { From = _from, To = _to, IncludeFixedFee = false, IncludeForecast = true, Page = 3, PerPage = 50 }, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/reports/time/projects",
			"?include_forecast=true&include_fixed_fee=false&from=20170101&to=20171231&page=3&per_page=50");
	}

	[Fact]
	public async Task GetTaskTimeReportAsync_RequiredOnly_SendsTheTimeframe()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Reports.GetTaskTimeReportAsync(new TimeReportOptions { From = _from, To = _to }, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/reports/time/tasks", "?from=20170101&to=20171231");
	}

	[Fact]
	public async Task GetTaskTimeReportAsync_AllOptions_SendsEveryParameter()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Reports.GetTaskTimeReportAsync(
			new TimeReportOptions { From = _from, To = _to, IncludeFixedFee = true, Page = 1, PerPage = 2000 }, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/reports/time/tasks", "?include_fixed_fee=true&from=20170101&to=20171231&page=1&per_page=2000");
	}

	[Fact]
	public async Task GetTeamTimeReportAsync_RequiredOnly_SendsTheTimeframe()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Reports.GetTeamTimeReportAsync(new TimeReportWithForecastOptions { From = _from, To = _to }, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/reports/time/team", "?from=20170101&to=20171231");
	}

	[Fact]
	public async Task GetTeamTimeReportAsync_AllOptions_SendsEveryParameter()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Reports.GetTeamTimeReportAsync(
			new TimeReportWithForecastOptions { From = _from, To = _to, IncludeFixedFee = true, IncludeForecast = false, Page = 2, PerPage = 10 }, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/reports/time/team",
			"?include_forecast=false&include_fixed_fee=true&from=20170101&to=20171231&page=2&per_page=10");
	}

	[Fact]
	public async Task GetClientExpenseReportAsync_RequiredOnly_SendsTheTimeframe()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Reports.GetClientExpenseReportAsync(new ReportOptions { From = _from, To = _to }, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/reports/expenses/clients", "?from=20170101&to=20171231");
	}

	[Fact]
	public async Task GetClientExpenseReportAsync_AllOptions_SendsEveryParameter()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Reports.GetClientExpenseReportAsync(new ReportOptions { From = _from, To = _to, Page = 2, PerPage = 25 }, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/reports/expenses/clients", "?from=20170101&to=20171231&page=2&per_page=25");
	}

	[Fact]
	public async Task GetProjectExpenseReportAsync_AllOptions_SendsEveryParameter()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Reports.GetProjectExpenseReportAsync(new ReportOptions { From = _from, To = _to, Page = 4, PerPage = 5 }, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/reports/expenses/projects", "?from=20170101&to=20171231&page=4&per_page=5");
	}

	[Fact]
	public async Task GetProjectExpenseReportAsync_RequiredOnly_SendsTheTimeframe()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Reports.GetProjectExpenseReportAsync(new ReportOptions { From = _from, To = _to }, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/reports/expenses/projects", "?from=20170101&to=20171231");
	}

	[Fact]
	public async Task GetExpenseCategoryExpenseReportAsync_RequiredOnly_SendsTheTimeframe()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Reports.GetExpenseCategoryExpenseReportAsync(new ReportOptions { From = _from, To = _to }, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/reports/expenses/categories", "?from=20170101&to=20171231");
	}

	[Fact]
	public async Task GetExpenseCategoryExpenseReportAsync_AllOptions_SendsEveryParameter()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Reports.GetExpenseCategoryExpenseReportAsync(new ReportOptions { From = _from, To = _to, Page = 2, PerPage = 1 }, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/reports/expenses/categories", "?from=20170101&to=20171231&page=2&per_page=1");
	}

	[Fact]
	public async Task GetTeamExpenseReportAsync_RequiredOnly_SendsTheTimeframe()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Reports.GetTeamExpenseReportAsync(new ReportOptions { From = _from, To = _to }, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/reports/expenses/team", "?from=20170101&to=20171231");
	}

	[Fact]
	public async Task GetTeamExpenseReportAsync_AllOptions_SendsEveryParameter()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Reports.GetTeamExpenseReportAsync(new ReportOptions { From = _from, To = _to, Page = 9, PerPage = 99 }, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/reports/expenses/team", "?from=20170101&to=20171231&page=9&per_page=99");
	}

	[Fact]
	public async Task GetUninvoicedReportAsync_RequiredOnly_SendsTheTimeframe()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Reports.GetUninvoicedReportAsync(new UninvoicedReportOptions { From = _from, To = _to }, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/reports/uninvoiced", "?from=20170101&to=20171231");
	}

	[Fact]
	public async Task GetUninvoicedReportAsync_AllOptions_SendsEveryParameter()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Reports.GetUninvoicedReportAsync(
			new UninvoicedReportOptions { From = _from, To = _to, IncludeFixedFee = false, Page = 2, PerPage = 500 }, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/reports/uninvoiced", "?include_fixed_fee=false&from=20170101&to=20171231&page=2&per_page=500");
	}

	[Fact]
	public async Task GetProjectBudgetReportAsync_NoOptions_SendsNoQuery()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Reports.GetProjectBudgetReportAsync(null, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/reports/project_budget");
	}

	[Fact]
	public async Task GetProjectBudgetReportAsync_AllOptions_SendsEveryParameter()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Reports.GetProjectBudgetReportAsync(
			new ProjectBudgetReportOptions { IsActive = true, Page = 2, PerPage = 100 }, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/reports/project_budget", "?is_active=true&page=2&per_page=100");
	}

	[Fact]
	public async Task ReportDates_AreZeroPaddedYyyyMmDd()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Reports.GetClientExpenseReportAsync(
			new ReportOptions { From = new DateOnly(2025, 3, 4), To = new DateOnly(2025, 11, 9) }, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/reports/expenses/clients", "?from=20250304&to=20251109");
	}

	// ---- Responses

	[Fact]
	public async Task GetClientTimeReportAsync_MapsTheResultsAndPaging()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.Reports.GetClientTimeReportAsync(new TimeReportOptions { From = _from, To = _to }, ct), ClientTimeJson);

		page.Results.Should().HaveCount(2);
		var result = page.Results[0];
		result.ClientId.Should().Be(5735776);
		result.ClientName.Should().Be("123 Industries");
		result.TotalHours.Should().Be(4.5m);
		result.BillableHours.Should().Be(3.5m);
		result.Currency.Should().Be("EUR");
		result.BillableAmount.Should().Be(350m);
		result.ProjectId.Should().BeNull();
		result.ProjectName.Should().BeNull();
		result.TaskId.Should().BeNull();
		result.TaskName.Should().BeNull();
		result.UserId.Should().BeNull();
		result.UserName.Should().BeNull();
		result.WeeklyCapacity.Should().BeNull();
		result.AvatarUrl.Should().BeNull();
		result.IsContractor.Should().BeNull();
		result.ScheduledHours.Should().BeNull();
		page.Results[1].ClientName.Should().Be("ABC Corp");

		page.PerPage.Should().Be(2000);
		page.TotalPages.Should().Be(1);
		page.TotalEntries.Should().Be(2);
		page.NextPage.Should().BeNull();
		page.PreviousPage.Should().BeNull();
		page.Page.Should().Be(1);
		page.Links!.First.Should().Be("https://api.harvestapp.com/v2/reports/time/clients?from=20170101&page=1&per_page=2000&to=20171231");
		page.Links.Last.Should().Be("https://api.harvestapp.com/v2/reports/time/clients?from=20170101&page=1&per_page=2000&to=20171231");
		page.Links.Next.Should().BeNull();
		page.Links.Previous.Should().BeNull();
		page.Links.NextCursor.Should().BeNull();
	}

	[Fact]
	public async Task GetProjectTimeReportAsync_MapsTheResultsWithScheduledHours()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.Reports.GetProjectTimeReportAsync(
			new TimeReportWithForecastOptions { From = _from, To = _to, IncludeForecast = true }, ct), ProjectTimeJson);

		page.Results.Should().HaveCount(3);
		var result = page.Results[0];
		result.ProjectId.Should().Be(14307913);
		result.ProjectName.Should().Be("[MW] Marketing Website");
		result.ClientId.Should().Be(5735774);
		result.ClientName.Should().Be("ABC Corp");
		result.TotalHours.Should().Be(2m);
		result.BillableHours.Should().Be(2m);
		result.Currency.Should().Be("USD");
		result.BillableAmount.Should().Be(200m);
		result.ScheduledHours.Should().Be(40m);
		page.Results[2].ScheduledHours.Should().BeNull();
		page.Results[2].TotalHours.Should().Be(0.5m);
		page.TotalEntries.Should().Be(3);
	}

	[Fact]
	public async Task GetTaskTimeReportAsync_MapsTheResults()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.Reports.GetTaskTimeReportAsync(new TimeReportOptions { From = _from, To = _to }, ct), TaskTimeJson);

		page.Results.Should().HaveCount(5);
		var result = page.Results[1];
		result.TaskId.Should().Be(8083366);
		result.TaskName.Should().Be("Programming");
		result.TotalHours.Should().Be(1.5m);
		result.BillableHours.Should().Be(1.5m);
		result.Currency.Should().Be("EUR");
		result.BillableAmount.Should().Be(150m);
		result.ClientId.Should().BeNull();
		page.Results[4].BillableHours.Should().Be(0m);
		page.Results[4].BillableAmount.Should().Be(0m);
		page.Links.Should().BeNull();
	}

	[Fact]
	public async Task GetTeamTimeReportAsync_MapsTheResults()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.Reports.GetTeamTimeReportAsync(
			new TimeReportWithForecastOptions { From = _from, To = _to, IncludeForecast = true }, ct), TeamTimeJson);

		page.Results.Should().HaveCount(3);
		var result = page.Results[0];
		result.UserId.Should().Be(1795925);
		result.UserName.Should().Be("Jane Smith");
		result.IsContractor.Should().BeFalse();
		result.TotalHours.Should().Be(0.5m);
		result.BillableHours.Should().Be(0.5m);
		result.Currency.Should().Be("EUR");
		result.BillableAmount.Should().Be(50m);
		result.WeeklyCapacity.Should().Be(126000);
		result.AvatarUrl.Should().Be("https://cache.harvestapp.com/assets/profile_images/abraj_albait_towers.png?1498516481");
		result.ScheduledHours.Should().Be(35m);
		page.Results[2].ScheduledHours.Should().BeNull();
	}

	[Fact]
	public async Task GetClientExpenseReportAsync_MapsTheResults()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.Reports.GetClientExpenseReportAsync(new ReportOptions { From = _from, To = _to }, ct), ClientExpenseJson);

		page.Results.Should().HaveCount(2);
		var result = page.Results[1];
		result.ClientId.Should().Be(5735774);
		result.ClientName.Should().Be("ABC Corp");
		result.TotalAmount.Should().Be(133.35m);
		result.BillableAmount.Should().Be(133.35m);
		result.Currency.Should().Be("USD");
		result.ProjectId.Should().BeNull();
		result.ProjectName.Should().BeNull();
		result.ExpenseCategoryId.Should().BeNull();
		result.ExpenseCategoryName.Should().BeNull();
		result.UserId.Should().BeNull();
		result.UserName.Should().BeNull();
		result.IsContractor.Should().BeNull();
		page.TotalEntries.Should().Be(2);
	}

	[Fact]
	public async Task GetProjectExpenseReportAsync_MapsTheResults()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.Reports.GetProjectExpenseReportAsync(new ReportOptions { From = _from, To = _to }, ct), ProjectExpenseJson);

		page.Results.Should().HaveCount(2);
		var result = page.Results[0];
		result.ClientId.Should().Be(5735774);
		result.ClientName.Should().Be("ABC Corp");
		result.ProjectId.Should().Be(14307913);
		result.ProjectName.Should().Be("[MW] Marketing Website");
		result.TotalAmount.Should().Be(133.35m);
		result.BillableAmount.Should().Be(133.35m);
		result.Currency.Should().Be("USD");
	}

	[Fact]
	public async Task GetExpenseCategoryExpenseReportAsync_MapsTheResults()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.Reports.GetExpenseCategoryExpenseReportAsync(new ReportOptions { From = _from, To = _to }, ct), CategoryExpenseJson);

		page.Results.Should().HaveCount(3);
		var result = page.Results[0];
		result.ExpenseCategoryId.Should().Be(4197501);
		result.ExpenseCategoryName.Should().Be("Lodging");
		result.TotalAmount.Should().Be(100m);
		result.BillableAmount.Should().Be(100m);
		result.Currency.Should().Be("EUR");
		page.Results[2].Currency.Should().Be("USD");
	}

	[Fact]
	public async Task GetTeamExpenseReportAsync_MapsTheResults()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.Reports.GetTeamExpenseReportAsync(new ReportOptions { From = _from, To = _to }, ct), TeamExpenseJson);

		page.Results.Should().HaveCount(3);
		var result = page.Results[0];
		result.UserId.Should().Be(1782884);
		result.UserName.Should().Be("Bob Powell");
		result.IsContractor.Should().BeFalse();
		result.TotalAmount.Should().Be(100m);
		result.BillableAmount.Should().Be(100m);
		result.Currency.Should().Be("USD");
	}

	[Fact]
	public async Task GetUninvoicedReportAsync_MapsTheResults()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.Reports.GetUninvoicedReportAsync(new UninvoicedReportOptions { From = _from, To = _to }, ct), UninvoicedJson);

		page.Results.Should().HaveCount(3);
		var result = page.Results[1];
		result.ClientId.Should().Be(5735776);
		result.ClientName.Should().Be("123 Industries");
		result.ProjectId.Should().Be(14808188);
		result.ProjectName.Should().Be("Task Force");
		result.Currency.Should().Be("EUR");
		result.TotalHours.Should().Be(0.5m);
		result.UninvoicedHours.Should().Be(0.5m);
		result.UninvoicedExpenses.Should().Be(0m);
		result.UninvoicedAmount.Should().Be(50m);
		page.Results[0].UninvoicedExpenses.Should().Be(100m);
	}

	[Fact]
	public async Task GetProjectBudgetReportAsync_MapsTheResultsAndPaging()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.Reports.GetProjectBudgetReportAsync(null, ct), ProjectBudgetJson);

		page.Results.Should().HaveCount(2);
		var result = page.Results[0];
		result.ProjectId.Should().Be(14308069);
		result.ProjectName.Should().Be("Online Store - Phase 1");
		result.ClientId.Should().Be(5735776);
		result.ClientName.Should().Be("123 Industries");
		result.BudgetIsMonthly.Should().BeFalse();
		result.BudgetBy.Should().Be("project");
		result.IsActive.Should().BeTrue();
		result.Budget.Should().Be(200m);
		result.BudgetSpent.Should().Be(4m);
		result.BudgetRemaining.Should().Be(196m);
		page.Links!.First.Should().Be("https://api.harvestapp.com/v2/reports/project_budget?page=1&per_page=2000");
		page.TotalEntries.Should().Be(2);
	}

	[Fact]
	public async Task GetProjectBudgetReportAsync_ToleratesHiddenMoneyFields()
	{
		// Money budgets are hidden from users without the View billable rates and amounts permission.
		var page = await TestClient.ReadAsync((c, ct) => c.Reports.GetProjectBudgetReportAsync(null, ct),
			"""{"results":[{"project_id":1,"project_name":"P","client_id":2,"client_name":"C","budget_is_monthly":true,"budget_by":"project_cost","is_active":false,"budget":null,"budget_spent":null,"budget_remaining":null}]}""");

		var result = page.Results.Should().ContainSingle().Subject;
		result.BudgetIsMonthly.Should().BeTrue();
		result.BudgetBy.Should().Be("project_cost");
		result.IsActive.Should().BeFalse();
		result.Budget.Should().BeNull();
		result.BudgetSpent.Should().BeNull();
		result.BudgetRemaining.Should().BeNull();
	}

	[Fact]
	public async Task GetClientTimeReportAsync_ToleratesHiddenBillableFields()
	{
		// Currency and billable amount are only visible to users with the View billable rates and amounts permission.
		var page = await TestClient.ReadAsync((c, ct) => c.Reports.GetClientTimeReportAsync(new TimeReportOptions { From = _from, To = _to }, ct),
			"""{"results":[{"client_id":1,"client_name":"C","total_hours":1.25,"billable_hours":1}]}""");

		var result = page.Results.Should().ContainSingle().Subject;
		result.TotalHours.Should().Be(1.25m);
		result.Currency.Should().BeNull();
		result.BillableAmount.Should().BeNull();
	}

	[Fact]
	public async Task EmptyReport_HasNoResults()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.Reports.GetUninvoicedReportAsync(new UninvoicedReportOptions { From = _from, To = _to }, ct), "{}");

		page.Results.Should().BeEmpty();
	}
}