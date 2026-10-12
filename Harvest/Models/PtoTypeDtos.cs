namespace Harvest.Models;

/// <summary>The optional fields the bodies that create and update a time off policy share.</summary>
public abstract class PtoTypeDtoBase : PtoAccrualSettings
{
	/// <summary>The color code.</summary>
	public string? Color { get; set; }

	/// <summary>The icon name.</summary>
	public string? Icon { get; set; }

	/// <summary>Default days per year.</summary>
	public decimal? DefaultDaysPerYear { get; set; }

	/// <summary>Maximum carryover days allowed.</summary>
	public decimal? MaxCarryoverDays { get; set; }

	/// <summary>Whether requests require approval.</summary>
	public bool? RequiresApproval { get; set; }

	/// <summary>Whether the policy is visible to all users.</summary>
	public bool? VisibleToAll { get; set; }
}

/// <summary>The body of <c>POST /v2/pto/types</c>.</summary>
public class PtoTypeCreationDto : PtoTypeDtoBase
{
	/// <summary>The policy name.</summary>
	public required string Name { get; set; }
}

/// <summary>The body of <c>PATCH /v2/pto/types/{id}</c>.</summary>
public class PtoTypePatchDto : PtoTypeDtoBase
{
	/// <summary>The updated policy name.</summary>
	public string? Name { get; set; }

	/// <summary>Set to <see langword="false"/> to reactivate a deactivated policy.</summary>
	public bool? Deactivated { get; set; }
}