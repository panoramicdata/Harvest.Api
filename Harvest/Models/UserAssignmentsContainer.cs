namespace Harvest.Models;

/// <summary>A page of <c>GET /v2/user_assignments</c> or <c>GET /v2/projects/{projectId}/user_assignments</c>.</summary>
public class UserAssignmentsContainer : ListContainerBase
{
	/// <summary>The user assignments on this page (<c>user_assignments</c>).</summary>
	public List<UserAssignment> UserAssignments { get; set; } = [];
}