namespace Harvest.Interfaces;

/// <summary>
/// The cost rates of a user (<see href="https://help.getharvest.com/api-v2/users-api/users/cost-rates/"/>). Only
/// Administrators may use this endpoint; anyone else gets HTTP 403.
/// </summary>
public interface ICostRateApi
{
	/// <summary>Lists one page of a user's cost rates, oldest start date first.</summary>
	/// <param name="userId">The user ID.</param>
	/// <param name="options">Paging, or <see langword="null"/> for none. Harvest documents no filters for cost rates.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of cost rates.</returns>
	[Get("/v2/users/{userId}/cost_rates")]
	Task<CostRatesContainer> ListAsync(long userId, [Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>Gets a cost rate.</summary>
	/// <param name="userId">The user ID.</param>
	/// <param name="id">The cost rate ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The cost rate.</returns>
	[Get("/v2/users/{userId}/cost_rates/{id}")]
	Task<CostRate> GetAsync(long userId, long id, CancellationToken cancellationToken);

	/// <summary>
	/// Creates a cost rate. With no start date it replaces the user's existing rates; with a start date before an existing
	/// rate's, it replaces those rates.
	/// </summary>
	/// <param name="userId">The user ID.</param>
	/// <param name="creationDto">The new cost rate.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The created cost rate.</returns>
	[Post("/v2/users/{userId}/cost_rates")]
	Task<CostRate> CreateAsync(long userId, [Body] CostRateCreationDto creationDto, CancellationToken cancellationToken);
}