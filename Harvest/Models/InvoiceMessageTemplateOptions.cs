namespace Harvest.Models;

/// <summary>
/// Which message template <c>GET /v2/invoices/{INVOICE_ID}/messages/new</c> returns. Leave both <see langword="null"/> (or
/// pass <see langword="null"/> for the whole object) for a general invoice message.
/// </summary>
public class InvoiceMessageTemplateOptions
{
	/// <summary>Set to <see langword="true"/> for the subject and body of a thank-you message (<c>thank_you</c>).</summary>
	[AliasAs("thank_you")]
	public bool? ThankYou { get; init; }

	/// <summary>Set to <see langword="true"/> for the subject and body of a reminder message (<c>reminder</c>).</summary>
	[AliasAs("reminder")]
	public bool? Reminder { get; init; }
}