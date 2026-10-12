namespace Harvest.Models;

/// <summary>The body of <c>POST /v2/contacts</c>.</summary>
public class ContactCreationDto : ContactDetails
{
	/// <summary>The ID of the client associated with this contact.</summary>
	public required long ClientId { get; set; }

	/// <summary>The first name of the contact.</summary>
	public required string FirstName { get; set; }
}

/// <summary>The body of <c>PATCH /v2/contacts/{id}</c>.</summary>
public class ContactPatchDto : ContactDetails
{
	/// <summary>The ID of the client associated with this contact.</summary>
	public long? ClientId { get; set; }

	/// <summary>The first name of the contact.</summary>
	public string? FirstName { get; set; }
}