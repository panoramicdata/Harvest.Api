namespace Harvest.Models;

/// <summary>
/// One of a user's project assignments (<see href="https://help.getharvest.com/api-v2/users-api/users/project-assignments/"/>).
/// </summary>
public class UserProjectAssignment
{
	/// <summary>Unique ID for the project assignment.</summary>
	public long Id { get; set; }

	/// <summary>Whether the project assignment is active or archived.</summary>
	public bool IsActive { get; set; }

	/// <summary>Whether the user has project manager permissions for the project.</summary>
	public bool IsProjectManager { get; set; }

	/// <summary>Whether the user assignment uses the user's default billable rates.</summary>
	public bool? UseDefaultRates { get; set; }

	/// <summary>Rate used when the project's bill_by is People.</summary>
	public decimal? HourlyRate { get; set; }

	/// <summary>Budget used when the project's budget_by is person.</summary>
	public decimal? Budget { get; set; }

	/// <summary>Date and time the project assignment was created.</summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>Date and time the project assignment was last updated.</summary>
	public DateTime UpdatedAt { get; set; }

	/// <summary>The assigned project's id, name and code.</summary>
	public ProjectReference? Project { get; set; }

	/// <summary>The project's client's id and name.</summary>
	public ClientReference? Client { get; set; }

	/// <summary>The task assignments of the project.</summary>
	public List<ProjectTaskAssignment>? TaskAssignments { get; set; }
}