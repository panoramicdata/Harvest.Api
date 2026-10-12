namespace Harvest.Models;

/// <summary>
/// A client contact's details beyond their first name, as a <see cref="Contact"/> reports them and as the bodies that
/// create and update a contact set them.
/// </summary>
public abstract class ContactDetails
{
	/// <summary>The title of the contact.</summary>
	public string? Title { get; set; }

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
	/// <see cref="Models.InvoiceRecipientStatus"/> values (<c>none</c>, <c>recipient</c>, <c>cc</c> or <c>bcc</c>).
	/// Harvest's default is <c>none</c>; a response may omit it.
	/// </summary>
	public string? InvoiceRecipientStatus { get; set; }
}