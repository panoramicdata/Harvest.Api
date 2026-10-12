namespace Harvest.Interfaces;

/// <summary>
/// Estimates (<see href="https://help.getharvest.com/api-v2/estimates-api/estimates/estimates/"/>). Requires an
/// Administrator, or a Manager with permission to create and edit estimates.
/// </summary>
public interface IEstimateApi
{
	/// <summary>Lists one page of estimates, most recently issued first.</summary>
	/// <param name="options">Filtering and paging, or <see langword="null"/> for none.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of estimates.</returns>
	[Get("/v2/estimates")]
	Task<EstimatesContainer> ListAsync([Query] EstimateListOptions? options, CancellationToken cancellationToken);

	/// <summary>Gets an estimate.</summary>
	/// <param name="id">The estimate ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The estimate.</returns>
	[Get("/v2/estimates/{id}")]
	Task<Estimate> GetAsync(long id, CancellationToken cancellationToken);

	/// <summary>Creates an estimate, optionally with line items.</summary>
	/// <param name="creationDto">The new estimate.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The created estimate.</returns>
	[Post("/v2/estimates")]
	Task<Estimate> CreateAsync([Body] EstimateCreationDto creationDto, CancellationToken cancellationToken);

	/// <summary>
	/// Updates an estimate; fields left <see langword="null"/> are unchanged. Line items are added (no
	/// <see cref="EstimateLineItemPatchDto.Id"/>), updated (with an <see cref="EstimateLineItemPatchDto.Id"/>) or deleted
	/// (<see cref="EstimateLineItemPatchDto.Destroy"/>; see <see cref="EstimateLineItemPatchDto.Destroying"/>) through
	/// <see cref="EstimatePatchDto.LineItems"/>; line items not listed are left unchanged.
	/// </summary>
	/// <param name="id">The estimate ID.</param>
	/// <param name="patchDto">The fields and line item changes.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The updated estimate.</returns>
	[Patch("/v2/estimates/{id}")]
	Task<Estimate> UpdateAsync(long id, [Body] EstimatePatchDto patchDto, CancellationToken cancellationToken);

	/// <summary>Deletes an estimate.</summary>
	/// <param name="id">The estimate ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A task that completes when the estimate is deleted.</returns>
	[Delete("/v2/estimates/{id}")]
	System.Threading.Tasks.Task DeleteAsync(long id, CancellationToken cancellationToken);
}