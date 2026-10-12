namespace Harvest.Models;

/// <summary>
/// A time off policy, which Harvest's paths and JSON call a PTO type
/// (<see href="https://help.getharvest.com/api-v2/pto-api/pto/time-off-policies/"/>).
/// </summary>
public class PtoType : PtoAccrualSettings
{
	/// <summary>Unique ID for the time off policy.</summary>
	public long Id { get; set; }

	/// <summary>The name of the time off policy (e.g. "Vacation", "Sick Leave").</summary>
	public string? Name { get; set; }

	/// <summary>Color assigned to the policy in the UI.</summary>
	public string? Color { get; set; }

	/// <summary>Icon slug assigned to the policy.</summary>
	public string? Icon { get; set; }

	/// <summary>Default days per year credited to teammates.</summary>
	public decimal? DefaultDaysPerYear { get; set; }

	/// <summary>Maximum days allowed to carry over between years.</summary>
	public decimal? MaxCarryoverDays { get; set; }

	/// <summary>Whether requests booked under this policy require manager or admin review.</summary>
	public bool RequiresApproval { get; set; }

	/// <summary>Whether this policy is visible to all teammates for self-booking.</summary>
	public bool VisibleToAll { get; set; }

	/// <summary>Whether the policy is active or deactivated.</summary>
	public bool IsActive { get; set; }

	/// <summary>Date and time the policy was created.</summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>Date and time the policy was last updated.</summary>
	public DateTime UpdatedAt { get; set; }
}