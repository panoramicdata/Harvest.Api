namespace Harvest.Models;

/// <summary>A line item of an <see cref="Invoice"/>.</summary>
public class InvoiceLineItem
{
	/// <summary>Unique ID for the line item.</summary>
	public long Id { get; set; }

	/// <summary>The associated project: its id, name and code; <see langword="null"/> for a line item without one.</summary>
	public ProjectReference? Project { get; set; }

	/// <summary>The name of an invoice item category (see <see cref="InvoiceItemCategory"/>).</summary>
	public string? Kind { get; set; }

	/// <summary>Text description of the line item.</summary>
	public string? Description { get; set; }

	/// <summary>The unit quantity of the item.</summary>
	public decimal? Quantity { get; set; }

	/// <summary>The individual price per unit.</summary>
	public decimal? UnitPrice { get; set; }

	/// <summary>The line item subtotal (quantity * unit_price).</summary>
	public decimal? Amount { get; set; }

	/// <summary>Whether the invoice's tax percentage applies to this line item.</summary>
	public bool Taxed { get; set; }

	/// <summary>Whether the invoice's tax2 percentage applies to this line item.</summary>
	public bool Taxed2 { get; set; }
}