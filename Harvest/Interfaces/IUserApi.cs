namespace Harvest.Interfaces;

public interface IUserApi
{
	[Get("/v2/users")]
	Task<UsersContainer> ListAllAsync([Query] ListOptions? options, CancellationToken cancellationToken);

	[Get("/v2/users/{id}")]
	Task<User> GetAsync(long id, CancellationToken cancellationToken);

	[Get("/v2/users/me")]
	Task<User> GetMeAsync(CancellationToken cancellationToken);

	[Post("/v2/users")]
	Task<User> CreateAsync(UserCreationDto creationDto, CancellationToken cancellationToken);

	[Patch("/v2/users/{id}")]
	Task<User> UpdateAsync(long id, UserPatchDto patchDto, CancellationToken cancellationToken);

	[Delete("/v2/users/{id}")]
	System.Threading.Tasks.Task DeleteAsync(long id, CancellationToken cancellationToken);
}