namespace Harvest.Models;

/// <summary>A role (<see href="https://help.getharvest.com/api-v2/roles-api/roles/roles/"/>).</summary>
public class Role
{
	/// <summary>Unique ID for the role.</summary>
	public long Id { get; set; }

	/// <summary>The name of the role.</summary>
	public string? Name { get; set; }

	/// <summary>The IDs of the users assigned to this role.</summary>
	public List<long>? UserIds { get; set; }

	/// <summary>Date and time the role was created.</summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>Date and time the role was last updated.</summary>
	public DateTime UpdatedAt { get; set; }
}