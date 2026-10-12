using System.Text.Json.Serialization;

namespace Harvest.Models;

/// <summary>
/// A user's yearly allocation for a time off policy: the overrides customized for the user (each <see langword="null"/>
/// when not customized) and the effective values
/// (<see href="https://help.getharvest.com/api-v2/pto-api/pto/assignments-and-allocations/"/>).
/// </summary>
public class PtoAllocation : PtoAccrualSettings
{
	/// <summary>Unique ID for the allocation record.</summary>
	public long Id { get; set; }

	/// <summary>Allocation year.</summary>
	public int? Year { get; set; }

	/// <summary>The user: its id and name.</summary>
	public UserReference? User { get; set; }

	/// <summary>The time off policy: its id, name, color and icon.</summary>
	public PtoTypeReference? PtoType { get; set; }

	/// <summary>Annual allowance override in days if customized.</summary>
	public decimal? DaysPerYear { get; set; }

	/// <summary>Carryover days override if customized.</summary>
	public decimal? CarryoverDays { get; set; }

	/// <summary>Approval requirement override if customized.</summary>
	public bool? RequiresApproval { get; set; }

	/// <summary>Effective days credited for the year.</summary>
	public decimal? EffectiveDaysPerYear { get; set; }

	/// <summary>Effective accrual mode applied to the user.</summary>
	public string? EffectiveAccrualType { get; set; }

	/// <summary>Effective accrual interval applied to the user.</summary>
	public string? EffectiveAccrualSchedule { get; set; }

	/// <summary>Effective approval requirement applied to the user.</summary>
	public bool? EffectiveRequiresApproval { get; set; }

	/// <summary>Date and time the allocation was created.</summary>
	public DateTime? CreatedAt { get; set; }

	/// <summary>Date and time the allocation was last updated.</summary>
	public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// The response of <c>PATCH /v2/pto/allocations</c>: either the saved allocation, or, when the allocation was reset to
/// the policy defaults, a reset confirmation carrying only <see cref="PtoAllocation.User"/>,
/// <see cref="PtoAllocation.PtoType"/>, <see cref="PtoAllocation.Year"/>, <see cref="Customized"/> and
/// <see cref="Message"/>.
/// </summary>
public class PtoAllocationUpdateResult : PtoAllocation
{
	/// <summary>In a reset confirmation, <see langword="false"/>: the allocation no longer has customized overrides.</summary>
	public bool? Customized { get; set; }

	/// <summary>In a reset confirmation, Harvest's message (for example "Allocation reset to type defaults").</summary>
	public string? Message { get; set; }

	/// <summary>Whether this is a reset confirmation rather than a saved allocation.</summary>
	[JsonIgnore]
	public bool IsReset => Customized == false;
}