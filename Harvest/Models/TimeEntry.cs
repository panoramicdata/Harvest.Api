using Harvest.Serialization;
using System.Text.Json.Serialization;

namespace Harvest.Models;

/// <summary>A time entry (<see href="https://help.getharvest.com/api-v2/timesheets-api/timesheets/time-entries/"/>).</summary>
public class TimeEntry
{
	/// <summary>Unique ID for the time entry.</summary>
	public long Id { get; set; }

	/// <summary>Date of the time entry.</summary>
	public DateOnly SpentDate { get; set; }

	/// <summary>The id and name of the associated user.</summary>
	public UserReference? User { get; set; }

	/// <summary>The user assignment of the associated user (without its project and user).</summary>
	public UserAssignment? UserAssignment { get; set; }

	/// <summary>The id and name of the associated client.</summary>
	public ClientReference? Client { get; set; }

	/// <summary>The id and name of the associated project.</summary>
	public ProjectReference? Project { get; set; }

	/// <summary>The id and name of the associated task.</summary>
	public TaskReference? Task { get; set; }

	/// <summary>The task assignment of the associated task (without its project and task).</summary>
	public ProjectTaskAssignment? TaskAssignment { get; set; }

	/// <summary>The associated external reference, if any.</summary>
	public ExternalReference? ExternalReference { get; set; }

	/// <summary>
	/// Once the time entry has been invoiced, the associated invoice's id and number. Harvest does not return it in list
	/// responses.
	/// </summary>
	public InvoiceReference? Invoice { get; set; }

	/// <summary>Number of (decimal time) hours tracked in this time entry.</summary>
	public decimal Hours { get; set; }

	/// <summary>Number of (decimal time) hours already tracked in this time entry, before the timer was last started.</summary>
	public decimal? HoursWithoutTimer { get; set; }

	/// <summary>Number of (decimal time) hours tracked in this time entry used in summary reports and invoices, rounded per the company's settings.</summary>
	public decimal? RoundedHours { get; set; }

	/// <summary>Notes attached to the time entry.</summary>
	public string? Notes { get; set; }

	/// <summary>Whether or not the time entry has been locked.</summary>
	public bool IsLocked { get; set; }

	/// <summary>Why the time entry has been locked.</summary>
	public string? LockedReason { get; set; }

	/// <summary>Whether or not the time entry has been approved via Timesheet Approval.</summary>
	public bool IsClosed { get; set; }

	/// <summary>The approval status of the time entry: one of the <see cref="Models.ApprovalStatus"/> values.</summary>
	public string? ApprovalStatus { get; set; }

	/// <summary>Whether or not the time entry has been marked as invoiced.</summary>
	public bool IsBilled { get; set; }

	/// <summary>Date and time the running timer was started; <see langword="null"/> when no timer is running.</summary>
	public DateTime? TimerStartedAt { get; set; }

	/// <summary>
	/// Time the time entry was started, if tracking by start and end times, as Harvest formats it for the company's clock
	/// (<c>8:00am</c> or <c>08:00</c>). <see cref="StartedTimeSpan"/> parses it.
	/// </summary>
	public string? StartedTime { get; set; }

	/// <summary>
	/// <see cref="StartedTime"/> as a time of day, or <see langword="null"/> when it is missing or not a recognised time.
	/// </summary>
	[JsonIgnore]
	public TimeSpan? StartedTimeSpan => HarvestTime.Parse(StartedTime);

	/// <summary>
	/// Time the time entry was ended, if tracking by start and end times, as Harvest formats it for the company's clock.
	/// <see cref="EndedTimeSpan"/> parses it.
	/// </summary>
	public string? EndedTime { get; set; }

	/// <summary>
	/// <see cref="EndedTime"/> as a time of day, or <see langword="null"/> when it is missing or not a recognised time.
	/// </summary>
	[JsonIgnore]
	public TimeSpan? EndedTimeSpan => HarvestTime.Parse(EndedTime);

	/// <summary>Whether or not the time entry is currently running.</summary>
	public bool IsRunning { get; set; }

	/// <summary>Whether or not the time entry is billable.</summary>
	public bool Billable { get; set; }

	/// <summary>Whether or not the time entry counts towards the project budget.</summary>
	public bool Budgeted { get; set; }

	/// <summary>The billable rate for the time entry.</summary>
	public decimal? BillableRate { get; set; }

	/// <summary>The cost rate for the time entry.</summary>
	public decimal? CostRate { get; set; }

	/// <summary>Date and time the time entry was created.</summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>Date and time the time entry was last updated.</summary>
	public DateTime UpdatedAt { get; set; }
}