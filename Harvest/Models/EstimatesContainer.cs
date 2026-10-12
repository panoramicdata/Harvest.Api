namespace Harvest.Models;

/// <summary>A page of <c>GET /v2/estimates</c>.</summary>
public class EstimatesContainer : ListContainerBase
{
	/// <summary>The estimates on this page (<c>estimates</c>).</summary>
	public List<Estimate> Estimates { get; set; } = [];
}