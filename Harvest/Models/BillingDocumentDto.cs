namespace Harvest.Models;

/// <summary>
/// The optional fields the bodies that create and update an estimate or an invoice share. As in every request body, a
/// <see langword="null"/> field is omitted from the JSON: on creation Harvest applies its default, on update it leaves the
/// field unchanged.
/// </summary>
/// <typeparam name="TLineItem">The body's line item type.</typeparam>
public abstract class BillingDocumentDto<TLineItem> where TLineItem : class
{
	/// <summary>The document number; Harvest generates one if none is set.</summary>
	public string? Number { get; set; }

	/// <summary>The purchase order number.</summary>
	public string? PurchaseOrder { get; set; }

	/// <summary>
	/// This percentage is applied to the subtotal, including line items and discounts. Example: use 10.0 for 10.0%.
	/// </summary>
	public decimal? Tax { get; set; }

	/// <summary>
	/// This second percentage is applied to the subtotal, including line items and discounts. Example: use 10.0 for 10.0%.
	/// </summary>
	public decimal? Tax2 { get; set; }

	/// <summary>This percentage is subtracted from the subtotal. Example: use 10.0 for 10.0%.</summary>
	public decimal? Discount { get; set; }

	/// <summary>The document subject.</summary>
	public string? Subject { get; set; }

	/// <summary>Any additional notes to include on the document.</summary>
	public string? Notes { get; set; }

	/// <summary>The currency used by the document; Harvest's default is the client's currency.</summary>
	public string? Currency { get; set; }

	/// <summary>Date the document was issued; Harvest's default is today.</summary>
	public DateOnly? IssueDate { get; set; }

	/// <summary>
	/// On creation, the document's line items. On update, line items to add (no ID), update (with an ID) or delete (with an
	/// ID and <c>_destroy</c>); line items not listed are left unchanged.
	/// </summary>
	public List<TLineItem>? LineItems { get; set; }
}