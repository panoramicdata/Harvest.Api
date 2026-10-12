namespace Harvest.Interfaces;

/// <summary>
/// PTO time off requests (<see href="https://help.getharvest.com/api-v2/pto-api/pto/time-off-requests/"/>).
/// Administrators can manage every request; other users can view, create, update and cancel only their own.
/// </summary>
/// <remarks>
/// The PTO API is being rolled out gradually: an account without it receives <c>403 Forbidden</c> (a
/// <see cref="HarvestApiException"/>) from every call.
/// </remarks>
public interface IPtoRequestApi
{
	/// <summary>Lists one page of time off requests.</summary>
	/// <param name="options">Filtering and paging, or <see langword="null"/> for none.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of time off requests.</returns>
	[Get("/v2/pto/requests")]
	Task<PtoRequestsContainer> ListAsync([Query] PtoRequestListOptions? options, CancellationToken cancellationToken);

	/// <summary>Gets a time off request.</summary>
	/// <param name="id">The time off request ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The time off request.</returns>
	[Get("/v2/pto/requests/{id}")]
	Task<PtoRequest> GetAsync(long id, CancellationToken cancellationToken);

	/// <summary>Creates a time off request.</summary>
	/// <param name="creationDto">The new time off request.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The created time off request.</returns>
	[Post("/v2/pto/requests")]
	Task<PtoRequest> CreateAsync([Body] PtoRequestCreationDto creationDto, CancellationToken cancellationToken);

	/// <summary>
	/// Updates an unreviewed (<c>pending</c> or <c>changes_requested</c>) time off request; fields left
	/// <see langword="null"/> are unchanged. Harvest cancels the original request and returns a replacement with a new ID.
	/// </summary>
	/// <param name="id">The time off request ID.</param>
	/// <param name="patchDto">The fields to change.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The replacement time off request; its <see cref="PtoRequest.Id"/> may differ from <paramref name="id"/>.</returns>
	[Patch("/v2/pto/requests/{id}")]
	Task<PtoRequest> UpdateAsync(long id, [Body] PtoRequestPatchDto patchDto, CancellationToken cancellationToken);

	/// <summary>
	/// Cancels a time off request (<c>DELETE /v2/pto/requests/{id}</c>). The request is not deleted: its status becomes
	/// <c>cancelled</c> and it remains visible in listings.
	/// </summary>
	/// <param name="id">The time off request ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The cancelled time off request.</returns>
	[Delete("/v2/pto/requests/{id}")]
	Task<PtoRequest> CancelAsync(long id, CancellationToken cancellationToken);

	/// <summary>Approves a <c>pending</c> or <c>changes_requested</c> time off request. Only administrators can do this.</summary>
	/// <param name="id">The time off request ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The approved time off request.</returns>
	[Post("/v2/pto/requests/{id}/approve")]
	Task<PtoRequest> ApproveAsync(long id, CancellationToken cancellationToken);

	/// <summary>Declines a time off request. Only administrators can do this.</summary>
	/// <param name="id">The time off request ID.</param>
	/// <param name="declineDto">The optional decline reason.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The declined time off request.</returns>
	[Post("/v2/pto/requests/{id}/decline")]
	Task<PtoRequest> DeclineAsync(long id, [Body] PtoRequestDeclineDto declineDto, CancellationToken cancellationToken);

	/// <summary>Requests changes on a time off request from its submitter. Only administrators can do this.</summary>
	/// <param name="id">The time off request ID.</param>
	/// <param name="requestChangesDto">The explanation of the changes required.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The time off request, with status <c>changes_requested</c>.</returns>
	[Post("/v2/pto/requests/{id}/request_changes")]
	Task<PtoRequest> RequestChangesAsync(long id, [Body] PtoRequestChangesDto requestChangesDto, CancellationToken cancellationToken);
}