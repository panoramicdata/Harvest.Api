namespace Harvest.Models;

/// <summary>The optional fields the bodies that create and update a user share.</summary>
public abstract class UserDtoBase : UserSettings
{
	/// <summary>The user's timezone; on a new user Harvest's default is the company's timezone.</summary>
	public string? Timezone { get; set; }

	/// <summary>
	/// Whether the user should be automatically added to future projects; on a new user Harvest's default is
	/// <see langword="false"/>.
	/// </summary>
	public bool? HasAccessToAllFutureProjects { get; set; }

	/// <summary>Whether the user is a contractor or an employee; on a new user Harvest's default is <see langword="false"/>.</summary>
	public bool? IsContractor { get; set; }

	/// <summary>Whether the user is active or archived; on a new user Harvest's default is <see langword="true"/>.</summary>
	public bool? IsActive { get; set; }
}

/// <summary>The body of <c>POST /v2/users</c>.</summary>
public class UserCreationDto : UserDtoBase
{
	/// <summary>The first name of the user.</summary>
	public required string FirstName { get; set; }

	/// <summary>The last name of the user.</summary>
	public required string LastName { get; set; }

	/// <summary>The email address of the user.</summary>
	public required string Email { get; set; }
}

/// <summary>The body of <c>PATCH /v2/users/{id}</c>.</summary>
public class UserPatchDto : UserDtoBase
{
	/// <summary>The first name of the user.</summary>
	public string? FirstName { get; set; }

	/// <summary>The last name of the user.</summary>
	public string? LastName { get; set; }

	/// <summary>The email address of the user.</summary>
	public string? Email { get; set; }
}