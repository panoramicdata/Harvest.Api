namespace Harvest.Models;

/// <summary>
/// The values of a time entry's or an expense's <c>approval_status</c>. They are strings rather than an enum so that a
/// value Harvest adds later is still read.
/// </summary>
public static class ApprovalStatus
{
	/// <summary>Not yet submitted for approval.</summary>
	public static string Unsubmitted => "unsubmitted";

	/// <summary>Submitted and awaiting approval.</summary>
	public static string Submitted => "submitted";

	/// <summary>Approved.</summary>
	public static string Approved => "approved";
}