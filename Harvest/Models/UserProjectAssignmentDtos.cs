using Harvest.Dtos;

namespace Harvest.Models;

public class UserProjectAssignmentCreationDto : CreationDto
{
	[AliasAs("project_id")]
	public required long ProjectId { get; set; }

	[AliasAs("is_active")]
	public bool IsActive { get; set; } = true;

	[AliasAs("is_project_manager")]
	public bool IsProjectManager { get; set; }

	[AliasAs("hourly_rate")]
	public decimal? HourlyRate { get; set; }

	[AliasAs("budget")]
	public decimal? Budget { get; set; }
}

public class UserProjectAssignmentPatchDto : PatchDto
{
	[AliasAs("is_active")]
	public bool? IsActive { get; set; }

	[AliasAs("is_project_manager")]
	public bool? IsProjectManager { get; set; }

	[AliasAs("hourly_rate")]
	public decimal? HourlyRate { get; set; }

	[AliasAs("budget")]
	public decimal? Budget { get; set; }
}