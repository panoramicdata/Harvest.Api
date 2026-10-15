using System.Net;

namespace Harvest.Test;

/// <summary>Read-only live checks: reports (rate limited to 100 requests per 15 minutes), rates, teammates, accounts, company, estimates.</summary>
public class ReadOnlyApiTests(ITestOutputHelper testOutputHelper) : HarvestTest(testOutputHelper)
{
	[Fact]
	public async System.Threading.Tasks.Task AllReports()
	{
		// Ten requests in all; the Reports API allows only 100 per 15 minutes.
		var from = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-6));
		var to = DateOnly.FromDateTime(DateTime.UtcNow);
		var reports = HarvestClient.Reports;

		(await reports.GetClientTimeReportAsync(new TimeReportOptions { From = from, To = to, IncludeFixedFee = true }, CancellationToken)).Results.Should().NotBeNull();
		(await reports.GetProjectTimeReportAsync(new TimeReportWithForecastOptions { From = from, To = to, IncludeForecast = true }, CancellationToken)).Results.Should().NotBeNull();
		(await reports.GetTaskTimeReportAsync(new TimeReportOptions { From = from, To = to }, CancellationToken)).Results.Should().NotBeNull();
		(await reports.GetTeamTimeReportAsync(new TimeReportWithForecastOptions { From = from, To = to }, CancellationToken)).Results.Should().NotBeNull();
		(await reports.GetClientExpenseReportAsync(new ReportOptions { From = from, To = to }, CancellationToken)).Results.Should().NotBeNull();
		(await reports.GetProjectExpenseReportAsync(new ReportOptions { From = from, To = to }, CancellationToken)).Results.Should().NotBeNull();
		(await reports.GetExpenseCategoryExpenseReportAsync(new ReportOptions { From = from, To = to }, CancellationToken)).Results.Should().NotBeNull();
		(await reports.GetTeamExpenseReportAsync(new ReportOptions { From = from, To = to }, CancellationToken)).Results.Should().NotBeNull();
		(await reports.GetUninvoicedReportAsync(new UninvoicedReportOptions { From = from, To = to }, CancellationToken)).Results.Should().NotBeNull();
		(await reports.GetProjectBudgetReportAsync(new ProjectBudgetReportOptions { IsActive = true, PerPage = 10 }, CancellationToken)).Results.Should().NotBeNull();
	}

	[Fact]
	public async System.Threading.Tasks.Task RatesAreReadable()
	{
		// Rates cannot be deleted once created, so only the read operations are exercised.
		var me = await HarvestClient.Users.GetMeAsync(CancellationToken);

		var billable = await HarvestClient.BillableRates.ListAsync(me.Id, new() { PerPage = 10 }, CancellationToken);
		billable.BillableRates.Should().NotBeNull();
		if (billable.BillableRates.Count > 0)
		{
			var one = await HarvestClient.BillableRates.GetAsync(me.Id, billable.BillableRates[0].Id, CancellationToken);
			one.Id.Should().Be(billable.BillableRates[0].Id);
		}

		var cost = await HarvestClient.CostRates.ListAsync(me.Id, new() { PerPage = 10 }, CancellationToken);
		cost.CostRates.Should().NotBeNull();
		if (cost.CostRates.Count > 0)
		{
			var one = await HarvestClient.CostRates.GetAsync(me.Id, cost.CostRates[0].Id, CancellationToken);
			one.Id.Should().Be(cost.CostRates[0].Id);
		}
	}

	[Fact]
	public async System.Threading.Tasks.Task TeammatesAreReadable()
	{
		var me = await HarvestClient.Users.GetMeAsync(CancellationToken);

		var teammates = await HarvestClient.Teammates.ListAsync(me.Id, new() { PerPage = 10 }, CancellationToken);

		teammates.Teammates.Should().NotBeNull();
	}

	[Fact]
	public async System.Threading.Tasks.Task AccountsAreListed()
	{
		var response = await HarvestClient.Accounts.ListAsync(CancellationToken);

		response.Accounts.Should().Contain(account => account.Id == Configuration.AccountId);
		response.User.Should().NotBeNull();
	}

	[Fact]
	public async System.Threading.Tasks.Task CompanyIsReadable()
	{
		var company = await HarvestClient.Companies.GetAsync(CancellationToken);

		company.Name.Should().NotBeNullOrEmpty();
		company.WeeklyCapacity.Should().BePositive();
	}

	[Fact]
	public async System.Threading.Tasks.Task EstimatesAreNotOnThePlan()
	{
		var thrown = (await FluentActions.Awaiting(() => HarvestClient.Estimates.ListAsync(options: null, CancellationToken)).Should().ThrowAsync<HarvestApiException>()).Which;

		thrown.StatusCode.Should().Be(HttpStatusCode.Forbidden);
		thrown.Message.Should().NotBeNullOrEmpty();
	}

	[Fact]
	public async System.Threading.Tasks.Task EstimateItemCategoriesAreNotOnThePlan()
	{
		var thrown = (await FluentActions.Awaiting(() => HarvestClient.EstimateItemCategories.ListAsync(options: null, CancellationToken)).Should().ThrowAsync<HarvestApiException>()).Which;

		thrown.StatusCode.Should().Be(HttpStatusCode.Forbidden);
	}
}