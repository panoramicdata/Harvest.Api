namespace Harvest.Models;

/// <summary>The fields an <see cref="EstimateLineItem"/> and an <see cref="InvoiceLineItem"/> share.</summary>
public abstract class LineItem
{
	/// <summary>Unique ID for the line item.</summary>
	public long Id { get; set; }

	/// <summary>
	/// The name of an item category: an <see cref="EstimateItemCategory.Name"/> or an <see cref="InvoiceItemCategory.Name"/>.
	/// </summary>
	public string? Kind { get; set; }

	/// <summary>Text description of the line item.</summary>
	public string? Description { get; set; }

	/// <summary>
	/// The unit quantity of the item. Harvest documents an integer for estimates but returns decimals (<c>1.0</c>), so it is
	/// read as a <see cref="decimal"/>.
	/// </summary>
	public decimal? Quantity { get; set; }

	/// <summary>The individual price per unit.</summary>
	public decimal? UnitPrice { get; set; }

	/// <summary>The line item subtotal (quantity * unit_price).</summary>
	public decimal? Amount { get; set; }

	/// <summary>Whether the document's tax percentage applies to this line item.</summary>
	public bool Taxed { get; set; }

	/// <summary>Whether the document's tax2 percentage applies to this line item.</summary>
	public bool Taxed2 { get; set; }
}