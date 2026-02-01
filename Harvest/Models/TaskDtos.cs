using Harvest.Dtos;

namespace Harvest.Models;

public class TaskCreationDto : CreationDto
{
	[AliasAs("name")]
	public required string Name { get; set; }

	[AliasAs("billable_by_default")]
	public bool BillableByDefault { get; set; }

	[AliasAs("default_hourly_rate")]
	public decimal? DefaultHourlyRate { get; set; }

	[AliasAs("is_default")]
	public bool IsDefault { get; set; }

	[AliasAs("is_active")]
	public bool IsActive { get; set; } = true;
}

public class TaskPatchDto : PatchDto
{
	[AliasAs("name")]
	public string? Name { get; set; }

	[AliasAs("billable_by_default")]
	public bool? BillableByDefault { get; set; }

	[AliasAs("default_hourly_rate")]
	public decimal? DefaultHourlyRate { get; set; }

	[AliasAs("is_default")]
	public bool? IsDefault { get; set; }

	[AliasAs("is_active")]
	public bool? IsActive { get; set; }
}