using Harvest.Dtos;

namespace Harvest.Models;

public class ProjectCreationDto : CreationDto
{
	[AliasAs("name")]
	public required string Name { get; set; }

	[AliasAs("client_id")]
	public long? ClientId { get; set; }

	[AliasAs("is_billable")]
	public bool IsBillable { get; set; } = true;

	[AliasAs("bill_by")]
	public required string BillBy { get; set; }

	[AliasAs("hourly_rate")]
	public decimal? HourlyRate { get; set; }

	[AliasAs("budget")]
	public decimal? Budget { get; set; }

	[AliasAs("budget_by")]
	public required string BudgetBy { get; set; }

	[AliasAs("notify_when_over_budget")]
	public bool NotifyWhenOverBudget { get; set; }

	[AliasAs("over_budget_notification_percentage")]
	public decimal? OverBudgetNotificationPercentage { get; set; }

	[AliasAs("show_budget_to_all")]
	public bool ShowBudgetToAll { get; set; }

	[AliasAs("cost_budget")]
	public decimal? CostBudget { get; set; }

	[AliasAs("cost_budget_include_expenses")]
	public bool? CostBudgetIncludeExpenses { get; set; }

	[AliasAs("fee")]
	public decimal? Fee { get; set; }

	[AliasAs("notes")]
	public string? Notes { get; set; }

	[AliasAs("starts_on")]
	public string? StartsOn { get; set; }

	[AliasAs("ends_on")]
	public string? EndsOn { get; set; }
}

public class ProjectPatchDto : PatchDto
{
	[AliasAs("name")]
	public string? Name { get; set; }

	[AliasAs("client_id")]
	public long? ClientId { get; set; }

	[AliasAs("is_billable")]
	public bool? IsBillable { get; set; }

	[AliasAs("bill_by")]
	public string? BillBy { get; set; }

	[AliasAs("hourly_rate")]
	public decimal? HourlyRate { get; set; }

	[AliasAs("budget")]
	public decimal? Budget { get; set; }

	[AliasAs("budget_by")]
	public string? BudgetBy { get; set; }

	[AliasAs("notify_when_over_budget")]
	public bool? NotifyWhenOverBudget { get; set; }

	[AliasAs("over_budget_notification_percentage")]
	public decimal? OverBudgetNotificationPercentage { get; set; }

	[AliasAs("show_budget_to_all")]
	public bool? ShowBudgetToAll { get; set; }

	[AliasAs("cost_budget")]
	public decimal? CostBudget { get; set; }

	[AliasAs("cost_budget_include_expenses")]
	public bool? CostBudgetIncludeExpenses { get; set; }

	[AliasAs("fee")]
	public decimal? Fee { get; set; }

	[AliasAs("notes")]
	public string? Notes { get; set; }

	[AliasAs("starts_on")]
	public string? StartsOn { get; set; }

	[AliasAs("ends_on")]
	public string? EndsOn { get; set; }
}