namespace Harvest.Models;

/// <summary>A page of <c>GET /v2/invoices/{INVOICE_ID}/messages</c>.</summary>
public class InvoiceMessagesContainer : ListContainerBase
{
	/// <summary>The invoice messages on this page (<c>invoice_messages</c>).</summary>
	public List<InvoiceMessage> InvoiceMessages { get; set; } = [];
}