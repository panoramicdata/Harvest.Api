namespace Harvest.Models;

/// <summary>A time off request (<see href="https://help.getharvest.com/api-v2/pto-api/pto/time-off-requests/"/>).</summary>
public class PtoRequest
{
	/// <summary>Unique ID for the time off request.</summary>
	public long Id { get; set; }

	/// <summary>
	/// Status of request: <c>pending</c>, <c>approved</c>, <c>declined</c>, <c>changes_requested</c> or <c>cancelled</c>
	/// (see <see cref="PtoRequestStatus"/>).
	/// </summary>
	public string? Status { get; set; }

	/// <summary>Start date of the request.</summary>
	public DateOnly? StartDate { get; set; }

	/// <summary>End date of the request.</summary>
	public DateOnly? EndDate { get; set; }

	/// <summary>Total business days requested.</summary>
	public decimal? Days { get; set; }

	/// <summary>Working days snapshot at the time of submission.</summary>
	public decimal? BusinessDaysAtSubmission { get; set; }

	/// <summary>Starting minute offset for partial-day requests (e.g. 540 for 09:00).</summary>
	public int? StartTimeMinutes { get; set; }

	/// <summary>Ending minute offset for partial-day requests (e.g. 780 for 13:00).</summary>
	public int? EndTimeMinutes { get; set; }

	/// <summary>Submitter notes for the request.</summary>
	public string? Notes { get; set; }

	/// <summary>Reason provided during manager or admin review.</summary>
	public string? ReviewReason { get; set; }

	/// <summary>When the request was reviewed.</summary>
	public DateTime? ReviewedAt { get; set; }

	/// <summary>Date and time the request was created.</summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>Date and time the request was updated.</summary>
	public DateTime UpdatedAt { get; set; }

	/// <summary>The associated user: its id and name.</summary>
	public UserReference? User { get; set; }

	/// <summary>The associated time off policy: its id, name, color and icon.</summary>
	public PtoTypeReference? PtoType { get; set; }

	/// <summary>The reviewer: its id and name, or <see langword="null"/>.</summary>
	public UserReference? Reviewer { get; set; }

	/// <summary>The daily breakdown of the request.</summary>
	public List<PtoRequestDay>? RequestDays { get; set; }
}

/// <summary>One day of a time off request's daily breakdown.</summary>
public class PtoRequestDay
{
	/// <summary>Unique ID for the request day.</summary>
	public long Id { get; set; }

	/// <summary>The date of the day.</summary>
	public DateOnly? Date { get; set; }

	/// <summary>The hours requested on the day.</summary>
	public decimal? Hours { get; set; }

	/// <summary>The fraction of the working day requested (1.0 for a full day).</summary>
	public decimal? DayFraction { get; set; }

	/// <summary>Date and time the request day was created.</summary>
	public DateTime? CreatedAt { get; set; }

	/// <summary>Date and time the request day was updated.</summary>
	public DateTime? UpdatedAt { get; set; }
}