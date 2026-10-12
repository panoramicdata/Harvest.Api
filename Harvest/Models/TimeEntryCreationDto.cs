using Harvest.Dtos;

namespace Harvest.Models;

/// <summary>
/// The body of <c>POST /v2/time_entries</c>. Give <see cref="Hours"/> on an account that tracks time by duration, or
/// <see cref="StartedTime"/> and <see cref="EndedTime"/> on one that tracks start and end times.
/// </summary>
public class TimeEntryCreationDto : CreationDto
{
	/// <summary>The ID of the user to associate with the time entry. Defaults to the currently authenticated user's ID.</summary>
	public long? UserId { get; set; }

	/// <summary>The ID of the project to associate with the time entry.</summary>
	public required long ProjectId { get; set; }

	/// <summary>The ID of the task to associate with the time entry.</summary>
	public required long TaskId { get; set; }

	/// <summary>The date of the time entry.</summary>
	public required DateOnly SpentDate { get; set; }

	/// <summary>The time the entry started, for example <c>8:00am</c>. Defaults to the current time.</summary>
	public string? StartedTime { get; set; }

	/// <summary>The time the entry ended, for example <c>9:30am</c>. If omitted, a timer is started.</summary>
	public string? EndedTime { get; set; }

	/// <summary>The current amount of time tracked. If omitted, a timer is started.</summary>
	public decimal? Hours { get; set; }

	/// <summary>Any notes to be associated with the time entry.</summary>
	public string? Notes { get; set; }

	/// <summary>An external reference to associate with the time entry.</summary>
	public ExternalReferenceCreationDto? ExternalReference { get; set; }
}