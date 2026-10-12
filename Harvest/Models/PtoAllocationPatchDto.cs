using Harvest.Dtos;

namespace Harvest.Models;

/// <summary>
/// The body of <c>PATCH /v2/pto/allocations</c>. The user and policy identify the allocation, so they are required.
/// </summary>
public class PtoAllocationPatchDto : PatchDto
{
	/// <summary>The target user ID.</summary>
	public required long UserId { get; set; }

	/// <summary>The time off policy ID.</summary>
	public required long PtoTypeId { get; set; }

	/// <summary>The allocation year; Harvest's default is the current year.</summary>
	public int? Year { get; set; }

	/// <summary>Override accrual mode: <c>fixed</c>, <c>accrued</c> or <c>unlimited</c> (see <see cref="PtoAccrualType"/>).</summary>
	public string? AccrualType { get; set; }

	/// <summary>
	/// Override accrual interval: <c>monthly</c>, <c>biweekly</c>, <c>semimonthly</c> or <c>hourly</c> (see
	/// <see cref="PtoAccrualSchedule"/>).
	/// </summary>
	public string? AccrualSchedule { get; set; }

	/// <summary>Anchor date when <see cref="AccrualSchedule"/> is overridden to <c>biweekly</c>.</summary>
	public DateOnly? AccrualFirstPayday { get; set; }

	/// <summary>Leave hours earned per period when <see cref="AccrualSchedule"/> is overridden to <c>hourly</c>.</summary>
	public decimal? AccrualLeaveHours { get; set; }

	/// <summary>Base worked hours required to earn leave when <see cref="AccrualSchedule"/> is overridden to <c>hourly</c>.</summary>
	public decimal? AccrualWorkedHours { get; set; }

	/// <summary>Annual cap on accrued leave hours when <see cref="AccrualSchedule"/> is overridden to <c>hourly</c>.</summary>
	public decimal? AccrualMaxHoursPerYear { get; set; }

	/// <summary>Override annual allowance days.</summary>
	public decimal? DaysPerYear { get; set; }

	/// <summary>Override carryover days for this year.</summary>
	public decimal? CarryoverDays { get; set; }

	/// <summary>Override approval requirement.</summary>
	public bool? RequiresApproval { get; set; }

	/// <summary>Pass <see langword="true"/> to delete customized overrides and revert to policy defaults.</summary>
	public bool? ResetToDefault { get; set; }
}