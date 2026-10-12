namespace Harvest.Models;

/// <summary>A page of <c>GET /v2/time_entries</c>.</summary>
public class TimeEntriesContainer : ListContainerBase
{
	/// <summary>The time entries on this page (<c>time_entries</c>).</summary>
	public List<TimeEntry> TimeEntries { get; set; } = [];
}