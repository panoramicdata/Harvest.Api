using Harvest.Dtos;
using System.Text.Json.Serialization;

namespace Harvest.Models;

/// <summary>The body of <c>POST /v2/estimates</c>.</summary>
public class EstimateCreationDto : CreationDto
{
	/// <summary>The ID of the client this estimate belongs to.</summary>
	public required long ClientId { get; set; }

	/// <summary>The estimate number; if no value is set, the number will be automatically generated.</summary>
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

	/// <summary>The estimate subject.</summary>
	public string? Subject { get; set; }

	/// <summary>Any additional notes to include on the estimate.</summary>
	public string? Notes { get; set; }

	/// <summary>The currency used by the estimate; if not provided, the client's currency will be used.</summary>
	public string? Currency { get; set; }

	/// <summary>Date the estimate was issued; Harvest's default is today's date.</summary>
	public DateOnly? IssueDate { get; set; }

	/// <summary>The line items of the new estimate.</summary>
	public List<EstimateLineItemCreationDto>? LineItems { get; set; }
}

/// <summary>A line item in <see cref="EstimateCreationDto.LineItems"/>.</summary>
public class EstimateLineItemCreationDto : CreationDto
{
	/// <summary>The name of an estimate item category (<see cref="EstimateItemCategory.Name"/>).</summary>
	public required string Kind { get; set; }

	/// <summary>Text description of the line item.</summary>
	public string? Description { get; set; }

	/// <summary>The unit quantity of the item; Harvest's default is 1.</summary>
	public decimal? Quantity { get; set; }

	/// <summary>The individual price per unit.</summary>
	public required decimal UnitPrice { get; set; }

	/// <summary>Whether the estimate's tax percentage applies to this line item; Harvest's default is <see langword="false"/>.</summary>
	public bool? Taxed { get; set; }

	/// <summary>Whether the estimate's tax2 percentage applies to this line item; Harvest's default is <see langword="false"/>.</summary>
	public bool? Taxed2 { get; set; }
}

/// <summary>The body of <c>PATCH /v2/estimates/{id}</c>.</summary>
public class EstimatePatchDto : PatchDto
{
	/// <summary>The ID of the client this estimate belongs to.</summary>
	public long? ClientId { get; set; }

	/// <summary>The estimate number.</summary>
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

	/// <summary>The estimate subject.</summary>
	public string? Subject { get; set; }

	/// <summary>Any additional notes to include on the estimate.</summary>
	public string? Notes { get; set; }

	/// <summary>The currency used by the estimate.</summary>
	public string? Currency { get; set; }

	/// <summary>Date the estimate was issued.</summary>
	public DateOnly? IssueDate { get; set; }

	/// <summary>
	/// Line items to add (no <see cref="EstimateLineItemPatchDto.Id"/>), update (with an
	/// <see cref="EstimateLineItemPatchDto.Id"/>) or delete (<see cref="EstimateLineItemPatchDto.Destroying"/>). Line items
	/// not listed are left unchanged.
	/// </summary>
	public List<EstimateLineItemPatchDto>? LineItems { get; set; }
}

/// <summary>
/// A line item change in <see cref="EstimatePatchDto.LineItems"/>: without an <see cref="Id"/> it creates a line item
/// (then <see cref="Kind"/> and <see cref="UnitPrice"/> are required); with one it updates that line item, or deletes it
/// when <see cref="Destroy"/> is <see langword="true"/>.
/// </summary>
public class EstimateLineItemPatchDto : PatchDto
{
	/// <summary>Unique ID for the line item to update or delete; leave <see langword="null"/> to add a new line item.</summary>
	public long? Id { get; set; }

	/// <summary>The name of an estimate item category (<see cref="EstimateItemCategory.Name"/>).</summary>
	public string? Kind { get; set; }

	/// <summary>Text description of the line item.</summary>
	public string? Description { get; set; }

	/// <summary>The unit quantity of the item; Harvest's default for a new line item is 1.</summary>
	public decimal? Quantity { get; set; }

	/// <summary>The individual price per unit.</summary>
	public decimal? UnitPrice { get; set; }

	/// <summary>Whether the estimate's tax percentage applies to this line item.</summary>
	public bool? Taxed { get; set; }

	/// <summary>Whether the estimate's tax2 percentage applies to this line item.</summary>
	public bool? Taxed2 { get; set; }

	/// <summary><see langword="true"/> to delete the line item with this <see cref="Id"/> (<c>_destroy</c>).</summary>
	[JsonPropertyName("_destroy")]
	public bool? Destroy { get; set; }

	/// <summary>A line item change that deletes the line item: <c>{"id":…,"_destroy":true}</c>.</summary>
	/// <param name="id">The ID of the line item to delete.</param>
	/// <returns>The line item change.</returns>
	public static EstimateLineItemPatchDto Destroying(long id) => new() { Id = id, Destroy = true };
}