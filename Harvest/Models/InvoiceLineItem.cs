namespace Harvest.Models;

/// <summary>A line item of an <see cref="Invoice"/>.</summary>
public class InvoiceLineItem : LineItem
{
	/// <summary>The associated project: its id, name and code; <see langword="null"/> for a line item without one.</summary>
	public ProjectReference? Project { get; set; }
}