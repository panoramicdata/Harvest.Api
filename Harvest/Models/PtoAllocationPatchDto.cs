namespace Harvest.Models;

/// <summary>
/// The body of <c>PATCH /v2/pto/allocations</c>. The user and policy identify the allocation, so they are required.
/// </summary>
public class PtoAllocationPatchDto : PtoAccrualSettings
{
	/// <summary>The target user ID.</summary>
	public required long UserId { get; set; }

	/// <summary>The time off policy ID.</summary>
	public required long PtoTypeId { get; set; }

	/// <summary>The allocation year; Harvest's default is the current year.</summary>
	public int? Year { get; set; }

	/// <summary>Override annual allowance days.</summary>
	public decimal? DaysPerYear { get; set; }

	/// <summary>Override carryover days for this year.</summary>
	public decimal? CarryoverDays { get; set; }

	/// <summary>Override approval requirement.</summary>
	public bool? RequiresApproval { get; set; }

	/// <summary>Pass <see langword="true"/> to delete customized overrides and revert to policy defaults.</summary>
	public bool? ResetToDefault { get; set; }
}