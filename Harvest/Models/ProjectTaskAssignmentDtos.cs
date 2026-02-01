using Harvest.Dtos;

namespace Harvest.Models;

public class ProjectTaskAssignmentCreationDto : CreationDto
{
	[AliasAs("task_id")]
	public required long TaskId { get; set; }

	[AliasAs("is_active")]
	public bool IsActive { get; set; } = true;

	[AliasAs("billable")]
	public bool Billable { get; set; } = true;

	[AliasAs("hourly_rate")]
	public decimal? HourlyRate { get; set; }

	[AliasAs("budget")]
	public decimal? Budget { get; set; }
}

public class ProjectTaskAssignmentPatchDto : PatchDto
{
	[AliasAs("is_active")]
	public bool? IsActive { get; set; }

	[AliasAs("billable")]
	public bool? Billable { get; set; }

	[AliasAs("hourly_rate")]
	public decimal? HourlyRate { get; set; }

	[AliasAs("budget")]
	public decimal? Budget { get; set; }
}