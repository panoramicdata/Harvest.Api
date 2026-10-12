using Harvest.Dtos;

namespace Harvest.Models;

/// <summary>The body of <c>POST /v2/projects/{projectId}/user_assignments</c>.</summary>
public class UserAssignmentCreationDto : CreationDto
{
	/// <summary>The ID of the user to associate with the project.</summary>
	public required long UserId { get; set; }

	/// <summary>Whether the user assignment is active or archived; Harvest's default is <see langword="true"/>.</summary>
	public bool? IsActive { get; set; }

	/// <summary>
	/// Whether the user has Project Manager permissions for the project; Harvest's default is <see langword="false"/> for
	/// users with Regular User permissions and <see langword="true"/> for Project Managers and Administrators.
	/// </summary>
	public bool? IsProjectManager { get; set; }

	/// <summary>
	/// Which billable rate(s) the project uses for this user when bill_by is People: the user's default rates
	/// (<see langword="true"/>) or <see cref="HourlyRate"/> (<see langword="false"/>). Harvest's default is
	/// <see langword="true"/>.
	/// </summary>
	public bool? UseDefaultRates { get; set; }

	/// <summary>Custom rate used when the project's bill_by is People and use_default_rates is false; Harvest's default is 0.</summary>
	public decimal? HourlyRate { get; set; }

	/// <summary>Budget used when the project's budget_by is person.</summary>
	public decimal? Budget { get; set; }
}

/// <summary>The body of <c>PATCH /v2/projects/{projectId}/user_assignments/{id}</c>.</summary>
public class UserAssignmentPatchDto : PatchDto
{
	/// <summary>Whether the user assignment is active or archived.</summary>
	public bool? IsActive { get; set; }

	/// <summary>Whether the user has Project Manager permissions for the project.</summary>
	public bool? IsProjectManager { get; set; }

	/// <summary>
	/// Which billable rate(s) the project uses for this user when bill_by is People: the user's default rates
	/// (<see langword="true"/>) or <see cref="HourlyRate"/> (<see langword="false"/>).
	/// </summary>
	public bool? UseDefaultRates { get; set; }

	/// <summary>Custom rate used when the project's bill_by is People and use_default_rates is false.</summary>
	public decimal? HourlyRate { get; set; }

	/// <summary>Budget used when the project's budget_by is person.</summary>
	public decimal? Budget { get; set; }
}