namespace Harvest.Models;

/// <summary>A message on an invoice (<see href="https://help.getharvest.com/api-v2/invoices-api/invoices/invoice-messages/"/>).</summary>
public class InvoiceMessage : DocumentMessage
{
	/// <summary>
	/// Deprecated by Harvest: <see langword="true"/> when payment options are assigned to the invoice and
	/// <see langword="false"/> when there are none.
	/// </summary>
	public bool IncludeLinkToClientInvoice { get; set; }

	/// <summary>Whether to attach the invoice PDF to the message email.</summary>
	public bool AttachPdf { get; set; }

	/// <summary>Whether this is a thank you message.</summary>
	public bool ThankYou { get; set; }

	/// <summary>Whether this is a reminder message.</summary>
	public bool Reminder { get; set; }

	/// <summary>The date the reminder email will be sent.</summary>
	public DateOnly? SendReminderOn { get; set; }
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