namespace Harvest.Models;

public class UserProjectAssignment
{
	/// <summary>
	/// Unique ID for the project assignment.
	/// </summary>
	public long Id { get; set; }

	/// <summary>
	/// Whether the project assignment is active or archived.
	/// </summary>
	public bool IsActive { get; set; }

	/// <summary>
	/// Determines if the user has project manager permissions for the project.
	/// </summary>
	public bool IsProjectManager { get; set; }

	/// <summary>
	/// Rate used when the project's bill_by is People.
	/// </summary>
	public decimal? HourlyRate { get; set; }

	/// <summary>
	/// Budget used when the project's budget_by is person.
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

	/// <summary>
	/// An object containing the assigned project id, name, and code.
	/// </summary>
	public Project? Project { get; set; }

	/// <summary>
	/// 	An object containing the project's client id and name.
	/// </summary>
	public Client? Client { get; set; }

	/// <summary>
	/// Array of task assignment objects associated with the project.
	/// </summary>
	public List<ProjectTaskAssignment>? TaskAssignments { get; set; }
}