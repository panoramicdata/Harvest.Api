namespace Harvest.Models;

/// <summary>The fields a user's <see cref="BillableRate"/> and <see cref="CostRate"/> share.</summary>
public abstract class UserRate
{
	/// <summary>Unique ID for the rate.</summary>
	public long Id { get; set; }

	/// <summary>The amount of the rate.</summary>
	public decimal? Amount { get; set; }

	/// <summary>The date the rate is effective.</summary>
	public DateOnly? StartDate { get; set; }

	/// <summary>The date the rate is no longer effective, calculated by Harvest; <see langword="null"/> for the current rate.</summary>
	public DateOnly? EndDate { get; set; }

	/// <summary>Date and time the rate was created.</summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>Date and time the rate was last updated.</summary>
	public DateTime UpdatedAt { get; set; }
}