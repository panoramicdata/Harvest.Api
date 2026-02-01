namespace Harvest.Interfaces;

public interface IClientApi
{
	[Get("/v2/clients")]
	Task<ClientsContainer> ListAllAsync(
		[AliasAs("updated_since")] DateTime? updatedSince = null,
		int? page = null,
		[AliasAs("per_page")] int? perPage = null,
		CancellationToken cancellationToken = default
	);

	[Get("/v2/clients/{id}")]
	Task<Client> GetAsync(long id,
		CancellationToken cancellationToken);

	[Post("/v2/clients")]
	Task<Client> CreateAsync(ClientCreationDto creationDto, CancellationToken cancellationToken = default);

	[Patch("/v2/clients/{id}")]
	Task<Client> UpdateAsync(long id, ClientPatchDto patchDto, CancellationToken cancellationToken = default);

	[Delete("/v2/clients/{id}")]
	System.Threading.Tasks.Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}