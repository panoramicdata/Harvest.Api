namespace Harvest.Models;

/// <summary>The user behind an access token, as Harvest ID's accounts endpoint describes them.</summary>
public class AccountUser
{
	/// <summary>The Harvest ID user ID (not a user ID within any one account).</summary>
	public long Id { get; set; }

	/// <summary>The first name of the user.</summary>
	public string? FirstName { get; set; }

	/// <summary>The last name of the user.</summary>
	public string? LastName { get; set; }

	/// <summary>The email address of the user.</summary>
	public string? Email { get; set; }
}