namespace Harvest.Models;

/// <summary>A page of <c>GET /v2/users/{userId}/billable_rates</c>.</summary>
public class BillableRatesContainer : ListContainerBase
{
	/// <summary>The billable rates on this page (<c>billable_rates</c>).</summary>
	public List<BillableRate> BillableRates { get; set; } = [];
}