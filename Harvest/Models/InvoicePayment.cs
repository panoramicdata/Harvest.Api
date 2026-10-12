namespace Harvest.Models;

/// <summary>A payment on an invoice (<see href="https://help.getharvest.com/api-v2/invoices-api/invoices/invoice-payments/"/>).</summary>
public class InvoicePayment
{
	/// <summary>Unique ID for the payment.</summary>
	public long Id { get; set; }

	/// <summary>The amount of the payment.</summary>
	public decimal Amount { get; set; }

	/// <summary>Date and time the payment was made.</summary>
	public DateTime? PaidAt { get; set; }

	/// <summary>Date the payment was made.</summary>
	public DateOnly? PaidDate { get; set; }

	/// <summary>The name of the person who recorded the payment.</summary>
	public string? RecordedBy { get; set; }

	/// <summary>The email of the person who recorded the payment.</summary>
	public string? RecordedByEmail { get; set; }

	/// <summary>Any notes associated with the payment.</summary>
	public string? Notes { get; set; }

	/// <summary>Either the card authorization or PayPal transaction ID.</summary>
	public string? TransactionId { get; set; }

	/// <summary>The payment gateway id and name used to process the payment.</summary>
	public PaymentGatewayReference? PaymentGateway { get; set; }

	/// <summary>Date and time the payment was recorded.</summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>Date and time the payment was last updated.</summary>
	public DateTime UpdatedAt { get; set; }
}