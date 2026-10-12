namespace Harvest.Models;

/// <summary>An estimate item category (<see href="https://help.getharvest.com/api-v2/estimates-api/estimates/estimate-item-categories/"/>).</summary>
public class EstimateItemCategory
{
	/// <summary>Unique ID for the estimate item category.</summary>
	public long Id { get; set; }

	/// <summary>The name of the estimate item category.</summary>
	public string? Name { get; set; }

	/// <summary>Date and time the estimate item category was created.</summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>Date and time the estimate item category was last updated.</summary>
	public DateTime UpdatedAt { get; set; }
}