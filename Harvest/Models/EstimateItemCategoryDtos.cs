using Harvest.Dtos;

namespace Harvest.Models;

/// <summary>The body of <c>POST /v2/estimate_item_categories</c>.</summary>
public class EstimateItemCategoryCreationDto : CreationDto
{
	/// <summary>The name of the estimate item category.</summary>
	public required string Name { get; set; }
}

/// <summary>The body of <c>PATCH /v2/estimate_item_categories/{id}</c>.</summary>
public class EstimateItemCategoryPatchDto : PatchDto
{
	/// <summary>The name of the estimate item category.</summary>
	public string? Name { get; set; }
}