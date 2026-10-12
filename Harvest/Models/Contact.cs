namespace Harvest.Models;

/// <summary>A client contact (<see href="https://help.getharvest.com/api-v2/clients-api/clients/contacts/"/>).</summary>
public class Contact : ContactDetails
{
	/// <summary>Unique ID for the contact.</summary>
	public long Id { get; set; }

	/// <summary>The contact's client: its id and name.</summary>
	public ClientReference? Client { get; set; }

	/// <summary>The first name of the contact.</summary>
	public string? FirstName { get; set; }

	/// <summary>Date and time the contact was created.</summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>Date and time the contact was last updated.</summary>
	public DateTime UpdatedAt { get; set; }
}