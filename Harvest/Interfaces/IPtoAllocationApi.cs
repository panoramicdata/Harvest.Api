namespace Harvest.Interfaces;

/// <summary>
/// PTO user allocations: per-user, per-year overrides of time off policies
/// (<see href="https://help.getharvest.com/api-v2/pto-api/pto/assignments-and-allocations/"/>). Only administrators can
/// use them.
/// </summary>
/// <remarks>
/// The PTO API is being rolled out gradually: an account without it receives <c>403 Forbidden</c> (a
/// <see cref="HarvestApiException"/>) from every call.
/// </remarks>
public interface IPtoAllocationApi
{
	/// <summary>Lists one page of customized and yearly time off allocations across users.</summary>
	/// <param name="options">Filtering and paging, or <see langword="null"/> for none.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of allocations.</returns>
	[Get("/v2/pto/allocations")]
	Task<PtoAllocationsContainer> ListAsync([Query] PtoAllocationListOptions? options, CancellationToken cancellationToken);

	/// <summary>
	/// Updates or resets a user's yearly allocation overrides for a time off policy. When the allocation is reset to the
	/// policy defaults (by <see cref="PtoAllocationPatchDto.ResetToDefault"/>, or because the values submitted match the
	/// defaults) Harvest removes the override and answers with a reset confirmation instead of an allocation.
	/// </summary>
	/// <param name="patchDto">The user, policy and overrides.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>
	/// The allocation, or a reset confirmation (<see cref="PtoAllocationUpdateResult.IsReset"/> <see langword="true"/>).
	/// </returns>
	[Patch("/v2/pto/allocations")]
	Task<PtoAllocationUpdateResult> UpdateAsync([Body] PtoAllocationPatchDto patchDto, CancellationToken cancellationToken);
}