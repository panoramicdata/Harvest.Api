using Harvest.Dtos;

namespace Harvest.Models;

/// <summary>The body of <c>POST /v2/invoices/{INVOICE_ID}/payments</c>.</summary>
public class InvoicePaymentCreationDto : CreationDto
{
	/// <summary>The amount of the payment.</summary>
	public required decimal Amount { get; set; }

	/// <summary>
	/// Date and time the payment was made; give it a <see cref="DateTimeKind"/> of <see cref="DateTimeKind.Utc"/> so it is
	/// sent with a <c>Z</c>. Pass either this or <see cref="PaidDate"/>, but not both.
	/// </summary>
	public DateTime? PaidAt { get; set; }

	/// <summary>Date the payment was made. Pass either this or <see cref="PaidAt"/>, but not both.</summary>
	public DateOnly? PaidDate { get; set; }

	/// <summary>Any notes to be associated with the payment.</summary>
	public string? Notes { get; set; }

	/// <summary>
	/// Whether to send a thank you email (if enabled for the account in Invoices &gt; Configure &gt; Messages); it is only
	/// sent if the invoice is fully paid after this payment. Harvest's default is <see langword="true"/>.
	/// </summary>
	public bool? SendThankYou { get; set; }
}