using Harvest.Dtos;

namespace Harvest.Models;

public class UserCreationDto : CreationDto
{
	[AliasAs("first_name")]
	public required string FirstName { get; set; }

	[AliasAs("last_name")]
	public required string LastName { get; set; }

	[AliasAs("email")]
	public required string Email { get; set; }

	[AliasAs("timezone")]
	public string? Timezone { get; set; }

	[AliasAs("has_access_to_all_future_projects")]
	public bool HasAccessToAllFutureProjects { get; set; }

	[AliasAs("is_contractor")]
	public bool IsContractor { get; set; }

	[AliasAs("is_active")]
	public bool IsActive { get; set; } = true;

	[AliasAs("weekly_capacity")]
	public int? WeeklyCapacity { get; set; }

	[AliasAs("default_hourly_rate")]
	public decimal? DefaultHourlyRate { get; set; }

	[AliasAs("cost_rate")]
	public decimal? CostRate { get; set; }

	[AliasAs("roles")]
	public List<string>? Roles { get; set; }
}

public class UserPatchDto : PatchDto
{
	[AliasAs("first_name")]
	public string? FirstName { get; set; }

	[AliasAs("last_name")]
	public string? LastName { get; set; }

	[AliasAs("email")]
	public string? Email { get; set; }

	[AliasAs("timezone")]
	public string? Timezone { get; set; }

	[AliasAs("has_access_to_all_future_projects")]
	public bool? HasAccessToAllFutureProjects { get; set; }

	[AliasAs("is_contractor")]
	public bool? IsContractor { get; set; }

	[AliasAs("is_active")]
	public bool? IsActive { get; set; }

	[AliasAs("weekly_capacity")]
	public int? WeeklyCapacity { get; set; }

	[AliasAs("default_hourly_rate")]
	public decimal? DefaultHourlyRate { get; set; }

	[AliasAs("cost_rate")]
	public decimal? CostRate { get; set; }

	[AliasAs("roles")]
	public List<string>? Roles { get; set; }
}