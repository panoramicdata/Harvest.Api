using Harvest.Dtos;

namespace Harvest.Models;

/// <summary>The body of <c>POST /v2/pto/work_schedules</c>.</summary>
public class PtoWorkScheduleCreationDto : CreationDto
{
	/// <summary>Name for the schedule.</summary>
	public required string Name { get; set; }

	/// <summary>Set to <see langword="true"/> to make this the company default schedule.</summary>
	public bool? IsDefault { get; set; }

	/// <summary>Monday working hours (0-24).</summary>
	public decimal? MondayHours { get; set; }

	/// <summary>Tuesday working hours (0-24).</summary>
	public decimal? TuesdayHours { get; set; }

	/// <summary>Wednesday working hours (0-24).</summary>
	public decimal? WednesdayHours { get; set; }

	/// <summary>Thursday working hours (0-24).</summary>
	public decimal? ThursdayHours { get; set; }

	/// <summary>Friday working hours (0-24).</summary>
	public decimal? FridayHours { get; set; }

	/// <summary>Saturday working hours (0-24).</summary>
	public decimal? SaturdayHours { get; set; }

	/// <summary>Sunday working hours (0-24).</summary>
	public decimal? SundayHours { get; set; }
}

/// <summary>The body of <c>PATCH /v2/pto/work_schedules/{id}</c>.</summary>
public class PtoWorkSchedulePatchDto : PatchDto
{
	/// <summary>Updated schedule name.</summary>
	public string? Name { get; set; }

	/// <summary>Set to <see langword="true"/> to promote this schedule to company default.</summary>
	public bool? IsDefault { get; set; }

	/// <summary>Updated Monday hours (0-24).</summary>
	public decimal? MondayHours { get; set; }

	/// <summary>Updated Tuesday hours (0-24).</summary>
	public decimal? TuesdayHours { get; set; }

	/// <summary>Updated Wednesday hours (0-24).</summary>
	public decimal? WednesdayHours { get; set; }

	/// <summary>Updated Thursday hours (0-24).</summary>
	public decimal? ThursdayHours { get; set; }

	/// <summary>Updated Friday hours (0-24).</summary>
	public decimal? FridayHours { get; set; }

	/// <summary>Updated Saturday hours (0-24).</summary>
	public decimal? SaturdayHours { get; set; }

	/// <summary>Updated Sunday hours (0-24).</summary>
	public decimal? SundayHours { get; set; }
}