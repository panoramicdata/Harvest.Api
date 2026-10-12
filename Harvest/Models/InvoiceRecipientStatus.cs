namespace Harvest.Models;

/// <summary>
/// The values of a contact's <c>invoice_recipient_status</c>: its default role on the client's invoices. They are strings
/// rather than an enum so that a value Harvest adds later is still read.
/// </summary>
public static class InvoiceRecipientStatus
{
	/// <summary>Not a default recipient (Harvest's default).</summary>
	public const string None = "none";

	/// <summary>A default recipient.</summary>
	public const string Recipient = "recipient";

	/// <summary>Copied by default.</summary>
	public const string Cc = "cc";

	/// <summary>Blind-copied by default.</summary>
	public const string Bcc = "bcc";
}