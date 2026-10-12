namespace Harvest.Models;

/// <summary>A cost rate of a user (<see href="https://help.getharvest.com/api-v2/users-api/users/cost-rates/"/>).</summary>
public class CostRate
{
	/// <summary>Unique ID for the cost rate.</summary>
	public long Id { get; set; }

	/// <summary>The amount of the cost rate.</summary>
	public decimal? Amount { get; set; }

	/// <summary>The date the cost rate is effective.</summary>
	public DateOnly? StartDate { get; set; }

	/// <summary>The date the cost rate is no longer effective, calculated by Harvest; <see langword="null"/> for the current rate.</summary>
	public DateOnly? EndDate { get; set; }

	/// <summary>Date and time the cost rate was created.</summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>Date and time the cost rate was last updated.</summary>
	public DateTime UpdatedAt { get; set; }
}