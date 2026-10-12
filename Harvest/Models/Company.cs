namespace Harvest.Models;

/// <summary>
/// The company of the authenticated user (<see href="https://help.getharvest.com/api-v2/company-api/company/company/"/>).
/// Display settings are strings rather than enums so that a value Harvest adds later is still read.
/// </summary>
public class Company
{
	/// <summary>The Harvest URL for the company.</summary>
	public string? BaseUri { get; set; }

	/// <summary>The Harvest domain for the company.</summary>
	public string? FullDomain { get; set; }

	/// <summary>The name of the company.</summary>
	public string? Name { get; set; }

	/// <summary>Whether the company is active or archived.</summary>
	public bool IsActive { get; set; }

	/// <summary>The weekday used as the start of the week: Saturday, Sunday or Monday.</summary>
	public DayOfWeek? WeekStartDay { get; set; }

	/// <summary>Whether time is tracked via duration (<see langword="false"/>) or start and end times (<see langword="true"/>).</summary>
	public bool WantsTimestampTimers { get; set; }

	/// <summary>The format used to display time in Harvest: <c>decimal</c> or <c>hours_minutes</c>.</summary>
	public string? TimeFormat { get; set; }

	/// <summary>
	/// The format used to display dates in Harvest: one of <c>%m/%d/%Y</c>, <c>%d/%m/%Y</c>, <c>%Y-%m-%d</c>,
	/// <c>%d.%m.%Y</c>, <c>%Y.%m.%d</c> or <c>%Y/%m/%d</c>.
	/// </summary>
	public string? DateFormat { get; set; }

	/// <summary>The type of plan the company is on, for example <c>trial</c>, <c>free</c> or <c>simple-v4</c>.</summary>
	public string? PlanType { get; set; }

	/// <summary>Whether the company uses a 12-hour (<c>12h</c>) or 24-hour (<c>24h</c>) clock.</summary>
	public string? Clock { get; set; }

	/// <summary>How to display the currency code: <c>iso_code_none</c>, <c>iso_code_before</c> or <c>iso_code_after</c>.</summary>
	public string? CurrencyCodeDisplay { get; set; }

	/// <summary>How to display the currency symbol: <c>symbol_none</c>, <c>symbol_before</c> or <c>symbol_after</c>.</summary>
	public string? CurrencySymbolDisplay { get; set; }

	/// <summary>Symbol used when formatting decimals.</summary>
	public string? DecimalSymbol { get; set; }

	/// <summary>Separator used when formatting numbers.</summary>
	public string? ThousandsSeparator { get; set; }

	/// <summary>The color scheme being used in the Harvest web client.</summary>
	public string? ColorScheme { get; set; }

	/// <summary>The weekly capacity in seconds.</summary>
	public int? WeeklyCapacity { get; set; }

	/// <summary>Whether the expense module is enabled.</summary>
	public bool ExpenseFeature { get; set; }

	/// <summary>Whether the invoice module is enabled.</summary>
	public bool InvoiceFeature { get; set; }

	/// <summary>Whether the estimate module is enabled.</summary>
	public bool EstimateFeature { get; set; }

	/// <summary>Whether the approval module is enabled.</summary>
	public bool ApprovalFeature { get; set; }

	/// <summary>Whether the team module is enabled.</summary>
	public bool TeamFeature { get; set; }
}