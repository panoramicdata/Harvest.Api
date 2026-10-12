using Harvest.Dtos;

namespace Harvest.Models;

/// <summary>The body of <c>POST /v2/pto/requests</c>.</summary>
public class PtoRequestCreationDto : CreationDto
{
	/// <summary>The time off policy ID to request time against.</summary>
	public required long PtoTypeId { get; set; }

	/// <summary>Start date for the request.</summary>
	public required DateOnly StartDate { get; set; }

	/// <summary>End date for the request.</summary>
	public required DateOnly EndDate { get; set; }

	/// <summary>Target user ID. Only administrators can submit on behalf of another user; Harvest's default is the authenticated user.</summary>
	public long? UserId { get; set; }

	/// <summary>Optional notes describing the request.</summary>
	public string? Notes { get; set; }

	/// <summary>Starting minute offset for partial-day requests (e.g. 540 for 09:00).</summary>
	public int? StartTimeMinutes { get; set; }

	/// <summary>Ending minute offset for partial-day requests (e.g. 780 for 13:00).</summary>
	public int? EndTimeMinutes { get; set; }

	/// <summary>Either <c>skip</c> or <c>replace</c> to resolve overlapping requests (see <see cref="PtoConflictResolution"/>).</summary>
	public string? ConflictResolution { get; set; }
}

/// <summary>The body of <c>PATCH /v2/pto/requests/{id}</c>.</summary>
public class PtoRequestPatchDto : PatchDto
{
	/// <summary>Updated time off policy ID.</summary>
	public long? PtoTypeId { get; set; }

	/// <summary>Updated start date.</summary>
	public DateOnly? StartDate { get; set; }

	/// <summary>Updated end date.</summary>
	public DateOnly? EndDate { get; set; }

	/// <summary>Updated notes.</summary>
	public string? Notes { get; set; }

	/// <summary>Updated start minute offset.</summary>
	public int? StartTimeMinutes { get; set; }

	/// <summary>Updated end minute offset.</summary>
	public int? EndTimeMinutes { get; set; }

	/// <summary>Overlap resolution: <c>skip</c> or <c>replace</c> (see <see cref="PtoConflictResolution"/>).</summary>
	public string? ConflictResolution { get; set; }
}

/// <summary>The body of <c>POST /v2/pto/requests/{id}/decline</c>.</summary>
public class PtoRequestDeclineDto
{
	/// <summary>Optional decline reason.</summary>
	public string? Reason { get; set; }
}

/// <summary>The body of <c>POST /v2/pto/requests/{id}/request_changes</c>.</summary>
public class PtoRequestChangesDto
{
	/// <summary>Explanation of changes required from the submitter.</summary>
	public required string Reason { get; set; }
}