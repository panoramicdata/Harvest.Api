namespace Harvest.Models;

public class Task
{
	/// <summary>
	/// Unique ID for the task.
	/// </summary>
	public long Id { get; set; }

	/// <summary>
	/// The name of the task.
	/// </summary>
	public required string Name { get; set; }

	/// <summary>
	/// Used in determining whether default tasks should be marked billable when creating a new project.
	/// </summary>
	public bool BillableByDefault { get; set; }

	/// <summary>
	/// The hourly rate to use for this task when it is added to a project.
	/// </summary>
	public decimal DefaultHourlyRate { get; set; }

	/// <summary>
	/// Whether this task should be automatically added to future projects.
	/// </summary>
	public bool IsDefault { get; set; }

	/// <summary>
	/// Whether this task is active or archived.
	/// </summary>
	public bool IsActive { get; set; }

	/// <summary>
	/// Date and time the task was created.
	/// </summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>
	/// Date and time the task was last updated.
	/// </summary>
	public DateTime UpdatedAt { get; set; }
}