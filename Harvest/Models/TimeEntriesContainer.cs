namespace Harvest.Models;

public class TimeEntriesContainer : ListContainerBase
{
	public List<TimeEntry> TimeEntries { get; set; } = [];
}