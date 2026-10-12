namespace Harvest.Models;

/// <summary>
/// The values of a time off request's <c>status</c>. They are strings rather than an enum so that a value Harvest adds
/// later is still read.
/// </summary>
public static class PtoRequestStatus
{
	/// <summary>Awaiting review.</summary>
	public const string Pending = "pending";

	/// <summary>Approved.</summary>
	public const string Approved = "approved";

	/// <summary>Declined.</summary>
	public const string Declined = "declined";

	/// <summary>A reviewer asked the submitter for changes.</summary>
	public const string ChangesRequested = "changes_requested";

	/// <summary>Cancelled.</summary>
	public const string Cancelled = "cancelled";
}

/// <summary>The values of a time off policy's or allocation's <c>accrual_type</c>.</summary>
public static class PtoAccrualType
{
	/// <summary>A front-loaded allowance.</summary>
	public const string Fixed = "fixed";

	/// <summary>Earned over time.</summary>
	public const string Accrued = "accrued";

	/// <summary>Unlimited allowance.</summary>
	public const string Unlimited = "unlimited";
}

/// <summary>The values of a time off policy's or allocation's <c>accrual_schedule</c>.</summary>
public static class PtoAccrualSchedule
{
	/// <summary>Accrues monthly.</summary>
	public const string Monthly = "monthly";

	/// <summary>Accrues every two weeks, anchored on <c>accrual_first_payday</c>.</summary>
	public const string Biweekly = "biweekly";

	/// <summary>Accrues twice a month.</summary>
	public const string Semimonthly = "semimonthly";

	/// <summary>Accrues per hour worked (<c>accrual_leave_hours</c> per <c>accrual_worked_hours</c>).</summary>
	public const string Hourly = "hourly";
}

/// <summary>The values of a time off request's <c>conflict_resolution</c>, which resolves overlapping requests.</summary>
public static class PtoConflictResolution
{
	/// <summary>Skip the overlapping days.</summary>
	public const string Skip = "skip";

	/// <summary>Replace the overlapping requests.</summary>
	public const string Replace = "replace";
}
