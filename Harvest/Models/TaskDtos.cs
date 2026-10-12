using Harvest.Dtos;

namespace Harvest.Models;

/// <summary>The body of <c>POST /v2/tasks</c>.</summary>
public class TaskCreationDto : CreationDto
{
	/// <summary>The name of the task.</summary>
	public required string Name { get; set; }

	/// <summary>
	/// Whether default tasks should be marked billable when creating a new project; Harvest's default is
	/// <see langword="true"/>.
	/// </summary>
	public bool? BillableByDefault { get; set; }

	/// <summary>The default hourly rate to use for this task when it is added to a project; Harvest's default is 0.</summary>
	public decimal? DefaultHourlyRate { get; set; }

	/// <summary>Whether this task should be automatically added to future projects; Harvest's default is <see langword="false"/>.</summary>
	public bool? IsDefault { get; set; }

	/// <summary>Whether this task is active or archived; Harvest's default is <see langword="true"/>.</summary>
	public bool? IsActive { get; set; }
}

/// <summary>The body of <c>PATCH /v2/tasks/{id}</c>.</summary>
public class TaskPatchDto : PatchDto
{
	/// <summary>The name of the task.</summary>
	public string? Name { get; set; }

	/// <summary>Whether default tasks should be marked billable when creating a new project.</summary>
	public bool? BillableByDefault { get; set; }

	/// <summary>The default hourly rate to use for this task when it is added to a project.</summary>
	public decimal? DefaultHourlyRate { get; set; }

	/// <summary>Whether this task should be automatically added to future projects.</summary>
	public bool? IsDefault { get; set; }

	/// <summary>Whether this task is active or archived.</summary>
	public bool? IsActive { get; set; }
}