namespace Harvest.Models;

/// <summary>A teammate: a user assigned to a manager (<see href="https://help.getharvest.com/api-v2/users-api/users/teammates/"/>).</summary>
public class Teammate
{
	/// <summary>Unique ID for the teammate.</summary>
	public long Id { get; set; }

	/// <summary>The first name of the teammate.</summary>
	public string? FirstName { get; set; }

	/// <summary>The last name of the teammate.</summary>
	public string? LastName { get; set; }

	/// <summary>The email of the teammate.</summary>
	public string? Email { get; set; }
}