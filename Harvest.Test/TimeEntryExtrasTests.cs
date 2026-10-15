namespace Harvest.Test;

public class TimeEntryExtrasTests(ITestOutputHelper testOutputHelper) : HarvestTest(testOutputHelper)
{
	[Fact]
	public System.Threading.Tasks.Task ExternalReferenceCanBeSetAndDeleted()
		=> RunStrictScenarioAsync(async () =>
		{
			var (me, project, task) = await CreateFixtureAsync("ExternalReference");

			var created = await HarvestClient.TimeEntries.CreateAsync(new TimeEntryCreationDto
			{
				UserId = me.Id,
				ProjectId = project.Id,
				TaskId = task.Id,
				SpentDate = DateOnly.FromDateTime(DateTime.UtcNow),
				Hours = 1m,
				Notes = $"{Prefix}External reference",
				ExternalReference = new ExternalReferenceCreationDto
				{
					Id = "TEST-123",
					GroupId = "TEST-GROUP",
					AccountId = "TEST-ACCOUNT",
					Permalink = "https://example.invalid/tasks/123"
				}
			}, CancellationToken);
			Track("time entry", created.Id, created.Notes, id => HarvestClient.TimeEntries.DeleteAsync(id, CancellationToken));
			created.ExternalReference.Should().NotBeNull();
			created.ExternalReference!.Id.Should().Be("TEST-123");
			created.ExternalReference.Permalink.Should().Be("https://example.invalid/tasks/123");

			await HarvestClient.TimeEntries.DeleteExternalReferenceAsync(created.Id, CancellationToken);

			var retrieved = await HarvestClient.TimeEntries.GetAsync(created.Id, CancellationToken);
			retrieved.ExternalReference.Should().BeNull();

			await DeleteTrackedAsync("time entry", created.Id);
		});

	[Fact]
	public System.Threading.Tasks.Task TimerCanBeStoppedAndRestarted()
		=> RunStrictScenarioAsync(async () =>
		{
			var (me, project, task) = await CreateFixtureAsync("Timer");

			// No hours: Harvest starts a running timer.
			var running = await HarvestClient.TimeEntries.CreateAsync(new TimeEntryCreationDto
			{
				UserId = me.Id,
				ProjectId = project.Id,
				TaskId = task.Id,
				SpentDate = DateOnly.FromDateTime(DateTime.UtcNow),
				Notes = $"{Prefix}Timer"
			}, CancellationToken);
			Track("time entry", running.Id, running.Notes, id => HarvestClient.TimeEntries.DeleteAsync(id, CancellationToken));
			running.IsRunning.Should().BeTrue();

			var stopped = await HarvestClient.TimeEntries.StopAsync(running.Id, CancellationToken);
			stopped.IsRunning.Should().BeFalse();

			var restarted = await HarvestClient.TimeEntries.RestartAsync(running.Id, CancellationToken);
			restarted.IsRunning.Should().BeTrue();

			var stoppedAgain = await HarvestClient.TimeEntries.StopAsync(running.Id, CancellationToken);
			stoppedAgain.IsRunning.Should().BeFalse();

			await DeleteTrackedAsync("time entry", running.Id);
		});

	[Fact]
	public System.Threading.Tasks.Task ReportsCountTimeAndExpenses()
		=> RunStrictScenarioAsync(async () =>
		{
			var (me, project, task) = await CreateFixtureAsync("Report");
			var today = DateOnly.FromDateTime(DateTime.UtcNow);

			var entry = await HarvestClient.TimeEntries.CreateAsync(new TimeEntryCreationDto
			{
				UserId = me.Id,
				ProjectId = project.Id,
				TaskId = task.Id,
				SpentDate = today,
				Hours = 1.5m,
				Notes = $"{Prefix}Report"
			}, CancellationToken);
			Track("time entry", entry.Id, entry.Notes, id => HarvestClient.TimeEntries.DeleteAsync(id, CancellationToken));

			var category = await HarvestClient.ExpenseCategories.CreateAsync(new ExpenseCategoryCreationDto { Name = TestName("Report", "Expense Category") }, CancellationToken);
			Track("expense category", category.Id, category.Name, id => HarvestClient.ExpenseCategories.DeleteAsync(id, CancellationToken));
			var expense = await HarvestClient.Expenses.CreateAsync(new ExpenseCreationDto
			{
				ProjectId = project.Id,
				ExpenseCategoryId = category.Id,
				SpentDate = today,
				TotalCost = 9m,
				Notes = $"{Prefix}Report"
			}, CancellationToken);
			Track("expense", expense.Id, expense.Notes, id => HarvestClient.Expenses.DeleteAsync(id, CancellationToken));

			var timeReport = await HarvestClient.Reports.GetProjectTimeReportAsync(new TimeReportWithForecastOptions { From = today, To = today }, CancellationToken);
			var projectRow = timeReport.Results.Single(row => row.ProjectId == project.Id);
			projectRow.TotalHours.Should().Be(1.5m);
			projectRow.ProjectName.Should().Be(project.Name);

			var expenseReport = await HarvestClient.Reports.GetProjectExpenseReportAsync(new ReportOptions { From = today, To = today }, CancellationToken);
			var expenseRow = expenseReport.Results.Single(row => row.ProjectId == project.Id);
			expenseRow.TotalAmount.Should().Be(9m);

			var uninvoiced = await HarvestClient.Reports.GetUninvoicedReportAsync(new UninvoicedReportOptions { From = today, To = today }, CancellationToken);
			uninvoiced.Results.Should().Contain(row => row.ProjectId == project.Id);
		});

	/// <summary>The current user plus a prefixed client, project, task and task assignment, all tracked.</summary>
	private async Task<(User Me, Project Project, Models.Task Task)> CreateFixtureAsync(string purpose)
	{
		var me = await HarvestClient.Users.GetMeAsync(CancellationToken);
		var client = await CreateTestClientAsync(purpose, address: null);
		var project = await CreateTestProjectAsync(client.Id, purpose, notes: null);
		var task = await CreateTestTaskAsync(purpose, defaultHourlyRate: 75m);
		_ = await CreateTestTaskAssignmentAsync(project.Id, task.Id, hourlyRate: 75m);
		return (me, project, task);
	}
}