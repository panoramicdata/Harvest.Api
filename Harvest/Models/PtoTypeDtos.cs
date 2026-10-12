using Harvest.Dtos;

namespace Harvest.Models;

/// <summary>The body of <c>POST /v2/pto/types</c>.</summary>
public class PtoTypeCreationDto : CreationDto
{
	/// <summary>The policy name.</summary>
	public required string Name { get; set; }

	/// <summary>Optional color code.</summary>
	public string? Color { get; set; }

	/// <summary>Optional icon name.</summary>
	public string? Icon { get; set; }

	/// <summary>
	/// Accrual mode: <c>fixed</c>, <c>accrued</c> or <c>unlimited</c> (see <see cref="PtoAccrualType"/>); Harvest's default
	/// is <c>fixed</c>.
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

	/// <summary>Default days per year.</summary>
	public decimal? DefaultDaysPerYear { get; set; }

	/// <summary>Maximum carryover days allowed.</summary>
	public decimal? MaxCarryoverDays { get; set; }

	/// <summary>Whether requests require approval.</summary>
	public bool? RequiresApproval { get; set; }

	/// <summary>Whether visible to all users.</summary>
	public bool? VisibleToAll { get; set; }
}

/// <summary>The body of <c>PATCH /v2/pto/types/{id}</c>.</summary>
public class PtoTypePatchDto : PatchDto
{
	/// <summary>The updated policy name.</summary>
	public string? Name { get; set; }

	/// <summary>The updated color.</summary>
	public string? Color { get; set; }

	/// <summary>The updated icon.</summary>
	public string? Icon { get; set; }

	/// <summary>The updated accrual mode: <c>fixed</c>, <c>accrued</c> or <c>unlimited</c>.</summary>
	public string? AccrualType { get; set; }

	/// <summary>The updated accrual interval: <c>monthly</c>, <c>biweekly</c>, <c>semimonthly</c> or <c>hourly</c>.</summary>
	public string? AccrualSchedule { get; set; }

	/// <summary>The updated first payday for biweekly accruals.</summary>
	public DateOnly? AccrualFirstPayday { get; set; }

	/// <summary>The updated leave hours earned per period for hourly accruals.</summary>
	public decimal? AccrualLeaveHours { get; set; }

	/// <summary>The updated base worked hours for hourly accruals.</summary>
	public decimal? AccrualWorkedHours { get; set; }

	/// <summary>The updated maximum accruable hours for hourly accruals.</summary>
	public decimal? AccrualMaxHoursPerYear { get; set; }

	/// <summary>The updated default days per year.</summary>
	public decimal? DefaultDaysPerYear { get; set; }

	/// <summary>The updated maximum carryover days.</summary>
	public decimal? MaxCarryoverDays { get; set; }

	/// <summary>Update approval requirement.</summary>
	public bool? RequiresApproval { get; set; }

	/// <summary>Update visibility.</summary>
	public bool? VisibleToAll { get; set; }

	/// <summary>Set to <see langword="false"/> to reactivate a deactivated policy.</summary>
	public bool? Deactivated { get; set; }
}