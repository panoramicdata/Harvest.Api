namespace Harvest.Models;

/// <summary>An expense (<see href="https://help.getharvest.com/api-v2/expenses-api/expenses/expenses/"/>).</summary>
public class Expense
{
	/// <summary>Unique ID for the expense.</summary>
	public long Id { get; set; }

	/// <summary>The expense's client: its id, name and currency.</summary>
	public ClientReference? Client { get; set; }

	/// <summary>The expense's project: its id, name and code.</summary>
	public ProjectReference? Project { get; set; }

	/// <summary>The expense's expense category: its id, name, unit_price and unit_name.</summary>
	public ExpenseCategoryReference? ExpenseCategory { get; set; }

	/// <summary>The id and name of the user that recorded the expense.</summary>
	public UserReference? User { get; set; }

	/// <summary>The user assignment of the user that recorded the expense (without its project and user).</summary>
	public UserAssignment? UserAssignment { get; set; }

	/// <summary>The expense's receipt, or <see langword="null"/> if none is attached.</summary>
	public ExpenseReceipt? Receipt { get; set; }

	/// <summary>Once the expense has been invoiced, the associated invoice's id and number; otherwise <see langword="null"/>.</summary>
	public InvoiceReference? Invoice { get; set; }

	/// <summary>Textual notes used to describe the expense.</summary>
	public string? Notes { get; set; }

	/// <summary>
	/// The quantity of units used to calculate the total_cost of the expense. Harvest documents an integer but sends a
	/// decimal (<c>1.0</c>).
	/// </summary>
	public decimal? Units { get; set; }

	/// <summary>The total amount of the expense.</summary>
	public decimal TotalCost { get; set; }

	/// <summary>Whether the expense is billable or not.</summary>
	public bool Billable { get; set; }

	/// <summary>Whether the expense has been approved or not. Deprecated by Harvest: use <see cref="ApprovalStatus"/>.</summary>
	public bool IsClosed { get; set; }

	/// <summary>The approval status of the expense: one of the <see cref="Models.ApprovalStatus"/> values.</summary>
	public string? ApprovalStatus { get; set; }

	/// <summary>
	/// Whether the expense has been invoiced, approved, or the project or person related to the expense is archived.
	/// </summary>
	public bool IsLocked { get; set; }

	/// <summary>Whether or not the expense has been marked as invoiced.</summary>
	public bool IsBilled { get; set; }

	/// <summary>An explanation of why the expense has been locked, or <see langword="null"/> if it is not.</summary>
	public string? LockedReason { get; set; }

	/// <summary>Date the expense occurred.</summary>
	public DateOnly SpentDate { get; set; }

	/// <summary>Date and time the expense was created.</summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>Date and time the expense was last updated.</summary>
	public DateTime UpdatedAt { get; set; }
}

/// <summary>The receipt attached to an <see cref="Expense"/>.</summary>
public class ExpenseReceipt
{
	/// <summary>The URL of the receipt (on the account's <c>harvestapp.com</c> subdomain).</summary>
	public string? Url { get; set; }

	/// <summary>The receipt's file name.</summary>
	public string? FileName { get; set; }

	/// <summary>The receipt's size in bytes.</summary>
	public long? FileSize { get; set; }

	/// <summary>The receipt's media type, for example <c>image/gif</c>.</summary>
	public string? ContentType { get; set; }
}