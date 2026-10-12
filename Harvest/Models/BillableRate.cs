namespace Harvest.Models;

/// <summary>A billable rate of a user (<see href="https://help.getharvest.com/api-v2/users-api/users/billable-rates/"/>).</summary>
public class BillableRate
{
	/// <summary>Unique ID for the billable rate.</summary>
	public long Id { get; set; }

	/// <summary>The amount of the billable rate.</summary>
	public decimal? Amount { get; set; }

	/// <summary>The date the billable rate is effective.</summary>
	public DateOnly? StartDate { get; set; }

	/// <summary>The date the billable rate is no longer effective, calculated by Harvest; <see langword="null"/> for the current rate.</summary>
	public DateOnly? EndDate { get; set; }

	/// <summary>Date and time the billable rate was created.</summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>Date and time the billable rate was last updated.</summary>
	public DateTime UpdatedAt { get; set; }
}