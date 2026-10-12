namespace Harvest.Models;

/// <summary>A page of <c>GET /v2/invoices/{INVOICE_ID}/payments</c>.</summary>
public class InvoicePaymentsContainer : ListContainerBase
{
	/// <summary>The invoice payments on this page (<c>invoice_payments</c>).</summary>
	public List<InvoicePayment> InvoicePayments { get; set; } = [];
}