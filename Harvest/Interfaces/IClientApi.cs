namespace Harvest.Interfaces;

/// <summary>Clients (<see href="https://help.getharvest.com/api-v2/clients-api/clients/clients/"/>).</summary>
public interface IClientApi
{
	/// <summary>Lists one page of clients, most recently created first.</summary>
	/// <param name="options">Filtering and paging (<c>is_active</c>, <c>updated_since</c>, <c>cursor</c>, <c>page</c>, <c>per_page</c>), or <see langword="null"/> for none.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of clients.</returns>
	[Get("/v2/clients")]
	Task<ClientsContainer> ListAsync([Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>Gets a client.</summary>
	/// <param name="id">The client ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The client.</returns>
	[Get("/v2/clients/{id}")]
	Task<Client> GetAsync(long id, CancellationToken cancellationToken);

	/// <summary>Creates a client.</summary>
	/// <param name="creationDto">The new client.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The created client.</returns>
	[Post("/v2/clients")]
	Task<Client> CreateAsync([Body] ClientCreationDto creationDto, CancellationToken cancellationToken);

	/// <summary>Updates a client; fields left <see langword="null"/> are unchanged.</summary>
	/// <param name="id">The client ID.</param>
	/// <param name="patchDto">The fields to change.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The updated client.</returns>
	[Patch("/v2/clients/{id}")]
	Task<Client> UpdateAsync(long id, [Body] ClientPatchDto patchDto, CancellationToken cancellationToken);

	/// <summary>Deletes a client. Harvest refuses (HTTP 422) while the client has projects or invoices.</summary>
	/// <param name="id">The client ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A task that completes when the client is deleted.</returns>
	[Delete("/v2/clients/{id}")]
	System.Threading.Tasks.Task DeleteAsync(long id, CancellationToken cancellationToken);
}