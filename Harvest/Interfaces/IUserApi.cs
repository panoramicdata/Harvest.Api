namespace Harvest.Interfaces;

/// <summary>Users (<see href="https://help.getharvest.com/api-v2/users-api/users/users/"/>).</summary>
public interface IUserApi
{
	/// <summary>Lists one page of users, most recently created first.</summary>
	/// <param name="options">Filtering and paging, or <see langword="null"/> for none.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of users.</returns>
	[Get("/v2/users")]
	Task<UsersContainer> ListAsync([Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>Gets a user.</summary>
	/// <param name="id">The user ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The user.</returns>
	[Get("/v2/users/{id}")]
	Task<User> GetAsync(long id, CancellationToken cancellationToken);

	/// <summary>Gets the currently authenticated user.</summary>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The authenticated user.</returns>
	[Get("/v2/users/me")]
	Task<User> GetMeAsync(CancellationToken cancellationToken);

	/// <summary>Creates a user, who is sent an invitation email.</summary>
	/// <param name="creationDto">The new user.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The created user.</returns>
	[Post("/v2/users")]
	Task<User> CreateAsync([Body] UserCreationDto creationDto, CancellationToken cancellationToken);

	/// <summary>Updates a user; fields left <see langword="null"/> are unchanged.</summary>
	/// <param name="id">The user ID.</param>
	/// <param name="patchDto">The fields to change.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The updated user.</returns>
	[Patch("/v2/users/{id}")]
	Task<User> UpdateAsync(long id, [Body] UserPatchDto patchDto, CancellationToken cancellationToken);

	/// <summary>Deletes a user. Harvest refuses (HTTP 422) while the user has time entries or expenses; archive instead.</summary>
	/// <param name="id">The user ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A task that completes when the user is deleted.</returns>
	[Delete("/v2/users/{id}")]
	System.Threading.Tasks.Task DeleteAsync(long id, CancellationToken cancellationToken);
}