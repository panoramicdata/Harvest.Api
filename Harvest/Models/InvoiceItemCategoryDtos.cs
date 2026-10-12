using Harvest.Dtos;

namespace Harvest.Models;

/// <summary>The body of <c>POST /v2/invoice_item_categories</c>.</summary>
public class InvoiceItemCategoryCreationDto : CreationDto
{
	/// <summary>The name of the invoice item category.</summary>
	public required string Name { get; set; }
}

/// <summary>The body of <c>PATCH /v2/invoice_item_categories/{id}</c>.</summary>
public class InvoiceItemCategoryPatchDto : PatchDto
{
	/// <summary>The name of the invoice item category.</summary>
	public string? Name { get; set; }
}