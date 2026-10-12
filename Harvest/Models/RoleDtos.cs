using Harvest.Dtos;

namespace Harvest.Models;

/// <summary>The body of <c>POST /v2/roles</c>.</summary>
public class RoleCreationDto : CreationDto
{
	/// <summary>The name of the role.</summary>
	public required string Name { get; set; }

	/// <summary>The IDs of the users assigned to this role; Harvest's default is none.</summary>
	public List<long>? UserIds { get; set; }
}

/// <summary>The body of <c>PATCH /v2/roles/{id}</c>.</summary>
public class RolePatchDto : PatchDto
{
	/// <summary>The name of the role.</summary>
	public string? Name { get; set; }

	/// <summary>
	/// The IDs of the users assigned to this role. This replaces the existing users: anyone not listed is unassigned. An
	/// empty list unassigns everyone.
	/// </summary>
	public List<long>? UserIds { get; set; }
}