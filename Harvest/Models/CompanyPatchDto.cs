using Harvest.Dtos;

namespace Harvest.Models;

/// <summary>The body of <c>PATCH /v2/company</c>. Harvest documents only these two settings as changeable.</summary>
public class CompanyPatchDto : PatchDto
{
	/// <summary>Whether time is tracked via duration (<see langword="false"/>) or start and end times (<see langword="true"/>).</summary>
	public bool? WantsTimestampTimers { get; set; }

	/// <summary>The weekly capacity in seconds.</summary>
	public int? WeeklyCapacity { get; set; }
}