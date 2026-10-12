namespace Harvest.Models;

/// <summary>The body of <c>POST /v2/pto/work_schedules</c>.</summary>
public class PtoWorkScheduleCreationDto : PtoScheduleHours
{
	/// <summary>Name for the schedule.</summary>
	public required string Name { get; set; }

	/// <summary>Set to <see langword="true"/> to make this the company default schedule.</summary>
	public bool? IsDefault { get; set; }
}

/// <summary>The body of <c>PATCH /v2/pto/work_schedules/{id}</c>.</summary>
public class PtoWorkSchedulePatchDto : PtoScheduleHours
{
	/// <summary>Updated schedule name.</summary>
	public string? Name { get; set; }

	/// <summary>Set to <see langword="true"/> to promote this schedule to company default.</summary>
	public bool? IsDefault { get; set; }
}