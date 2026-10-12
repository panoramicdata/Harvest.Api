namespace Harvest.Models;

/// <summary>
/// The <c>{id, name, color, icon}</c> summary of a time off policy (PTO type) that Harvest nests in requests, balances and
/// allocations.
/// </summary>
public class PtoTypeReference
{
	/// <summary>Unique ID for the time off policy.</summary>
	public long Id { get; set; }

	/// <summary>The name of the time off policy.</summary>
	public string? Name { get; set; }

	/// <summary>Color assigned to the policy in the UI.</summary>
	public string? Color { get; set; }

	/// <summary>Icon slug assigned to the policy.</summary>
	public string? Icon { get; set; }
}

/// <summary>The <c>{id, name}</c> summary of a holiday calendar that Harvest nests in PTO user assignments.</summary>
public class PtoHolidayCalendarReference
{
	/// <summary>Unique ID for the holiday calendar.</summary>
	public long Id { get; set; }

	/// <summary>Name of the calendar.</summary>
	public string? Name { get; set; }
}

/// <summary>The <c>{id, name}</c> summary of a work schedule that Harvest nests in PTO user assignments.</summary>
public class PtoWorkScheduleReference
{
	/// <summary>Unique ID for the work schedule.</summary>
	public long Id { get; set; }

	/// <summary>Schedule name.</summary>
	public string? Name { get; set; }
}

/// <summary>The <c>{name, weekly_hours}</c> summary of the work schedule effectively applied to a user.</summary>
public class PtoEffectiveWorkSchedule
{
	/// <summary>Schedule name.</summary>
	public string? Name { get; set; }

	/// <summary>Total working hours across the week.</summary>
	public decimal? WeeklyHours { get; set; }
}
