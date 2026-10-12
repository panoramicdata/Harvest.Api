namespace Harvest.Models;

/// <summary>
/// A task assignment: a task assigned to a project
/// (<see href="https://help.getharvest.com/api-v2/projects-api/projects/task-assignments/"/>). Time entries nest a subset
/// of it, without <see cref="Project"/> or <see cref="Task"/>.
/// </summary>
public class ProjectTaskAssignment
{
	/// <summary>Unique ID for the task assignment.</summary>
	public long Id { get; set; }

	/// <summary>The project's id, name and code; absent where the assignment is nested in another object.</summary>
	public ProjectReference? Project { get; set; }

	/// <summary>The task's id and name; absent where the assignment is nested in a time entry.</summary>
	public TaskReference? Task { get; set; }

	/// <summary>Whether the task assignment is active or archived.</summary>
	public bool IsActive { get; set; }

	/// <summary>Whether the task assignment is billable or not.</summary>
	public bool Billable { get; set; }

	/// <summary>Rate used when the project's bill_by is Tasks.</summary>
	public decimal? HourlyRate { get; set; }

	/// <summary>Budget used when the project's budget_by is task or task_fees.</summary>
	public decimal? Budget { get; set; }

	/// <summary>Date and time the task assignment was created.</summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>Date and time the task assignment was last updated.</summary>
	public DateTime UpdatedAt { get; set; }
}