namespace Harvest.Models;

/// <summary>A page of <c>GET /v2/projects/{projectId}/task_assignments</c>.</summary>
public class ProjectTaskAssignmentsContainer : ListContainerBase
{
	/// <summary>The task assignments on this page (<c>task_assignments</c>).</summary>
	public List<ProjectTaskAssignment> TaskAssignments { get; set; } = [];
}