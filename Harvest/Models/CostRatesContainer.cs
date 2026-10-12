namespace Harvest.Models;

/// <summary>A page of <c>GET /v2/users/{userId}/cost_rates</c>.</summary>
public class CostRatesContainer : ListContainerBase
{
	/// <summary>The cost rates on this page (<c>cost_rates</c>).</summary>
	public List<CostRate> CostRates { get; set; } = [];
}