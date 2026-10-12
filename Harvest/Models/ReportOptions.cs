namespace Harvest.Models;

/// <summary>
/// The timeframe and paging of a Harvest report that requires a timeframe: the expense reports
/// (<c>GET /v2/reports/expenses/*</c>), and the base of <see cref="TimeReportOptions"/> and
/// <see cref="UninvoicedReportOptions"/>.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="From"/> and <see cref="To"/> are required by Harvest and are sent in the compact <c>YYYYMMDD</c> form the
/// Reports API documents (<c>from=20170101&amp;to=20171231</c>). The timeframe cannot exceed one year (365 days).
/// </para>
/// <para>
/// Reports are paged by page number only (no cursor): pass <see cref="ListContainerBase.NextPage"/> as
/// <see cref="Page"/> until it is <see langword="null"/>.
/// </para>
/// </remarks>
public class ReportOptions
{
	/// <summary>The <c>YYYYMMDD</c> format the Reports API documents for <c>from</c> and <c>to</c>.</summary>
	internal const string DateFormat = "yyyyMMdd";

	/// <summary>Only report on records with a <c>spent_date</c> on or after this date (<c>from</c>, sent as <c>YYYYMMDD</c>).</summary>
	[AliasAs("from")]
	[Query(Format = DateFormat)]
	public required DateOnly From { get; init; }

	/// <summary>Only report on records with a <c>spent_date</c> on or before this date (<c>to</c>, sent as <c>YYYYMMDD</c>).</summary>
	[AliasAs("to")]
	[Query(Format = DateFormat)]
	public required DateOnly To { get; init; }

	/// <summary>The page to return (<c>page</c>); Harvest's default is 1.</summary>
	[AliasAs("page")]
	public int? Page { get; init; }

	/// <summary>The number of results per page, from 1 to 2000 (<c>per_page</c>); Harvest's default is 2000.</summary>
	[AliasAs("per_page")]
	public int? PerPage { get; init; }
}

/// <summary>
/// The options of the time reports by client and by task (<c>GET /v2/reports/time/clients</c>,
/// <c>GET /v2/reports/time/tasks</c>).
/// </summary>
public class TimeReportOptions : ReportOptions
{
	/// <summary>
	/// When <see langword="true"/>, billable amounts are calculated and included for fixed fee projects
	/// (<c>include_fixed_fee</c>).
	/// </summary>
	[AliasAs("include_fixed_fee")]
	public bool? IncludeFixedFee { get; init; }
}

/// <summary>
/// The options of the time reports by project and by team member (<c>GET /v2/reports/time/projects</c>,
/// <c>GET /v2/reports/time/team</c>), which can also include Forecast's scheduled hours.
/// </summary>
public class TimeReportWithForecastOptions : TimeReportOptions
{
	/// <summary>
	/// When <see langword="true"/>, scheduled hours from Forecast are included in the response as
	/// <see cref="TimeReportResult.ScheduledHours"/> (<c>include_forecast</c>). Requires the account to be connected to
	/// Forecast; otherwise the field is absent.
	/// </summary>
	[AliasAs("include_forecast")]
	public bool? IncludeForecast { get; init; }
}

/// <summary>The options of the uninvoiced report (<c>GET /v2/reports/uninvoiced</c>).</summary>
public class UninvoicedReportOptions : ReportOptions
{
	/// <summary>
	/// Whether to include fixed-fee projects (<c>include_fixed_fee</c>); Harvest's default is <see langword="true"/>. A
	/// fixed-fee project's uninvoiced fee shows as long as the timeframe is on or after the project's start date (or its
	/// creation date, if it has no start date); otherwise it is 0.
	/// </summary>
	[AliasAs("include_fixed_fee")]
	public bool? IncludeFixedFee { get; init; }
}

/// <summary>
/// The options of the project budget report (<c>GET /v2/reports/project_budget</c>), which takes no timeframe. Leave a
/// property <see langword="null"/> for Harvest's default.
/// </summary>
public class ProjectBudgetReportOptions
{
	/// <summary>
	/// <see langword="true"/> to only return active projects, <see langword="false"/> to return inactive projects
	/// (<c>is_active</c>).
	/// </summary>
	[AliasAs("is_active")]
	public bool? IsActive { get; init; }

	/// <summary>The page to return (<c>page</c>); Harvest's default is 1.</summary>
	[AliasAs("page")]
	public int? Page { get; init; }

	/// <summary>The number of results per page, from 1 to 2000 (<c>per_page</c>); Harvest's default is 2000.</summary>
	[AliasAs("per_page")]
	public int? PerPage { get; init; }
}
