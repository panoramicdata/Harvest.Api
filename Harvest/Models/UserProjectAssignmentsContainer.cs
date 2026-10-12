namespace Harvest.Models;

/// <summary>A page of <c>GET /v2/users/{userId}/project_assignments</c> or <c>GET /v2/users/me/project_assignments</c>.</summary>
public class UserProjectAssignmentsContainer : ListContainerBase
{
	/// <summary>The project assignments on this page (<c>project_assignments</c>).</summary>
	public List<UserProjectAssignment> ProjectAssignments { get; set; } = [];
}