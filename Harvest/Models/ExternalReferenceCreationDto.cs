using Harvest.Dtos;

namespace Harvest.Models;

/// <summary>The external reference to attach to a time entry when creating or updating it.</summary>
public class ExternalReferenceCreationDto : CreationDto
{
	/// <summary>The ID of the item in the external service.</summary>
	public string? Id { get; set; }

	/// <summary>The ID of the group the item belongs to.</summary>
	public string? GroupId { get; set; }

	/// <summary>The ID of the account in the external service.</summary>
	public string? AccountId { get; set; }

	/// <summary>The URL of the item.</summary>
	public string? Permalink { get; set; }
}