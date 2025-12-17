namespace Harvest.Models;

public class Project
{
	/// <summary>
	/// Unique ID for the project.
	/// </summary>
	public long Id { get; set; }

	/// <summary>
	/// An object containing the project’s client id, name, and currency.
	/// </summary>
	public Client? Client { get; set; }

	/// <summary>
	/// Unique name for the project.
	/// </summary>
	public required string Name { get; set; }

	/// <summary>
	/// The code associated with the project.
	/// </summary>
	public string? Code { get; set; }

	/// <summary>
	/// Whether the project is active or archived.
	/// </summary>
	public bool IsActive { get; set; }

	/// <summary>
	/// Whether the project is billable or not.
	/// </summary>
	public bool IsBillable { get; set; }

	/// <summary>
	/// Whether the project is a fixed-fee project or not.
	/// </summary>
	public bool IsFixedFee { get; set; }

	/// <summary>
	/// The method by which the project is invoiced.
	/// </summary>
	public required string BillBy { get; set; }

	/// <summary>
	/// Rate for projects billed by Project Hourly Rate.
	/// </summary>
	public decimal? HourlyRate { get; set; }

	/// <summary>
	/// The budget in hours for the project when budgeting by time.
	/// </summary>
	public decimal? Budget { get; set; }

	/// <summary>
	/// The method by which the project is budgeted.
	/// </summary>
	public required string BudgetBy { get; set; }

	/// <summary>
	/// Whether project managers should be notified when the project goes over budget.
	/// </summary>
	public bool NotifyWhenOverBudget { get; set; }

	/// <summary>
	/// Percentage value used to trigger over budget email alerts.
	/// </summary>
	public decimal OverBudgetNotificationPercentage { get; set; }

	/// <summary>
	/// Date of last over budget notification.If none have been sent, this will be null.
	/// </summary>
	public DateTime? OverBudgetNotificationDate { get; set; }

	/// <summary>
	/// Option to show project budget to all employees. Does not apply to Total Project Fee projects.
	/// </summary>
	public bool ShowBudgetToAll { get; set; }

	/// <summary>
	/// The monetary budget for the project when budgeting by money.
	/// </summary>
	public decimal? CostBudget { get; set; }

	/// <summary>
	/// Option for budget of Total Project Fees projects to include tracked expenses.
	/// </summary>
	public bool? CostBudgetIncludeExpenses { get; set; }

	/// <summary>
	/// The amount you plan to invoice for the project. Only used by fixed-fee projects.
	/// </summary>
	public decimal? Fee { get; set; }

	/// <summary>
	/// Project notes.
	/// </summary>
	public string? Notes { get; set; }

	/// <summary>
	/// Date the project was started.
	/// </summary>
	public DateTime? StartsOn { get; set; }

	/// <summary>
	/// Date the project will end.
	/// </summary>
	public DateTime? EndsOn { get; set; }

	/// <summary>
	/// Date and time the project was created.
	/// </summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>
	/// Date and time the project was last updated.
	/// </summary>
	public DateTime UpdatedAt { get; set; }
}