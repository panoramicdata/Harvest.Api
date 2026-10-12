namespace Harvest.Models;

/// <summary>The optional fields the bodies that create and update a time entry share.</summary>
public abstract class TimeEntryDtoBase
{
	/// <summary>The time the entry started, for example <c>8:00am</c>. On creation, defaults to the current time.</summary>
	public string? StartedTime { get; set; }

	/// <summary>The time the entry ended, for example <c>9:30am</c>. On creation, a timer is started if it is omitted.</summary>
	public string? EndedTime { get; set; }

	/// <summary>The current amount of time tracked. On creation, a timer is started if it is omitted.</summary>
	public decimal? Hours { get; set; }

	/// <summary>Any notes to be associated with the time entry.</summary>
	public string? Notes { get; set; }

	/// <summary>An external reference to associate with the time entry.</summary>
	public ExternalReferenceCreationDto? ExternalReference { get; set; }
}