namespace Harvest.Interfaces;

/// <summary>
/// PTO user assignments: each user's holiday calendar and work schedule
/// (<see href="https://help.getharvest.com/api-v2/pto-api/pto/assignments-and-allocations/"/>). Only administrators can
/// use them.
/// </summary>
/// <remarks>
/// The PTO API is being rolled out gradually: an account without it receives <c>403 Forbidden</c> (a
/// <see cref="HarvestApiException"/>) from every call.
/// </remarks>
public interface IPtoAssignmentApi
{
	/// <summary>Lists users with their assigned holiday calendars, assigned work schedules and effective working hours.</summary>
	/// <param name="options">The user filters, or <see langword="null"/> for every user.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The assignments.</returns>
	[Get("/v2/pto/assignments")]
	Task<PtoAssignmentsContainer> ListAsync([Query] PtoAssignmentListOptions? options, CancellationToken cancellationToken);

	/// <summary>Assigns or unassigns a user's holiday calendar and work schedule; fields not set are unchanged.</summary>
	/// <param name="userId">The user ID.</param>
	/// <param name="patchDto">The assignments to change.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The user's assignment.</returns>
	[Patch("/v2/pto/assignments/{userId}")]
	Task<PtoAssignment> UpdateAsync(long userId, [Body] PtoAssignmentPatchDto patchDto, CancellationToken cancellationToken);
}