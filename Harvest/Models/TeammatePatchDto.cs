using Harvest.Dtos;

namespace Harvest.Models;

/// <summary>
/// The body of <c>PATCH /v2/users/{userId}/teammates</c>. Unlike other patches, the one field is required: it is the full
/// list of teammates, and any teammate not in it is unassigned.
/// </summary>
public class TeammatePatchDto : PatchDto
{
	/// <summary>Full list of user IDs to be assigned to the manager; an empty list unassigns everyone.</summary>
	public required List<long> TeammateIds { get; set; }
}