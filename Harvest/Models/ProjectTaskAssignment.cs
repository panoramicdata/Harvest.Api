namespace Harvest.Models;

public class ProjectTaskAssignment
{
	/// <summary>
	/// Unique ID for the task assignment.
	/// </summary>
	public long Id { get; set; }

	/// <summary>
	/// An object containing the id and name of the associated task.
	/// </summary>
	public Task? Task { get; set; }

	/// <summary>
	/// Whether the task assignment is active or archived.
	/// </summary>
	public bool IsActive { get; set; }

	/// <summary>
	/// Whether or not the time entry is billable.
	/// </summary>
	public bool Billable { get; set; }

	/// <summary>
	/// Rate used when the project’s bill_by is Tasks.
	/// </summary>
	public decimal? HourlyRate { get; set; }

	/// <summary>
	/// Whether or not the time entry is billable.
	/// </summary>
	public decimal? Budget { get; set; }

	/// <summary>
	/// Date and time the time entry was created.
	/// </summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>
	/// Date and time the time entry was last updated.
	/// </summary>
	public DateTime UpdatedAt { get; set; }
}