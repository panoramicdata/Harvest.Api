namespace Harvest.Models;

/// <summary>
/// The optional fields the bodies that create and update a project share; on creation Harvest applies its default to each
/// one left <see langword="null"/>.
/// </summary>
public abstract class ProjectDtoBase
{
	/// <summary>The code associated with the project.</summary>
	public string? Code { get; set; }

	/// <summary>Whether the project is active or archived; Harvest's default is <see langword="true"/>.</summary>
	public bool? IsActive { get; set; }

	/// <summary>Whether the project is a fixed-fee project or not.</summary>
	public bool? IsFixedFee { get; set; }

	/// <summary>Rate for projects billed by Project Hourly Rate.</summary>
	public decimal? HourlyRate { get; set; }

	/// <summary>Whether the budget resets every month; Harvest's default is <see langword="false"/>.</summary>
	public bool? BudgetIsMonthly { get; set; }

	/// <summary>The budget in hours for the project when budgeting by time.</summary>
	public decimal? Budget { get; set; }

	/// <summary>The monetary budget for the project when budgeting by money.</summary>
	public decimal? CostBudget { get; set; }

	/// <summary>Option for budget of Total Project Fees projects to include tracked expenses.</summary>
	public bool? CostBudgetIncludeExpenses { get; set; }

	/// <summary>Whether project managers should be notified when the project goes over budget.</summary>
	public bool? NotifyWhenOverBudget { get; set; }

	/// <summary>Percentage value used to trigger over budget email alerts.</summary>
	public decimal? OverBudgetNotificationPercentage { get; set; }

	/// <summary>Option to show project budget to all employees.</summary>
	public bool? ShowBudgetToAll { get; set; }

	/// <summary>The amount you plan to invoice for the project. Only used by fixed-fee projects.</summary>
	public decimal? Fee { get; set; }

	/// <summary>Project notes.</summary>
	public string? Notes { get; set; }

	/// <summary>Date the project was started.</summary>
	public DateOnly? StartsOn { get; set; }

	/// <summary>Date the project will end.</summary>
	public DateOnly? EndsOn { get; set; }
}

/// <summary>The body of <c>POST /v2/projects</c>.</summary>
public class ProjectCreationDto : ProjectDtoBase
{
	/// <summary>The ID of the client to associate this project with.</summary>
	public required long ClientId { get; set; }

	/// <summary>The name of the project.</summary>
	public required string Name { get; set; }

	/// <summary>Whether the project is billable or not.</summary>
	public required bool IsBillable { get; set; }

	/// <summary>The method by which the project is invoiced: <c>Project</c>, <c>Tasks</c>, <c>People</c> or <c>none</c>.</summary>
	public required string BillBy { get; set; }

	/// <summary>
	/// The method by which the project is budgeted: <c>project</c>, <c>project_cost</c>, <c>task</c>, <c>task_fees</c>,
	/// <c>person</c> or <c>none</c>.
	/// </summary>
	public required string BudgetBy { get; set; }
}

/// <summary>The body of <c>PATCH /v2/projects/{id}</c>.</summary>
public class ProjectPatchDto : ProjectDtoBase
{
	/// <summary>The ID of the client to associate this project with.</summary>
	public long? ClientId { get; set; }

	/// <summary>The name of the project.</summary>
	public string? Name { get; set; }

	/// <summary>Whether the project is billable or not.</summary>
	public bool? IsBillable { get; set; }

	/// <summary>The method by which the project is invoiced.</summary>
	public string? BillBy { get; set; }

	/// <summary>The method by which the project is budgeted.</summary>
	public string? BudgetBy { get; set; }
}