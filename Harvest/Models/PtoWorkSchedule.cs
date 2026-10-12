namespace Harvest.Models;

/// <summary>A PTO work schedule (<see href="https://help.getharvest.com/api-v2/pto-api/pto/work-schedules/"/>).</summary>
public class PtoWorkSchedule : PtoScheduleHours
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

	/// <summary>Date and time the schedule was created.</summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>Date and time the schedule was last updated.</summary>
	public DateTime UpdatedAt { get; set; }
}