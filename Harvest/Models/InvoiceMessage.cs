namespace Harvest.Models;

/// <summary>A message on an invoice (<see href="https://help.getharvest.com/api-v2/invoices-api/invoices/invoice-messages/"/>).</summary>
public class InvoiceMessage
{
	/// <summary>Unique ID for the message.</summary>
	public long Id { get; set; }

	/// <summary>Name of the user that created the message.</summary>
	public string? SentBy { get; set; }

	/// <summary>Email of the user that created the message.</summary>
	public string? SentByEmail { get; set; }

	/// <summary>Name of the user that the message was sent from.</summary>
	public string? SentFrom { get; set; }

	/// <summary>Email of the user that message was sent from.</summary>
	public string? SentFromEmail { get; set; }

	/// <summary>The message's recipients.</summary>
	public List<InvoiceMessageRecipient>? Recipients { get; set; }

	/// <summary>The message subject.</summary>
	public string? Subject { get; set; }

	/// <summary>The message body.</summary>
	public string? Body { get; set; }

	/// <summary>
	/// Deprecated by Harvest: <see langword="true"/> when payment options are assigned to the invoice and
	/// <see langword="false"/> when there are none.
	/// </summary>
	public bool IncludeLinkToClientInvoice { get; set; }

	/// <summary>Whether to attach the invoice PDF to the message email.</summary>
	public bool AttachPdf { get; set; }

	/// <summary>Whether to email a copy of the message to the current user.</summary>
	public bool SendMeACopy { get; set; }

	/// <summary>Whether this is a thank you message.</summary>
	public bool ThankYou { get; set; }

	/// <summary>
	/// The type of invoice event that occurred with the message: one of the <see cref="InvoiceMessageEventType"/> values
	/// (<c>close</c>, <c>draft</c>, <c>re-open</c> or <c>send</c>, which marked the invoice as sent); <see langword="null"/>
	/// means the message was sent.
	/// </summary>
	public string? EventType { get; set; }

	/// <summary>Whether this is a reminder message.</summary>
	public bool Reminder { get; set; }

	/// <summary>The date the reminder email will be sent.</summary>
	public DateOnly? SendReminderOn { get; set; }

	/// <summary>Date and time the message was created.</summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>Date and time the message was last updated.</summary>
	public DateTime UpdatedAt { get; set; }
}

/// <summary>A recipient of an <see cref="InvoiceMessage"/>.</summary>
public class InvoiceMessageRecipient
{
	/// <summary>Name of the message recipient.</summary>
	public string? Name { get; set; }

	/// <summary>Email of the message recipient.</summary>
	public string? Email { get; set; }
}

/// <summary>
/// The subject and body Harvest has configured for a new message on an invoice, from
/// <c>GET /v2/invoices/{INVOICE_ID}/messages/new</c>.
/// </summary>
public class InvoiceMessageTemplate
{
	/// <summary>The ID of the invoice.</summary>
	public long InvoiceId { get; set; }

	/// <summary>The message subject.</summary>
	public string? Subject { get; set; }

	/// <summary>The message body.</summary>
	public string? Body { get; set; }

	/// <summary>Whether this is a reminder message.</summary>
	public bool Reminder { get; set; }

	/// <summary>Whether this is a thank you message.</summary>
	public bool ThankYou { get; set; }
}