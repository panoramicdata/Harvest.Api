using Harvest.Dtos;

namespace Harvest.Models;

/// <summary>The body of <c>POST /v2/users</c>.</summary>
public class UserCreationDto : CreationDto
{
	/// <summary>The first name of the user.</summary>
	public required string FirstName { get; set; }

	/// <summary>The last name of the user.</summary>
	public required string LastName { get; set; }

	/// <summary>The email address of the user.</summary>
	public required string Email { get; set; }

	/// <summary>The user's timezone; Harvest's default is the company's timezone.</summary>
	public string? Timezone { get; set; }

	/// <summary>Whether the user should be automatically added to future projects; Harvest's default is <see langword="false"/>.</summary>
	public bool? HasAccessToAllFutureProjects { get; set; }

	/// <summary>Whether the user is a contractor or an employee; Harvest's default is <see langword="false"/>.</summary>
	public bool? IsContractor { get; set; }

	/// <summary>Whether the user is active or archived; Harvest's default is <see langword="true"/>.</summary>
	public bool? IsActive { get; set; }

	/// <summary>Whether the user can sign in without single sign-on; Harvest's default is <see langword="false"/>.</summary>
	public bool? SamlExempt { get; set; }

	/// <summary>The number of hours per week this person is available to work, in seconds; Harvest's default is 126000 (35 hours).</summary>
	public int? WeeklyCapacity { get; set; }

	/// <summary>The billable rate to use for this user when they are added to a project; Harvest's default is 0.</summary>
	public decimal? DefaultHourlyRate { get; set; }

	/// <summary>The cost rate to use for this user when calculating a project's costs vs billable amount; Harvest's default is 0.</summary>
	public decimal? CostRate { get; set; }

	/// <summary>Descriptive names of the business roles assigned to this person.</summary>
	public List<string>? Roles { get; set; }

	/// <summary>Access role(s) that determine the user's permissions in Harvest; Harvest's default is <c>member</c>.</summary>
	public List<string>? AccessRoles { get; set; }

	/// <summary>The name of the permissions profile to give the user.</summary>
	public string? PermissionsProfile { get; set; }
}

/// <summary>The body of <c>PATCH /v2/users/{id}</c>.</summary>
public class UserPatchDto : PatchDto
{
	/// <summary>The first name of the user.</summary>
	public string? FirstName { get; set; }

	/// <summary>The last name of the user.</summary>
	public string? LastName { get; set; }

	/// <summary>The email address of the user.</summary>
	public string? Email { get; set; }

	/// <summary>The user's timezone.</summary>
	public string? Timezone { get; set; }

	/// <summary>Whether the user should be automatically added to future projects.</summary>
	public bool? HasAccessToAllFutureProjects { get; set; }

	/// <summary>Whether the user is a contractor or an employee.</summary>
	public bool? IsContractor { get; set; }

	/// <summary>Whether the user is active or archived.</summary>
	public bool? IsActive { get; set; }

	/// <summary>Whether the user can sign in without single sign-on.</summary>
	public bool? SamlExempt { get; set; }

	/// <summary>The number of hours per week this person is available to work, in seconds.</summary>
	public int? WeeklyCapacity { get; set; }

	/// <summary>The billable rate to use for this user when they are added to a project.</summary>
	public decimal? DefaultHourlyRate { get; set; }

	/// <summary>The cost rate to use for this user when calculating a project's costs vs billable amount.</summary>
	public decimal? CostRate { get; set; }

	/// <summary>Descriptive names of the business roles assigned to this person.</summary>
	public List<string>? Roles { get; set; }

	/// <summary>Access role(s) that determine the user's permissions in Harvest.</summary>
	public List<string>? AccessRoles { get; set; }

	/// <summary>The name of the permissions profile to give the user.</summary>
	public string? PermissionsProfile { get; set; }
}