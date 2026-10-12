namespace Harvest.Models;

/// <summary>A client (<see href="https://help.getharvest.com/api-v2/clients-api/clients/clients/"/>).</summary>
public class Client
{
	/// <summary>Unique ID for the client.</summary>
	public long Id { get; set; }

	/// <summary>A textual description of the client.</summary>
	public string? Name { get; set; }

	/// <summary>Whether the client is active or archived.</summary>
	public bool IsActive { get; set; }

	/// <summary>The physical address for the client.</summary>
	public string? Address { get; set; }

	/// <summary>Used to build a URL to your client's invoice dashboard.</summary>
	public string? StatementKey { get; set; }

	/// <summary>The currency code associated with this client.</summary>
	public string? Currency { get; set; }

	/// <summary>Date and time the client was created.</summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>Date and time the client was last updated.</summary>
	public DateTime UpdatedAt { get; set; }
}