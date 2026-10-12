using Harvest.Dtos;

namespace Harvest.Models;

/// <summary>
/// The body of <c>POST /v2/expenses</c>. Either <see cref="Units"/> or <see cref="TotalCost"/> is required:
/// <see cref="Units"/> for a unit-based expense category, <see cref="TotalCost"/> otherwise.
/// </summary>
public class ExpenseCreationDto : CreationDto
{
	/// <summary>The ID of the user associated with this expense; Harvest's default is the currently authenticated user.</summary>
	public long? UserId { get; set; }

	/// <summary>The ID of the project associated with this expense.</summary>
	public required long ProjectId { get; set; }

	/// <summary>The ID of the expense category this expense is being tracked against.</summary>
	public required long ExpenseCategoryId { get; set; }

	/// <summary>Date the expense occurred.</summary>
	public required DateOnly SpentDate { get; set; }

	/// <summary>
	/// The quantity of units to use in calculating the total_cost of the expense. Required if using a unit-based expense
	/// category. Harvest documents an integer.
	/// </summary>
	public decimal? Units { get; set; }

	/// <summary>The total amount of the expense. Required if not using a unit-based expense category.</summary>
	public decimal? TotalCost { get; set; }

	/// <summary>Textual notes used to describe the expense.</summary>
	public string? Notes { get; set; }

	/// <summary>Whether this expense is billable or not; Harvest's default is <see langword="true"/>.</summary>
	public bool? Billable { get; set; }
}

/// <summary>
/// The body of <c>PATCH /v2/expenses/{id}</c>. Harvest silently drops changes to <see cref="ProjectId"/> and
/// <see cref="ExpenseCategoryId"/> if the expense is locked.
/// </summary>
public class ExpensePatchDto : PatchDto
{
	/// <summary>The ID of the project associated with this expense.</summary>
	public long? ProjectId { get; set; }

	/// <summary>The ID of the expense category this expense is being tracked against.</summary>
	public long? ExpenseCategoryId { get; set; }

	/// <summary>Date the expense occurred.</summary>
	public DateOnly? SpentDate { get; set; }

	/// <summary>The quantity of units to use in calculating the total_cost of the expense. Harvest documents an integer.</summary>
	public decimal? Units { get; set; }

	/// <summary>The total amount of the expense.</summary>
	public decimal? TotalCost { get; set; }

	/// <summary>Textual notes used to describe the expense.</summary>
	public string? Notes { get; set; }

	/// <summary>Whether this expense is billable or not.</summary>
	public bool? Billable { get; set; }

	/// <summary>Whether an attached expense receipt should be deleted: pass <see langword="true"/> to delete the receipt.</summary>
	public bool? DeleteReceipt { get; set; }
}