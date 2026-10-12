namespace Harvest.Models;

/// <summary>A user (<see href="https://help.getharvest.com/api-v2/users-api/users/users/"/>).</summary>
public class User : UserSettings
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

	/// <summary>The URL to the user's avatar image.</summary>
	public string? AvatarUrl { get; set; }

	/// <summary>Date and time the user was created.</summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>Date and time the user was last updated.</summary>
	public DateTime UpdatedAt { get; set; }
}