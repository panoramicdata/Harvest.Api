using Harvest.Dtos;

namespace Harvest.Models;

/// <summary>The body of <c>POST /v2/contacts</c>.</summary>
public class ContactCreationDto : CreationDto
{
	/// <summary>The ID of the client associated with this contact.</summary>
	public required long ClientId { get; set; }

	/// <summary>The title of the contact.</summary>
	public string? Title { get; set; }

	/// <summary>The first name of the contact.</summary>
	public required string FirstName { get; set; }

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
	/// The contact's default role on the client's invoices: one of the <see cref="Models.InvoiceRecipientStatus"/> values
	/// (<c>none</c>, <c>recipient</c>, <c>cc</c> or <c>bcc</c>); Harvest's default is <c>none</c>.
	/// </summary>
	public string? InvoiceRecipientStatus { get; set; }
}

/// <summary>The body of <c>PATCH /v2/contacts/{id}</c>.</summary>
public class ContactPatchDto : PatchDto
{
	/// <summary>The ID of the client associated with this contact.</summary>
	public long? ClientId { get; set; }

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
	/// The contact's default role on the client's invoices: one of the <see cref="Models.InvoiceRecipientStatus"/> values
	/// (<c>none</c>, <c>recipient</c>, <c>cc</c> or <c>bcc</c>).
	/// </summary>
	public string? InvoiceRecipientStatus { get; set; }
}