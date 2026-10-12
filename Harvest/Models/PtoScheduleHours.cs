namespace Harvest.Models;

/// <summary>
/// The working hours of each day of the week, as a <see cref="PtoWorkSchedule"/> reports them and as the bodies that
/// create and update a work schedule set them (0-24 each).
/// </summary>
public abstract class PtoScheduleHours
{
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
}