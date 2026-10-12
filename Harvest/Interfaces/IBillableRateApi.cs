namespace Harvest.Interfaces;

/// <summary>
/// The billable rates of a user (<see href="https://help.getharvest.com/api-v2/users-api/users/billable-rates/"/>). Requires
/// an Administrator, or a Manager with permission to edit billable rates; anyone else gets HTTP 403.
/// </summary>
public interface IBillableRateApi
{
	/// <summary>Lists one page of a user's billable rates, oldest start date first.</summary>
	/// <param name="userId">The user ID.</param>
	/// <param name="options">Paging, or <see langword="null"/> for none. Harvest documents no filters for billable rates.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of billable rates.</returns>
	[Get("/v2/users/{userId}/billable_rates")]
	Task<BillableRatesContainer> ListAsync(long userId, [Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>Gets a billable rate.</summary>
	/// <param name="userId">The user ID.</param>
	/// <param name="id">The billable rate ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The billable rate.</returns>
	[Get("/v2/users/{userId}/billable_rates/{id}")]
	Task<BillableRate> GetAsync(long userId, long id, CancellationToken cancellationToken);

	/// <summary>
	/// Creates a billable rate. With no start date it replaces the user's existing rates; with a start date before an
	/// existing rate's, it replaces those rates.
	/// </summary>
	/// <param name="userId">The user ID.</param>
	/// <param name="creationDto">The new billable rate.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The created billable rate.</returns>
	[Post("/v2/users/{userId}/billable_rates")]
	Task<BillableRate> CreateAsync(long userId, [Body] BillableRateCreationDto creationDto, CancellationToken cancellationToken);
}