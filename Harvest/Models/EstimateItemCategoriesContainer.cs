namespace Harvest.Models;

/// <summary>A page of <c>GET /v2/estimate_item_categories</c>.</summary>
public class EstimateItemCategoriesContainer : ListContainerBase
{
	/// <summary>The estimate item categories on this page (<c>estimate_item_categories</c>).</summary>
	public List<EstimateItemCategory> EstimateItemCategories { get; set; } = [];
}