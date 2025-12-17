namespace Harvest.Models;

public class TasksContainer : ListContainerBase
{
	public List<Task> Tasks { get; set; } = [];
}