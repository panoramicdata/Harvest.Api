namespace Harvest.Models;

/// <summary>The fields an <see cref="Estimate"/> and an <see cref="Invoice"/> share.</summary>
/// <typeparam name="TLineItem">The document's line item type.</typeparam>
public abstract class BillingDocument<TLineItem> where TLineItem : LineItem
{
	/// <summary>Unique ID for the document.</summary>
	public long Id { get; set; }

	/// <summary>The document's client: its id and name.</summary>
	public ClientReference? Client { get; set; }

	/// <summary>The document's line items.</summary>
	public List<TLineItem>? LineItems { get; set; }

	/// <summary>The person that created the document: their id and name.</summary>
	public UserReference? Creator { get; set; }

	/// <summary>
	/// Used to build a URL to the public web document for your client, at
	/// <c>https://{ACCOUNT_SUBDOMAIN}.harvestapp.com/client/estimates/{client_key}</c> or
	/// <c>.../client/invoices/{client_key}</c> (add <c>.pdf</c> to an invoice's for a PDF version).
	/// </summary>
	public string? ClientKey { get; set; }

	/// <summary>The document number. Harvest generates one if none was set.</summary>
	public string? Number { get; set; }

	/// <summary>The purchase order number.</summary>
	public string? PurchaseOrder { get; set; }

	/// <summary>This percentage is applied to the subtotal, including line items and discounts.</summary>
	public decimal? Tax { get; set; }

	/// <summary>The first amount of tax included, calculated from <see cref="Tax"/>; <see langword="null"/> if no tax is defined.</summary>
	public decimal? TaxAmount { get; set; }

	/// <summary>This second percentage is applied to the subtotal, including line items and discounts.</summary>
	public decimal? Tax2 { get; set; }

	/// <summary>The amount calculated from <see cref="Tax2"/>.</summary>
	public decimal? Tax2Amount { get; set; }

	/// <summary>This percentage is subtracted from the subtotal.</summary>
	public decimal? Discount { get; set; }

	/// <summary>The amount calculated from <see cref="Discount"/>.</summary>
	public decimal? DiscountAmount { get; set; }

	/// <summary>The document subject.</summary>
	public string? Subject { get; set; }

	/// <summary>Any additional notes included on the document.</summary>
	public string? Notes { get; set; }

	/// <summary>The currency code associated with this document.</summary>
	public string? Currency { get; set; }

	/// <summary>
	/// The current state of the document: one of the <see cref="EstimateState"/> values for an estimate, or of the
	/// <see cref="InvoiceState"/> values for an invoice.
	/// </summary>
	public string? State { get; set; }

	/// <summary>Date the document was issued.</summary>
	public DateOnly? IssueDate { get; set; }

	/// <summary>Date and time the document was sent.</summary>
	public DateTime? SentAt { get; set; }

	/// <summary>Date and time the document was created.</summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>Date and time the document was last updated.</summary>
	public DateTime UpdatedAt { get; set; }
}