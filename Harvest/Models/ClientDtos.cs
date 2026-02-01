using Harvest.Dtos;

namespace Harvest.Models;

public class ClientCreationDto : CreationDto
{
	[AliasAs("name")]
	public required string Name { get; set; }

	[AliasAs("is_active")]
	public bool IsActive { get; set; } = true;

	[AliasAs("address")]
	public string? Address { get; set; }

	[AliasAs("currency")]
	public string? Currency { get; set; }
}

public class ClientPatchDto : PatchDto
{
	[AliasAs("name")]
	public string? Name { get; set; }

	[AliasAs("is_active")]
	public bool? IsActive { get; set; }

	[AliasAs("address")]
	public string? Address { get; set; }

	[AliasAs("currency")]
	public string? Currency { get; set; }
}