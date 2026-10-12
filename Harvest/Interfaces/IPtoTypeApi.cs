namespace Harvest.Interfaces;

/// <summary>
/// PTO time off policies, which Harvest's paths and JSON call PTO types
/// (<see href="https://help.getharvest.com/api-v2/pto-api/pto/time-off-policies/"/>). Administrators can manage every
/// policy; other users can list and view only active policies that are visible to all.
/// </summary>
/// <remarks>
/// The PTO API is being rolled out gradually: an account without it receives <c>403 Forbidden</c> (a
/// <see cref="HarvestApiException"/>) from every call.
/// </remarks>
public interface IPtoTypeApi
{
	/// <summary>Lists one page of time off policies.</summary>
	/// <param name="options">
	/// Filtering and paging (<c>is_active</c>, Harvest's default <see langword="true"/>; <c>updated_since</c>; <c>page</c>;
	/// <c>per_page</c>), or <see langword="null"/> for none.
	/// </param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of time off policies.</returns>
	[Get("/v2/pto/types")]
	Task<PtoTypesContainer> ListAsync([Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>Gets a time off policy. A policy that is not visible to a non-administrator answers <c>404</c>.</summary>
	/// <param name="id">The time off policy ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The time off policy.</returns>
	[Get("/v2/pto/types/{id}")]
	Task<PtoType> GetAsync(long id, CancellationToken cancellationToken);

	/// <summary>Creates a time off policy. Only administrators can do this.</summary>
	/// <param name="creationDto">The new time off policy.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The created time off policy.</returns>
	[Post("/v2/pto/types")]
	Task<PtoType> CreateAsync([Body] PtoTypeCreationDto creationDto, CancellationToken cancellationToken);

	/// <summary>Updates a time off policy; fields left <see langword="null"/> are unchanged. Only administrators can do this.</summary>
	/// <param name="id">The time off policy ID.</param>
	/// <param name="patchDto">The fields to change.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The updated time off policy.</returns>
	[Patch("/v2/pto/types/{id}")]
	Task<PtoType> UpdateAsync(long id, [Body] PtoTypePatchDto patchDto, CancellationToken cancellationToken);

	/// <summary>
	/// Deactivates a time off policy (<c>DELETE /v2/pto/types/{id}</c> with Harvest's default <c>keep_history=true</c>),
	/// preserving its historical requests and allocations. Reactivate it with <see cref="PtoTypePatchDto.Deactivated"/>
	/// <see langword="false"/>. Only administrators can do this.
	/// </summary>
	/// <param name="id">The time off policy ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The deactivated policy, with <see cref="PtoType.IsActive"/> <see langword="false"/>.</returns>
	[Delete("/v2/pto/types/{id}")]
	Task<PtoType> DeactivateAsync(long id, CancellationToken cancellationToken);

	/// <summary>
	/// Permanently deletes a time off policy with all its requests and historical data
	/// (<c>DELETE /v2/pto/types/{id}?keep_history=false&amp;confirm_delete_history=1</c>). Harvest refuses when it is the
	/// company's only remaining policy or has requests included in locked payroll. Only administrators can do this.
	/// </summary>
	/// <param name="id">The time off policy ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A task that completes when the policy is deleted (Harvest answers <c>204 No Content</c>).</returns>
	[Delete("/v2/pto/types/{id}?keep_history=false&confirm_delete_history=1")]
	System.Threading.Tasks.Task DeleteAsync(long id, CancellationToken cancellationToken);
}
