using Harvest.Dtos;

namespace Harvest.Models;

/// <summary>The body of <c>PATCH /v2/time_entries/{id}</c>.</summary>
public class TimeEntryPatchDto : PatchDto
{
	/// <summary>The ID of the project to associate with the time entry.</summary>
	public long? ProjectId { get; set; }

	/// <summary>The ID of the task to associate with the time entry.</summary>
	public long? TaskId { get; set; }

	/// <summary>The date of the time entry.</summary>
	public DateOnly? SpentDate { get; set; }

	/// <summary>The time the entry started, for example <c>8:00am</c>.</summary>
	public string? StartedTime { get; set; }

	/// <summary>The time the entry ended, for example <c>9:30am</c>.</summary>
	public string? EndedTime { get; set; }

	/// <summary>The current amount of time tracked.</summary>
	public decimal? Hours { get; set; }

	/// <summary>Any notes to be associated with the time entry.</summary>
	public string? Notes { get; set; }

	/// <summary>An external reference to associate with the time entry.</summary>
	public ExternalReferenceCreationDto? ExternalReference { get; set; }
}