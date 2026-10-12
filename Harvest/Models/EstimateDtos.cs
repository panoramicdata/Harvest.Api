namespace Harvest.Models;

/// <summary>The body of <c>POST /v2/estimates</c>.</summary>
public class EstimateCreationDto : BillingDocumentDto<EstimateLineItemCreationDto>
{
	/// <summary>The ID of the client this estimate belongs to.</summary>
	public required long ClientId { get; set; }
}

/// <summary>A line item in <see cref="EstimateCreationDto"/>'s line items.</summary>
public class EstimateLineItemCreationDto : LineItemCreationDto
{
}

/// <summary>
/// The body of <c>PATCH /v2/estimates/{id}</c>. Its line items add (no <see cref="LineItemPatchDto.Id"/>), update (with
/// an ID) or delete (<see cref="EstimateLineItemPatchDto.Destroying"/>) line items; line items not listed are left
/// unchanged.
/// </summary>
public class EstimatePatchDto : BillingDocumentDto<EstimateLineItemPatchDto>
{
	/// <summary>The ID of the client this estimate belongs to.</summary>
	public long? ClientId { get; set; }
}

/// <summary>A line item change in <see cref="EstimatePatchDto"/>'s line items.</summary>
public class EstimateLineItemPatchDto : LineItemPatchDto
{
	/// <summary>A line item change that deletes the line item: <c>{"id":…,"_destroy":true}</c>.</summary>
	/// <param name="id">The ID of the line item to delete.</param>
	/// <returns>The line item change.</returns>
	public static EstimateLineItemPatchDto Destroying(long id) => new() { Id = id, Destroy = true };
}