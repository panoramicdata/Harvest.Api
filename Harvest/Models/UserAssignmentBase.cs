namespace Harvest.Models;

/// <summary>
/// The fields a <see cref="UserAssignment"/> (a project's view of an assigned user) and a
/// <see cref="UserProjectAssignment"/> (a user's view of a project they are assigned to) share.
/// </summary>
public abstract class UserAssignmentBase
{
	/// <summary>Unique ID for the assignment.</summary>
	public long Id { get; set; }

	/// <summary>Whether the assignment is active or archived.</summary>
	public bool IsActive { get; set; }

	/// <summary>Whether the user has project manager permissions for the project.</summary>
	public bool IsProjectManager { get; set; }

	/// <summary>Whether the assignment uses the user's default billable rates.</summary>
	public bool? UseDefaultRates { get; set; }

	/// <summary>Custom rate used when the project's bill_by is People and use_default_rates is false.</summary>
	public decimal? HourlyRate { get; set; }

	/// <summary>Budget used when the project's budget_by is person.</summary>
	public decimal? Budget { get; set; }

	/// <summary>Date and time the assignment was created.</summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>Date and time the assignment was last updated.</summary>
	public DateTime UpdatedAt { get; set; }
}