namespace Harvest.Models;

/// <summary>A user (<see href="https://help.getharvest.com/api-v2/users-api/users/users/"/>).</summary>
public class User
{
	/// <summary>Unique ID for the user.</summary>
	public long Id { get; set; }

	/// <summary>The first name of the user.</summary>
	public string? FirstName { get; set; }

	/// <summary>The last name of the user.</summary>
	public string? LastName { get; set; }

	/// <summary>The email address of the user.</summary>
	public string? Email { get; set; }

	/// <summary>The user's telephone number.</summary>
	public string? Telephone { get; set; }

	/// <summary>The user's timezone.</summary>
	public string? Timezone { get; set; }

	/// <summary>Whether the user should be automatically added to future projects.</summary>
	public bool HasAccessToAllFutureProjects { get; set; }

	/// <summary>Whether the user is a contractor or an employee.</summary>
	public bool IsContractor { get; set; }

	/// <summary>Whether the user is active or archived.</summary>
	public bool IsActive { get; set; }

	/// <summary>Whether the user can sign in without single sign-on when SAML is enabled for the account.</summary>
	public bool? SamlExempt { get; set; }

	/// <summary>The number of hours per week this person is available to work, in seconds.</summary>
	public int? WeeklyCapacity { get; set; }

	/// <summary>The billable rate to use for this user when they are added to a project.</summary>
	public decimal? DefaultHourlyRate { get; set; }

	/// <summary>The cost rate to use for this user when calculating a project's costs vs billable amount.</summary>
	public decimal? CostRate { get; set; }

	/// <summary>Descriptive names of the business roles assigned to this person.</summary>
	public List<string>? Roles { get; set; }

	/// <summary>
	/// Access role(s) that determine the user's permissions in Harvest: <c>administrator</c>, <c>manager</c> or
	/// <c>member</c>, plus manager permissions such as <c>project_creator</c> or <c>expenses_manager</c>.
	/// </summary>
	public List<string>? AccessRoles { get; set; }

	/// <summary>The name of the user's permissions profile.</summary>
	public string? PermissionsProfile { get; set; }

	/// <summary>The URL to the user's avatar image.</summary>
	public string? AvatarUrl { get; set; }

	/// <summary>Date and time the user was created.</summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>Date and time the user was last updated.</summary>
	public DateTime UpdatedAt { get; set; }
}