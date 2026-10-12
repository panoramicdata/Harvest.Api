using Harvest.Dtos;

namespace Harvest.Models;

/// <summary>The body of <c>POST /v2/clients</c>.</summary>
public class ClientCreationDto : CreationDto
{
	/// <summary>A textual description of the client.</summary>
	public required string Name { get; set; }

	/// <summary>Whether the client is active or archived; Harvest's default is <see langword="true"/>.</summary>
	public bool? IsActive { get; set; }

	/// <summary>A textual representation of the client's physical address. May include new line characters.</summary>
	public string? Address { get; set; }

	/// <summary>The currency used by the client; Harvest's default is the company's currency.</summary>
	public string? Currency { get; set; }
}

/// <summary>The body of <c>PATCH /v2/clients/{id}</c>.</summary>
public class ClientPatchDto : PatchDto
{
	/// <summary>A textual description of the client.</summary>
	public string? Name { get; set; }

	/// <summary>Whether the client is active or archived.</summary>
	public bool? IsActive { get; set; }

	/// <summary>A textual representation of the client's physical address.</summary>
	public string? Address { get; set; }

	/// <summary>The currency used by the client.</summary>
	public string? Currency { get; set; }
}