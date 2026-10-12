using Harvest.Dtos;
using System.Text.Json.Serialization;

namespace Harvest.Models;

/// <summary>The fields of a new estimate or invoice line item.</summary>
public abstract class LineItemCreationDto : CreationDto
{
	/// <summary>
	/// The name of an item category: an <see cref="EstimateItemCategory.Name"/> or an <see cref="InvoiceItemCategory.Name"/>.
	/// </summary>
	public required string Kind { get; set; }

	/// <summary>Text description of the line item.</summary>
	public string? Description { get; set; }

	/// <summary>The unit quantity of the item; Harvest's default is 1.</summary>
	public decimal? Quantity { get; set; }

	/// <summary>The individual price per unit.</summary>
	public required decimal UnitPrice { get; set; }

	/// <summary>Whether the document's tax percentage applies to this line item; Harvest's default is <see langword="false"/>.</summary>
	public bool? Taxed { get; set; }

	/// <summary>Whether the document's tax2 percentage applies to this line item; Harvest's default is <see langword="false"/>.</summary>
	public bool? Taxed2 { get; set; }
}

/// <summary>
/// A line item change in an estimate or invoice update: without an <see cref="Id"/> it adds a line item (then
/// <see cref="Kind"/> and <see cref="UnitPrice"/> are required); with one it updates that line item, or deletes it when
/// <see cref="Destroy"/> is <see langword="true"/>.
/// </summary>
public abstract class LineItemPatchDto : PatchDto
{
	/// <summary>Unique ID for the line item to update or delete; leave <see langword="null"/> to add a new line item.</summary>
	public long? Id { get; set; }

	/// <summary>
	/// The name of an item category: an <see cref="EstimateItemCategory.Name"/> or an <see cref="InvoiceItemCategory.Name"/>.
	/// Required by Harvest when adding a line item.
	/// </summary>
	public string? Kind { get; set; }

	/// <summary>Text description of the line item.</summary>
	public string? Description { get; set; }

	/// <summary>The unit quantity of the item; Harvest's default for a new line item is 1.</summary>
	public decimal? Quantity { get; set; }

	/// <summary>The individual price per unit. Required by Harvest when adding a line item.</summary>
	public decimal? UnitPrice { get; set; }

	/// <summary>Whether the document's tax percentage applies to this line item.</summary>
	public bool? Taxed { get; set; }

	/// <summary>Whether the document's tax2 percentage applies to this line item.</summary>
	public bool? Taxed2 { get; set; }

	/// <summary><see langword="true"/> to delete the line item with this <see cref="Id"/> (<c>_destroy</c>).</summary>
	[JsonPropertyName("_destroy")]
	public bool? Destroy { get; set; }
}