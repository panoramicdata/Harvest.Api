using Harvest.Dtos;

namespace Harvest.Models;

/// <summary>A recipient of an <see cref="EstimateMessage"/> or an <see cref="InvoiceMessage"/>.</summary>
public class MessageRecipient
{
	/// <summary>Name of the message recipient.</summary>
	public string? Name { get; set; }

	/// <summary>Email of the message recipient.</summary>
	public string? Email { get; set; }
}

/// <summary>
/// A recipient in <see cref="EstimateMessageCreationDto.Recipients"/> or <see cref="InvoiceMessageCreationDto.Recipients"/>.
/// </summary>
public class MessageRecipientCreationDto : CreationDto
{
	/// <summary>Name of the message recipient.</summary>
	public string? Name { get; set; }

	/// <summary>Email of the message recipient.</summary>
	public required string Email { get; set; }
}