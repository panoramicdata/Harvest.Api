namespace Harvest.Models;

/// <summary>A PTO work schedule (<see href="https://help.getharvest.com/api-v2/pto-api/pto/work-schedules/"/>).</summary>
public class PtoWorkSchedule
{
	/// <summary>Unique ID for the work schedule.</summary>
	public long Id { get; set; }

	/// <summary>Schedule name.</summary>
	public string? Name { get; set; }

	/// <summary>Whether this is the company fallback schedule.</summary>
	public bool IsDefault { get; set; }

	/// <summary>Whether the schedule is active or deactivated.</summary>
	public bool IsActive { get; set; }

	/// <summary>Total working hours across the week.</summary>
	public decimal? WeeklyHours { get; set; }

	/// <summary>Working hours scheduled for Monday.</summary>
	public decimal? MondayHours { get; set; }

	/// <summary>Working hours scheduled for Tuesday.</summary>
	public decimal? TuesdayHours { get; set; }

	/// <summary>Working hours scheduled for Wednesday.</summary>
	public decimal? WednesdayHours { get; set; }

	/// <summary>Working hours scheduled for Thursday.</summary>
	public decimal? ThursdayHours { get; set; }

	/// <summary>Working hours scheduled for Friday.</summary>
	public decimal? FridayHours { get; set; }

	/// <summary>Working hours scheduled for Saturday.</summary>
	public decimal? SaturdayHours { get; set; }

	/// <summary>Working hours scheduled for Sunday.</summary>
	public decimal? SundayHours { get; set; }

	/// <summary>Date and time the schedule was created.</summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>Date and time the schedule was last updated.</summary>
	public DateTime UpdatedAt { get; set; }
}