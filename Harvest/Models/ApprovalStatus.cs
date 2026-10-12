namespace Harvest.Models;

/// <summary>
/// The values of a time entry's <c>approval_status</c>. They are strings rather than an enum so that a value Harvest adds
/// later is still read.
/// </summary>
public static class ApprovalStatus
{
	/// <summary>Not yet submitted for approval.</summary>
	public const string Unsubmitted = "unsubmitted";

	/// <summary>Submitted and awaiting approval.</summary>
	public const string Submitted = "submitted";

	/// <summary>Approved.</summary>
	public const string Approved = "approved";
}