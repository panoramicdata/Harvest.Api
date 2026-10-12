namespace Harvest.Models;

/// <summary>An invoice (<see href="https://help.getharvest.com/api-v2/invoices-api/invoices/invoices/"/>).</summary>
public class Invoice : BillingDocument<InvoiceLineItem>
{
	/// <summary>The associated estimate (its id), or <see langword="null"/> if there is none.</summary>
	public EstimateReference? Estimate { get; set; }

	/// <summary>The associated retainer (its id), or <see langword="null"/> if there is none.</summary>
	public RetainerReference? Retainer { get; set; }

	/// <summary>The total amount for the invoice, including any discounts and taxes.</summary>
	public decimal Amount { get; set; }

	/// <summary>The total amount due at this time for this invoice.</summary>
	public decimal DueAmount { get; set; }

	/// <summary>Start of the period during which time entries were added to this invoice.</summary>
	public DateOnly? PeriodStart { get; set; }

	/// <summary>End of the period during which time entries were added to this invoice.</summary>
	public DateOnly? PeriodEnd { get; set; }

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

	/// <summary>Date and time the invoice was paid.</summary>
	public DateTime? PaidAt { get; set; }

	/// <summary>Date the invoice was paid.</summary>
	public DateOnly? PaidDate { get; set; }

	/// <summary>Date and time the invoice was closed.</summary>
	public DateTime? ClosedAt { get; set; }

	/// <summary>Unique ID of the associated recurring invoice.</summary>
	public long? RecurringInvoiceId { get; set; }
}