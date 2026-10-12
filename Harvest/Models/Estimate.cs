namespace Harvest.Models;

/// <summary>An estimate (<see href="https://help.getharvest.com/api-v2/estimates-api/estimates/estimates/"/>).</summary>
public class Estimate : BillingDocument<EstimateLineItem>
{
	/// <summary>The total amount for the estimate, including any discounts and taxes.</summary>
	public decimal? Amount { get; set; }

	/// <summary>Date and time the estimate was accepted.</summary>
	public DateTime? AcceptedAt { get; set; }

	/// <summary>Date and time the estimate was declined.</summary>
	public DateTime? DeclinedAt { get; set; }
}