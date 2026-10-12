namespace Harvest.Interfaces;

public interface IClientApi
{
	[Get("/v2/clients")]
	Task<ClientsContainer> ListAllAsync([Query] ListOptions? options, CancellationToken cancellationToken);

	[Get("/v2/clients/{id}")]
	Task<Client> GetAsync(long id, CancellationToken cancellationToken);

	[Post("/v2/clients")]
	Task<Client> CreateAsync(ClientCreationDto creationDto, CancellationToken cancellationToken);

	[Patch("/v2/clients/{id}")]
	Task<Client> UpdateAsync(long id, ClientPatchDto patchDto, CancellationToken cancellationToken);

	[Delete("/v2/clients/{id}")]
	System.Threading.Tasks.Task DeleteAsync(long id, CancellationToken cancellationToken);
}