namespace Harvest.Models;

/// <summary>A page of <c>GET /v2/tasks</c>.</summary>
public class TasksContainer : ListContainerBase
{
	/// <summary>The tasks on this page (<c>tasks</c>).</summary>
	public List<Task> Tasks { get; set; } = [];
}