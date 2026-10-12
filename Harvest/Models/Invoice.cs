namespace Harvest.Models;

/// <summary>An invoice (<see href="https://help.getharvest.com/api-v2/invoices-api/invoices/invoices/"/>).</summary>
public class Invoice
{
	/// <summary>Unique ID for the invoice.</summary>
	public long Id { get; set; }

	/// <summary>The invoice's client: its id and name.</summary>
	public ClientReference? Client { get; set; }

	/// <summary>The invoice's line items.</summary>
	public List<InvoiceLineItem>? LineItems { get; set; }

	/// <summary>The associated estimate (its id), or <see langword="null"/> if there is none.</summary>
	public EstimateReference? Estimate { get; set; }

	/// <summary>The associated retainer (its id), or <see langword="null"/> if there is none.</summary>
	public RetainerReference? Retainer { get; set; }

	/// <summary>The person that created the invoice: their id and name.</summary>
	public UserReference? Creator { get; set; }

	/// <summary>
	/// Used to build a URL to the public web invoice for your client by adding <c>/client/invoices/{CLIENT_KEY}</c> to your
	/// account URL <c>https://{SUBDOMAIN}.harvestapp.com/</c>; add <c>.pdf</c> for a PDF version.
	/// </summary>
	public string? ClientKey { get; set; }

	/// <summary>The invoice number. Harvest generates one if none was set.</summary>
	public string? Number { get; set; }

	/// <summary>The purchase order number.</summary>
	public string? PurchaseOrder { get; set; }

	/// <summary>The total amount for the invoice, including any discounts and taxes.</summary>
	public decimal Amount { get; set; }

	/// <summary>The total amount due at this time for this invoice.</summary>
	public decimal DueAmount { get; set; }

	/// <summary>This percentage is applied to the subtotal, including line items and discounts.</summary>
	public decimal? Tax { get; set; }

	/// <summary>The first amount of tax included, calculated from <see cref="Tax"/>; <see langword="null"/> if no tax is defined.</summary>
	public decimal? TaxAmount { get; set; }

	/// <summary>This percentage is applied to the subtotal, including line items and discounts.</summary>
	public decimal? Tax2 { get; set; }

	/// <summary>The amount calculated from <see cref="Tax2"/>.</summary>
	public decimal? Tax2Amount { get; set; }

	/// <summary>This percentage is subtracted from the subtotal.</summary>
	public decimal? Discount { get; set; }

	/// <summary>The amount calculated from <see cref="Discount"/>.</summary>
	public decimal? DiscountAmount { get; set; }

	/// <summary>The invoice subject.</summary>
	public string? Subject { get; set; }

	/// <summary>Any additional notes included on the invoice.</summary>
	public string? Notes { get; set; }

	/// <summary>The currency code associated with this invoice.</summary>
	public string? Currency { get; set; }

	/// <summary>The current state of the invoice: one of the <see cref="InvoiceState"/> values (<c>draft</c>, <c>open</c>, <c>paid</c> or <c>closed</c>).</summary>
	public string? State { get; set; }

	/// <summary>Start of the period during which time entries were added to this invoice.</summary>
	public DateOnly? PeriodStart { get; set; }

	/// <summary>End of the period during which time entries were added to this invoice.</summary>
	public DateOnly? PeriodEnd { get; set; }

	/// <summary>Date the invoice was issued.</summary>
	public DateOnly? IssueDate { get; set; }

	/// <summary>Date the invoice is due.</summary>
	public DateOnly? DueDate { get; set; }

	/// <summary>
	/// The timeframe in which the invoice should be paid: one of the <see cref="InvoicePaymentTerm"/> values
	/// (<c>upon receipt</c>, <c>net 15</c>, <c>net 30</c>, <c>net 45</c>, <c>net 60</c> or <c>custom</c>).
	/// </summary>
	public string? PaymentTerm { get; set; }

	/// <summary>
	/// The payment options enabled for the invoice: <see cref="InvoicePaymentOption"/> values (<c>ach</c>,
	/// <c>credit_card</c>, <c>paypal</c>).
	/// </summary>
	public List<string>? PaymentOptions { get; set; }

	/// <summary>Date and time the invoice was sent.</summary>
	public DateTime? SentAt { get; set; }

	/// <summary>Date and time the invoice was paid.</summary>
	public DateTime? PaidAt { get; set; }

	/// <summary>Date the invoice was paid.</summary>
	public DateOnly? PaidDate { get; set; }

	/// <summary>Date and time the invoice was closed.</summary>
	public DateTime? ClosedAt { get; set; }

	/// <summary>Unique ID of the associated recurring invoice.</summary>
	public long? RecurringInvoiceId { get; set; }

	/// <summary>Date and time the invoice was created.</summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>Date and time the invoice was last updated.</summary>
	public DateTime UpdatedAt { get; set; }
}