namespace Harvest.Models;

/// <summary>
/// A user's PTO assignment: holiday calendar and work schedule
/// (<see href="https://help.getharvest.com/api-v2/pto-api/pto/assignments-and-allocations/"/>).
/// </summary>
public class PtoAssignment
{
	/// <summary>The user: its id and name.</summary>
	public UserReference? User { get; set; }

	/// <summary>Assigned holiday calendar (id, name), or <see langword="null"/> if unassigned.</summary>
	public PtoHolidayCalendarReference? HolidayCalendar { get; set; }

	/// <summary>Assigned active work schedule (id, name), or <see langword="null"/> if unassigned or deactivated.</summary>
	public PtoWorkScheduleReference? WorkSchedule { get; set; }

	/// <summary>Effective schedule applied to the user (name, weekly_hours).</summary>
	public PtoEffectiveWorkSchedule? EffectiveWorkSchedule { get; set; }
}

/// <summary>The <c>{name, weekly_hours}</c> summary of the work schedule effectively applied to a user.</summary>
public class PtoEffectiveWorkSchedule
{
	/// <summary>Schedule name.</summary>
	public string? Name { get; set; }

	/// <summary>Total working hours across the week.</summary>
	public decimal? WeeklyHours { get; set; }
}