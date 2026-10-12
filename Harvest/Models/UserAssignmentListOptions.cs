namespace Harvest.Models;

/// <summary>Filtering and paging for <c>GET /v2/user_assignments</c> and <c>GET /v2/projects/{projectId}/user_assignments</c>.</summary>
public class UserAssignmentListOptions : ListOptions
{
	/// <summary>Only return user assignments belonging to the user with the given ID (<c>user_id</c>).</summary>
	[AliasAs("user_id")]
	public long? UserId { get; init; }
}