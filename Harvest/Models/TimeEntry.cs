namespace Harvest.Models;

/// <summary>
/// Time entry given by the Harvest API.
/// </summary>
public class TimeEntry
{
	/// <summary>
	/// Unique ID for the time entry.
	/// </summary>
	public long Id { get; set; }

	/// <summary>
	/// Date of the time entry.
	/// </summary>
	public DateTime SpentDate { get; set; }

	/// <summary>
	/// An object containing the id and name of the associated user.
	/// </summary>
	public User? User { get; set; }

	/// <summary>
	/// A user assignment object of the associated user.
	/// </summary>
	public UserAssignment? UserAssignment { get; set; }

	/// <summary>
	/// An object containing the id and name of the associated client.
	/// </summary>
	public Client? Client { get; set; }

	/// <summary>
	/// An object containing the id and name of the associated project.
	/// </summary>
	public Project? Project { get; set; }

	/// <summary>
	/// An object containing the id and name of the associated task.
	/// </summary>
	public Task? Task { get; set; }

	/// <summary>
	/// 	A task assignment object of the associated task.
	/// </summary>
	public ProjectTaskAssignment? TaskAssignment { get; set; }

	/// <summary>
	/// 	An object containing the id, group_id, permalink, service, and service_icon_url of the associated external reference.
	/// </summary>
	public ExternalReference? ExternalsReference { get; set; }

	/// <summary>
	/// Once the time entry has been invoiced, this field will include the associated invoice's id and number. Note that this field is not returned in the list response.
	/// </summary>
	public Invoice? Invoice { get; set; }

	/// <summary>
	/// Number of (decimal time) hours tracked in this time entry.
	/// </summary>
	public decimal Hours { get; set; }

	/// <summary>
	/// Time entry notes.
	/// </summary>
	public string? Notes { get; set; }

	/// <summary>
	/// Whether or not the time entry has been locked.
	/// </summary>
	public bool IsLocked { get; set; }

	/// <summary>
	/// Why the time entry has been locked.
	/// </summary>
	public string? LockedReason { get; set; }

	/// <summary>
	/// Whether or not the time entry has been approved via Timesheet Approval.
	/// </summary>
	public bool IsClosed { get; set; }

	/// <summary>
	/// Whether or not the time entry has been marked as invoiced.
	/// </summary>
	public bool IsBilled { get; set; }

	/// <summary>
	/// Date and time the timer was started (if tracking by duration).
	/// </summary>
	public DateTime? SpentAt { get; set; }

	/// <summary>
	/// Time the time entry was started (if tracking by start/end times).
	/// </summary>
	public string? StartedTime { get; set; }

	/// <summary>
	/// StartedTime comes back as a string in the form 12:34pm
	/// </summary>
	public TimeSpan? StartedTimeSpan => string.IsNullOrEmpty(StartedTime) ? null : TimeSpan.ParseExact(StartedTime, "hh:mmtt", CultureInfo.InvariantCulture);

	/// <summary>
	/// Time the time entry was ended (if tracking by start/end times).
	/// </summary>
	public string? EndedTime { get; set; }

	/// <summary>
	/// EndedTime comes back as a string in the form 12:34pm
	/// </summary>
	public TimeSpan? EndedTimeSpan => string.IsNullOrEmpty(EndedTime) ? null : TimeSpan.ParseExact(EndedTime, "hh:mmtt", CultureInfo.InvariantCulture);

	/// <summary>
	/// Whether or not the time entry is currently running.
	/// </summary>
	public bool IsRunning { get; set; }

	/// <summary>
	/// Whether or not the time entry is billable.
	/// </summary>
	public bool Billable { get; set; }

	/// <summary>
	/// Whether or not the time entry counts towards the project budget.
	/// </summary>
	public bool Budgeted { get; set; }

	/// <summary>
	/// The billable rate for the time entry.
	/// </summary>
	public decimal? BillableRate { get; set; }

	/// <summary>
	/// The cost rate for the time entry.
	/// </summary>
	public decimal? CostRate { get; set; }

	/// <summary>
	/// Date and time the time entry was created.
	/// </summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>
	/// Date and time the time entry was last updated.
	/// </summary>
	public DateTime UpdatedAt { get; set; }
}
