namespace Harvest.Models;

/// <summary>An expense category (<see href="https://help.getharvest.com/api-v2/expenses-api/expenses/expense-categories/"/>).</summary>
public class ExpenseCategory
{
	/// <summary>Unique ID for the expense category.</summary>
	public long Id { get; set; }

	/// <summary>The name of the expense category.</summary>
	public string? Name { get; set; }

	/// <summary>The unit name of the expense category, or <see langword="null"/> if it is not unit-based.</summary>
	public string? UnitName { get; set; }

	/// <summary>The unit price of the expense category, or <see langword="null"/> if it is not unit-based.</summary>
	public decimal? UnitPrice { get; set; }

	/// <summary>Whether the expense category is active or archived.</summary>
	public bool IsActive { get; set; }

	/// <summary>Date and time the expense category was created.</summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>Date and time the expense category was last updated.</summary>
	public DateTime UpdatedAt { get; set; }
}