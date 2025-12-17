namespace Harvest.Interfaces;

public interface IUserApi
{
	[Get("/v2/users")]
	Task<UsersContainer> ListAllAsync(
		[AliasAs("updated_since")] DateTime? updatedSince = null,
		int? page = null,
		[AliasAs("per_page")] int? perPage = null,
		CancellationToken cancellationToken = default
	);

	[Get("/v2/users/{id}")]
	Task<User> GetAsync(long id, CancellationToken cancellationToken);

	[Get("/v2/users/me")]
	Task<User> GetMeAsync(CancellationToken cancellationToken);
}