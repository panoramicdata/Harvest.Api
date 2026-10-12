using Harvest.Dtos;

namespace Harvest.Models;

/// <summary>The body of <c>POST /v2/projects/{projectId}/task_assignments</c>.</summary>
public class ProjectTaskAssignmentCreationDto : CreationDto
{
	/// <summary>The ID of the task to associate with the project.</summary>
	public required long TaskId { get; set; }

	/// <summary>Whether the task assignment is active or archived; Harvest's default is <see langword="true"/>.</summary>
	public bool? IsActive { get; set; }

	/// <summary>Whether the task assignment is billable; Harvest's default is the task's <c>billable_by_default</c>.</summary>
	public bool? Billable { get; set; }

	/// <summary>Rate used when the project's bill_by is Tasks; Harvest's default is the task's default hourly rate.</summary>
	public decimal? HourlyRate { get; set; }

	/// <summary>Budget used when the project's budget_by is task or task_fees.</summary>
	public decimal? Budget { get; set; }
}

/// <summary>The body of <c>PATCH /v2/projects/{projectId}/task_assignments/{id}</c>.</summary>
public class ProjectTaskAssignmentPatchDto : PatchDto
{
	/// <summary>Whether the task assignment is active or archived.</summary>
	public bool? IsActive { get; set; }

	/// <summary>Whether the task assignment is billable or not.</summary>
	public bool? Billable { get; set; }

	/// <summary>Rate used when the project's bill_by is Tasks.</summary>
	public decimal? HourlyRate { get; set; }

	/// <summary>Budget used when the project's budget_by is task or task_fees.</summary>
	public decimal? Budget { get; set; }
}