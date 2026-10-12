namespace Harvest.Models;

/// <summary>A client contact (<see href="https://help.getharvest.com/api-v2/clients-api/clients/contacts/"/>).</summary>
public class Contact
{
	/// <summary>Unique ID for the contact.</summary>
	public long Id { get; set; }

	/// <summary>The contact's client: its id and name.</summary>
	public ClientReference? Client { get; set; }

	/// <summary>The title of the contact.</summary>
	public string? Title { get; set; }

	/// <summary>The first name of the contact.</summary>
	public string? FirstName { get; set; }

	/// <summary>The last name of the contact.</summary>
	public string? LastName { get; set; }

	/// <summary>The contact's email address.</summary>
	public string? Email { get; set; }

	/// <summary>The contact's office phone number.</summary>
	public string? PhoneOffice { get; set; }

	/// <summary>The contact's mobile phone number.</summary>
	public string? PhoneMobile { get; set; }

	/// <summary>The contact's fax number.</summary>
	public string? Fax { get; set; }

	/// <summary>
	/// The contact's default role on the client's invoices (Invoice email default in the web UI): one of the
	/// <see cref="Models.InvoiceRecipientStatus"/> values. Harvest's default is <c>none</c>; it may be omitted.
	/// </summary>
	public string? InvoiceRecipientStatus { get; set; }

	/// <summary>Date and time the contact was created.</summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>Date and time the contact was last updated.</summary>
	public DateTime UpdatedAt { get; set; }
}