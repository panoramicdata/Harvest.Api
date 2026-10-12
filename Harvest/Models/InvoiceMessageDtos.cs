using Harvest.Dtos;

namespace Harvest.Models;

/// <summary>
/// The body of <c>POST /v2/invoices/{INVOICE_ID}/messages</c>. Without an <see cref="EventType"/> the message is created
/// and sent, so it needs <see cref="Recipients"/> or <see cref="SendMeACopy"/> set to <see langword="true"/> (Harvest
/// refuses a message with no recipients). With an <see cref="EventType"/> (<see cref="InvoiceMessageEventType"/>) it
/// changes the invoice's state instead and sends nothing.
/// </summary>
public class InvoiceMessageCreationDto : CreationDto
{
	/// <summary>The message's recipients.</summary>
	public List<MessageRecipientCreationDto>? Recipients { get; set; }

	/// <summary>The message subject.</summary>
	public string? Subject { get; set; }

	/// <summary>The message body.</summary>
	public string? Body { get; set; }

	/// <summary>
	/// Deprecated by Harvest: a link to the client invoice is included automatically when the invoice has payment options.
	/// <see langword="true"/> is ignored; <see langword="false"/> clears all payment options on the invoice.
	/// </summary>
	public bool? IncludeLinkToClientInvoice { get; set; }

	/// <summary>Whether to attach a PDF of the invoice to the message email; Harvest's default is <see langword="false"/>.</summary>
	public bool? AttachPdf { get; set; }

	/// <summary>Whether to email a copy of the message to the current user; Harvest's default is <see langword="false"/>.</summary>
	public bool? SendMeACopy { get; set; }

	/// <summary>Whether to send a thank you message email; Harvest's default is <see langword="false"/>.</summary>
	public bool? ThankYou { get; set; }

	/// <summary>
	/// Omit (<see langword="null"/>, Harvest's default) to create and send the message. Otherwise one of the
	/// <see cref="InvoiceMessageEventType"/> values: <c>send</c> (marks a draft invoice as sent without sending the message),
	/// <c>close</c>, <c>draft</c> or <c>re-open</c>.
	/// </summary>
	public string? EventType { get; set; }
}