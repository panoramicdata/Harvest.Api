namespace Harvest.Interfaces;

/// <summary>
/// The Reports API (<see href="https://help.getharvest.com/api-v2/reports-api/reports/time-reports/"/>): time, expense,
/// uninvoiced and project budget reports. Each method returns one page of results.
/// </summary>
/// <remarks>
/// <para>
/// Reports have a much stricter rate limit than the rest of the API: <b>100 requests per 15 minutes</b> (general requests
/// allow 100 per 15 seconds). A throttled request is answered 429 with a <c>Retry-After</c> header, which the client
/// honours when retrying (see <see cref="HarvestClientOptions.MaxRetries"/>), so a burst of report requests can stall for
/// minutes; cache report results rather than polling them.
/// </para>
/// <para>
/// Every report except the project budget report requires a timeframe (<see cref="ReportOptions.From"/> and
/// <see cref="ReportOptions.To"/>, at most one year), sent as <c>YYYYMMDD</c>. What a caller sees depends on their role:
/// Administrators see everything; Managers see their own records plus those of the projects and teammates they manage;
/// Members see only their own.
/// </para>
/// </remarks>
public interface IReportApi
{
	/// <summary>
	/// Gets one page of the time report by client: hours and billable amounts per client and currency
	/// (<see href="https://help.getharvest.com/api-v2/reports-api/reports/time-reports/#clients-report"/>).
	/// </summary>
	/// <param name="options">The required timeframe, plus optional fixed fee inclusion and paging.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of results.</returns>
	[Get("/v2/reports/time/clients")]
	Task<ReportResultsContainer<TimeReportResult>> GetClientTimeReportAsync([Query] TimeReportOptions options, CancellationToken cancellationToken);

	/// <summary>
	/// Gets one page of the time report by project: hours and billable amounts per project and currency, optionally with
	/// Forecast's scheduled hours
	/// (<see href="https://help.getharvest.com/api-v2/reports-api/reports/time-reports/#projects-report"/>).
	/// </summary>
	/// <param name="options">The required timeframe, plus optional fixed fee and Forecast inclusion and paging.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of results.</returns>
	[Get("/v2/reports/time/projects")]
	Task<ReportResultsContainer<TimeReportResult>> GetProjectTimeReportAsync([Query] TimeReportWithForecastOptions options, CancellationToken cancellationToken);

	/// <summary>
	/// Gets one page of the time report by task: hours and billable amounts per task and currency
	/// (<see href="https://help.getharvest.com/api-v2/reports-api/reports/time-reports/#tasks-report"/>).
	/// </summary>
	/// <param name="options">The required timeframe, plus optional fixed fee inclusion and paging.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of results.</returns>
	[Get("/v2/reports/time/tasks")]
	Task<ReportResultsContainer<TimeReportResult>> GetTaskTimeReportAsync([Query] TimeReportOptions options, CancellationToken cancellationToken);

	/// <summary>
	/// Gets one page of the time report by team member: hours and billable amounts per user and currency, optionally with
	/// Forecast's scheduled hours
	/// (<see href="https://help.getharvest.com/api-v2/reports-api/reports/time-reports/#team-report"/>).
	/// </summary>
	/// <param name="options">The required timeframe, plus optional fixed fee and Forecast inclusion and paging.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of results.</returns>
	[Get("/v2/reports/time/team")]
	Task<ReportResultsContainer<TimeReportResult>> GetTeamTimeReportAsync([Query] TimeReportWithForecastOptions options, CancellationToken cancellationToken);

	/// <summary>
	/// Gets one page of the expense report by client: expense totals per client and currency
	/// (<see href="https://help.getharvest.com/api-v2/reports-api/reports/expense-reports/#clients-report"/>).
	/// </summary>
	/// <param name="options">The required timeframe, plus optional paging.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of results.</returns>
	[Get("/v2/reports/expenses/clients")]
	Task<ReportResultsContainer<ExpenseReportResult>> GetClientExpenseReportAsync([Query] ReportOptions options, CancellationToken cancellationToken);

	/// <summary>
	/// Gets one page of the expense report by project: expense totals per project and currency
	/// (<see href="https://help.getharvest.com/api-v2/reports-api/reports/expense-reports/#projects-report"/>).
	/// </summary>
	/// <param name="options">The required timeframe, plus optional paging.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of results.</returns>
	[Get("/v2/reports/expenses/projects")]
	Task<ReportResultsContainer<ExpenseReportResult>> GetProjectExpenseReportAsync([Query] ReportOptions options, CancellationToken cancellationToken);

	/// <summary>
	/// Gets one page of the expense report by expense category: expense totals per expense category and currency
	/// (<see href="https://help.getharvest.com/api-v2/reports-api/reports/expense-reports/#expense-categories-report"/>).
	/// </summary>
	/// <param name="options">The required timeframe, plus optional paging.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of results.</returns>
	[Get("/v2/reports/expenses/categories")]
	Task<ReportResultsContainer<ExpenseReportResult>> GetExpenseCategoryExpenseReportAsync([Query] ReportOptions options, CancellationToken cancellationToken);

	/// <summary>
	/// Gets one page of the expense report by team member: expense totals per user and currency
	/// (<see href="https://help.getharvest.com/api-v2/reports-api/reports/expense-reports/#team-report"/>).
	/// </summary>
	/// <param name="options">The required timeframe, plus optional paging.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of results.</returns>
	[Get("/v2/reports/expenses/team")]
	Task<ReportResultsContainer<ExpenseReportResult>> GetTeamExpenseReportAsync([Query] ReportOptions options, CancellationToken cancellationToken);

	/// <summary>
	/// Gets one page of the uninvoiced report: the uninvoiced hours and expenses of every billable project in the timeframe
	/// (<see href="https://help.getharvest.com/api-v2/reports-api/reports/uninvoiced-report/"/>). Administrators see all
	/// projects; Managers with permission to create invoices see only the projects they manage.
	/// </summary>
	/// <param name="options">The required timeframe, plus optional fixed fee inclusion and paging.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of results.</returns>
	[Get("/v2/reports/uninvoiced")]
	Task<ReportResultsContainer<UninvoicedReportResult>> GetUninvoicedReportAsync([Query] UninvoicedReportOptions options, CancellationToken cancellationToken);

	/// <summary>
	/// Gets one page of the project budget report: budget information for the projects that have been assigned a budget
	/// (<see href="https://help.getharvest.com/api-v2/reports-api/reports/project-budget-report/"/>). It takes no
	/// timeframe.
	/// </summary>
	/// <param name="options">Active filter and paging, or <see langword="null"/> for none.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of results.</returns>
	[Get("/v2/reports/project_budget")]
	Task<ReportResultsContainer<ProjectBudgetReportResult>> GetProjectBudgetReportAsync([Query] ProjectBudgetReportOptions? options, CancellationToken cancellationToken);
}
