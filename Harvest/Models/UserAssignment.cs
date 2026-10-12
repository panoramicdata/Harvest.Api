namespace Harvest.Models;

/// <summary>
/// A user assignment: a user assigned to a project
/// (<see href="https://help.getharvest.com/api-v2/projects-api/projects/user-assignments/"/>). Time entries nest a subset
/// of it, without <see cref="Project"/>, <see cref="User"/> or <see cref="UseDefaultRates"/>.
/// </summary>
public class UserAssignment
{
	/// <summary>Unique ID for the user assignment.</summary>
	public long Id { get; set; }

	/// <summary>The project's id, name and code; absent where the assignment is nested in a time entry.</summary>
	public ProjectReference? Project { get; set; }

	/// <summary>The user's id and name; absent where the assignment is nested in a time entry.</summary>
	public UserReference? User { get; set; }

	/// <summary>Whether the user assignment is active or archived.</summary>
	public bool IsActive { get; set; }

	/// <summary>Whether the user has project manager permissions for the project.</summary>
	public bool IsProjectManager { get; set; }

	/// <summary>Whether the user assignment uses the user's default billable rates.</summary>
	public bool? UseDefaultRates { get; set; }

	/// <summary>Custom rate used when the project's bill_by is People and use_default_rates is false.</summary>
	public decimal? HourlyRate { get; set; }

	/// <summary>Budget used when the project's budget_by is person.</summary>
	public decimal? Budget { get; set; }

	/// <summary>Date and time the user assignment was created.</summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>Date and time the user assignment was last updated.</summary>
	public DateTime UpdatedAt { get; set; }
}