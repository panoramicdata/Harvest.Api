namespace Harvest.Models;

/// <summary>
/// A page of a Harvest report: the results (<c>results</c>) plus the usual page-numbered paging fields. Reports are not
/// cursor-paged; pass <see cref="ListContainerBase.NextPage"/> as the options' <c>Page</c> for the next page.
/// </summary>
/// <typeparam name="TResult">The report's result type.</typeparam>
public class ReportResultsContainer<TResult> : ListContainerBase
{
	/// <summary>The results on this page (<c>results</c>).</summary>
	public List<TResult> Results { get; set; } = [];
}

/// <summary>
/// One result of a time report (<see href="https://help.getharvest.com/api-v2/reports-api/reports/time-reports/"/>): the
/// hours and billable information for one client, project, task or user, in one currency, where tracked time is present
/// in the timeframe. Which subject fields are set depends on the report.
/// </summary>
public class TimeReportResult
{
	/// <summary>The ID of the client associated with the reported hours. Only returned in the Client and Project reports.</summary>
	public long? ClientId { get; set; }

	/// <summary>The name of the client associated with the reported hours. Only returned in the Client and Project reports.</summary>
	public string? ClientName { get; set; }

	/// <summary>The ID of the project associated with the reported hours. Only returned in the Client and Project reports.</summary>
	public long? ProjectId { get; set; }

	/// <summary>The name of the project associated with the reported hours. Only returned in the Client and Project reports.</summary>
	public string? ProjectName { get; set; }

	/// <summary>The ID of the task associated with the reported hours. Only returned in the Task report.</summary>
	public long? TaskId { get; set; }

	/// <summary>The name of the task associated with the reported hours. Only returned in the Task report.</summary>
	public string? TaskName { get; set; }

	/// <summary>The ID of the user associated with the reported hours. Only returned in the Team report.</summary>
	public long? UserId { get; set; }

	/// <summary>The name of the user associated with the reported hours. Only returned in the Team report.</summary>
	public string? UserName { get; set; }

	/// <summary>
	/// The number of hours per week this person is available to work, in seconds, in half hour increments (35 hours is
	/// 126000). Only returned in the Team report.
	/// </summary>
	public int? WeeklyCapacity { get; set; }

	/// <summary>The URL to the user's avatar image. Only returned in the Team report.</summary>
	public string? AvatarUrl { get; set; }

	/// <summary>The contractor status of the user associated with the reported hours. Only returned in the Team report.</summary>
	public bool? IsContractor { get; set; }

	/// <summary>
	/// The totaled hours for the timeframe, subject and currency, rounded according to the account's settings if Time
	/// Rounding is turned on.
	/// </summary>
	public decimal TotalHours { get; set; }

	/// <summary>
	/// The totaled billable hours for the timeframe, subject and currency, rounded according to the account's settings if
	/// Time Rounding is turned on.
	/// </summary>
	public decimal BillableHours { get; set; }

	/// <summary>
	/// The currency code associated with the tracked hours for this result. Only visible to Administrators and Project
	/// Managers with the View billable rates and amounts permission.
	/// </summary>
	public string? Currency { get; set; }

	/// <summary>
	/// The totaled billable amount for <see cref="BillableHours"/>. Only visible to Administrators and Project Managers with
	/// the View billable rates and amounts permission.
	/// </summary>
	public decimal? BillableAmount { get; set; }

	/// <summary>
	/// The total scheduled hours from Forecast for the timeframe. Only present in the Project and Team reports when
	/// <see cref="TimeReportWithForecastOptions.IncludeForecast"/> is <see langword="true"/> and the account is connected to
	/// Forecast; <see langword="null"/> if the project or user has no Forecast assignments.
	/// </summary>
	public decimal? ScheduledHours { get; set; }
}

/// <summary>
/// One result of an expense report (<see href="https://help.getharvest.com/api-v2/reports-api/reports/expense-reports/"/>):
/// the expense totals for one client, project, expense category or user, in one currency, where expenses are present in
/// the timeframe. Which subject fields are set depends on the report.
/// </summary>
public class ExpenseReportResult
{
	/// <summary>The ID of the client associated with the reported expenses. Only returned in the Client and Project reports.</summary>
	public long? ClientId { get; set; }

	/// <summary>The name of the client associated with the reported expenses. Only returned in the Client and Project reports.</summary>
	public string? ClientName { get; set; }

	/// <summary>The ID of the project associated with the reported expenses. Only returned in the Client and Project reports.</summary>
	public long? ProjectId { get; set; }

	/// <summary>The name of the project associated with the reported expenses. Only returned in the Client and Project reports.</summary>
	public string? ProjectName { get; set; }

	/// <summary>The ID of the expense category associated with the reported expenses. Only returned in the Expense Category report.</summary>
	public long? ExpenseCategoryId { get; set; }

	/// <summary>The name of the expense category associated with the reported expenses. Only returned in the Expense Category report.</summary>
	public string? ExpenseCategoryName { get; set; }

	/// <summary>The ID of the user associated with the reported expenses. Only returned in the Team report.</summary>
	public long? UserId { get; set; }

	/// <summary>The name of the user associated with the reported expenses. Only returned in the Team report.</summary>
	public string? UserName { get; set; }

	/// <summary>The contractor status of the user associated with the reported expenses. Only returned in the Team report.</summary>
	public bool? IsContractor { get; set; }

	/// <summary>The totaled cost for all expenses for the timeframe, subject and currency.</summary>
	public decimal TotalAmount { get; set; }

	/// <summary>The totaled cost for billable expenses for the timeframe, subject and currency.</summary>
	public decimal BillableAmount { get; set; }

	/// <summary>The currency code associated with the expenses for this result.</summary>
	public string? Currency { get; set; }
}

/// <summary>
/// One result of the uninvoiced report
/// (<see href="https://help.getharvest.com/api-v2/reports-api/reports/uninvoiced-report/"/>): the uninvoiced hours and
/// expenses of one billable project in the timeframe.
/// </summary>
public class UninvoicedReportResult
{
	/// <summary>The ID of the client associated with the reported hours and expenses.</summary>
	public long ClientId { get; set; }

	/// <summary>The name of the client associated with the reported hours and expenses.</summary>
	public string? ClientName { get; set; }

	/// <summary>The ID of the project associated with the reported hours and expenses.</summary>
	public long ProjectId { get; set; }

	/// <summary>The name of the project associated with the reported hours and expenses.</summary>
	public string? ProjectName { get; set; }

	/// <summary>The currency code associated with the tracked hours for this result.</summary>
	public string? Currency { get; set; }

	/// <summary>
	/// The total hours for the timeframe and project, rounded according to the account's settings if Time Rounding is
	/// turned on.
	/// </summary>
	public decimal TotalHours { get; set; }

	/// <summary>
	/// The total hours for the timeframe and project that have not been invoiced, rounded according to the account's
	/// settings if Time Rounding is turned on.
	/// </summary>
	public decimal UninvoicedHours { get; set; }

	/// <summary>The total amount for billable expenses for the timeframe and project that have not been invoiced.</summary>
	public decimal UninvoicedExpenses { get; set; }

	/// <summary>The total amount (time and expenses) for the timeframe and project that has not been invoiced.</summary>
	public decimal UninvoicedAmount { get; set; }
}

/// <summary>
/// One result of the project budget report
/// (<see href="https://help.getharvest.com/api-v2/reports-api/reports/project-budget-report/"/>): the budget of one
/// project that has been assigned a budget.
/// </summary>
public class ProjectBudgetReportResult
{
	/// <summary>The ID of the client associated with this project.</summary>
	public long ClientId { get; set; }

	/// <summary>The name of the client associated with this project.</summary>
	public string? ClientName { get; set; }

	/// <summary>The ID of the project.</summary>
	public long ProjectId { get; set; }

	/// <summary>The name of the project.</summary>
	public string? ProjectName { get; set; }

	/// <summary>Whether the budget is reset every month.</summary>
	public bool BudgetIsMonthly { get; set; }

	/// <summary>
	/// The method by which the project is budgeted: <c>project</c> (Hours Per Project), <c>project_cost</c> (Total Project
	/// Fees), <c>task</c> (Hours Per Task), <c>task_fees</c> (Fees Per Task), <c>person</c> (Hours Per Person) or
	/// <c>none</c> (No Budget).
	/// </summary>
	public string? BudgetBy { get; set; }

	/// <summary>Whether the project is active or archived.</summary>
	public bool IsActive { get; set; }

	/// <summary>
	/// The budget in hours or money for the project. If the project is budgeted by money, only visible to Administrators
	/// and Project Managers with the View billable rates and amounts permission.
	/// </summary>
	public decimal? Budget { get; set; }

	/// <summary>
	/// The total hours or money spent against the project's budget, rounded according to the account's settings if Time
	/// Rounding is turned on. If the project is budgeted by money, only visible to Administrators and Project Managers with
	/// the View billable rates and amounts permission.
	/// </summary>
	public decimal? BudgetSpent { get; set; }

	/// <summary>
	/// The total hours or money remaining in the project's budget, rounded according to the account's settings if Time
	/// Rounding is turned on. If the project is budgeted by money, only visible to Administrators and Project Managers with
	/// the View billable rates and amounts permission.
	/// </summary>
	public decimal? BudgetRemaining { get; set; }
}
