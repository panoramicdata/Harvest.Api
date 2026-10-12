namespace Harvest.Models;

/// <summary>
/// How time off accrues, as a time off policy (<see cref="PtoType"/>) defines it and an allocation
/// (<see cref="PtoAllocation"/>) may override it, in responses and in the bodies that create and update them.
/// </summary>
public abstract class PtoAccrualSettings
{
	/// <summary>
	/// Accrual mode: <c>fixed</c> (front-loaded allowance), <c>accrued</c> (earned over time) or <c>unlimited</c> (see
	/// <see cref="PtoAccrualType"/>). On a new policy Harvest's default is <c>fixed</c>.
	/// </summary>
	public string? AccrualType { get; set; }

	/// <summary>
	/// Accrual interval: <c>monthly</c>, <c>biweekly</c>, <c>semimonthly</c> or <c>hourly</c> (see
	/// <see cref="PtoAccrualSchedule"/>). Required when <see cref="AccrualType"/> is <c>accrued</c>.
	/// </summary>
	public string? AccrualSchedule { get; set; }

	/// <summary>Anchor date, required when <see cref="AccrualSchedule"/> is <c>biweekly</c>.</summary>
	public DateOnly? AccrualFirstPayday { get; set; }

	/// <summary>Hours of leave earned per worked interval when <see cref="AccrualSchedule"/> is <c>hourly</c>.</summary>
	public decimal? AccrualLeaveHours { get; set; }

	/// <summary>Base worked hours required to earn leave when <see cref="AccrualSchedule"/> is <c>hourly</c>.</summary>
	public decimal? AccrualWorkedHours { get; set; }

	/// <summary>Annual cap on accrued leave hours when <see cref="AccrualSchedule"/> is <c>hourly</c>.</summary>
	public decimal? AccrualMaxHoursPerYear { get; set; }
}