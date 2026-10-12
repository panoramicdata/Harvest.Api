namespace Harvest.Interfaces;

/// <summary>
/// Estimate item categories, the kinds of estimate line item (<see href="https://help.getharvest.com/api-v2/estimates-api/estimates/estimate-item-categories/"/>).
/// Admin permissions are required.
/// </summary>
public interface IEstimateItemCategoryApi
{
	/// <summary>Lists one page of estimate item categories, most recently created first.</summary>
	/// <param name="options">Filtering (<c>updated_since</c>) and paging, or <see langword="null"/> for none.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of estimate item categories.</returns>
	[Get("/v2/estimate_item_categories")]
	Task<EstimateItemCategoriesContainer> ListAsync([Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>Gets an estimate item category.</summary>
	/// <param name="id">The estimate item category ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The estimate item category.</returns>
	[Get("/v2/estimate_item_categories/{id}")]
	Task<EstimateItemCategory> GetAsync(long id, CancellationToken cancellationToken);

	/// <summary>Creates an estimate item category.</summary>
	/// <param name="creationDto">The new estimate item category.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The created estimate item category.</returns>
	[Post("/v2/estimate_item_categories")]
	Task<EstimateItemCategory> CreateAsync([Body] EstimateItemCategoryCreationDto creationDto, CancellationToken cancellationToken);

	/// <summary>Updates an estimate item category; fields left <see langword="null"/> are unchanged.</summary>
	/// <param name="id">The estimate item category ID.</param>
	/// <param name="patchDto">The fields to change.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The updated estimate item category.</returns>
	[Patch("/v2/estimate_item_categories/{id}")]
	Task<EstimateItemCategory> UpdateAsync(long id, [Body] EstimateItemCategoryPatchDto patchDto, CancellationToken cancellationToken);

	/// <summary>Deletes an estimate item category.</summary>
	/// <param name="id">The estimate item category ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A task that completes when the estimate item category is deleted.</returns>
	[Delete("/v2/estimate_item_categories/{id}")]
	System.Threading.Tasks.Task DeleteAsync(long id, CancellationToken cancellationToken);
}