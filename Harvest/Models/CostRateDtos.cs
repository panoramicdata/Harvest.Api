using Harvest.Dtos;

namespace Harvest.Models;

/// <summary>The body of <c>POST /v2/users/{userId}/cost_rates</c>.</summary>
public class CostRateCreationDto : CreationDto
{
	/// <summary>The amount of the cost rate.</summary>
	public required decimal Amount { get; set; }

	/// <summary>
	/// The date the cost rate is effective; cannot be in the future. Leave <see langword="null"/> to replace all of the
	/// user's existing rates.
	/// </summary>
	public DateOnly? StartDate { get; set; }
}