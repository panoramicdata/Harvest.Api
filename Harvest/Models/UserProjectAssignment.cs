namespace Harvest.Models;

/// <summary>
/// One of a user's project assignments (<see href="https://help.getharvest.com/api-v2/users-api/users/project-assignments/"/>).
/// </summary>
public class UserProjectAssignment : UserAssignmentBase
{
	/// <summary>The assigned project's id, name and code.</summary>
	public ProjectReference? Project { get; set; }

	/// <summary>The project's client's id and name.</summary>
	public ClientReference? Client { get; set; }

	/// <summary>The task assignments of the project.</summary>
	public List<ProjectTaskAssignment>? TaskAssignments { get; set; }
}