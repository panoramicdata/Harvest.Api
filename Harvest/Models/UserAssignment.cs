namespace Harvest.Models;

/// <summary>
/// A user assignment: a user assigned to a project
/// (<see href="https://help.getharvest.com/api-v2/projects-api/projects/user-assignments/"/>). Time entries nest a subset
/// of it, without <see cref="Project"/>, <see cref="User"/> or <see cref="UserAssignmentBase.UseDefaultRates"/>.
/// </summary>
public class UserAssignment : UserAssignmentBase
{
	/// <summary>The project's id, name and code; absent where the assignment is nested in a time entry.</summary>
	public ProjectReference? Project { get; set; }

	/// <summary>The user's id and name; absent where the assignment is nested in a time entry.</summary>
	public UserReference? User { get; set; }
}