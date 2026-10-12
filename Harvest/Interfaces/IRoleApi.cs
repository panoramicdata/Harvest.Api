namespace Harvest.Interfaces;

/// <summary>
/// Roles: the business roles that people are grouped by (<see href="https://help.getharvest.com/api-v2/roles-api/roles/roles/"/>).
/// Only Administrators may use this endpoint; anyone else gets HTTP 403.
/// </summary>
public interface IRoleApi
{
	/// <summary>Lists one page of roles, most recently created first.</summary>
	/// <param name="options">Paging, or <see langword="null"/> for none. Harvest documents no filters for roles.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of roles.</returns>
	[Get("/v2/roles")]
	Task<RolesContainer> ListAsync([Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>Gets a role.</summary>
	/// <param name="id">The role ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The role.</returns>
	[Get("/v2/roles/{id}")]
	Task<Role> GetAsync(long id, CancellationToken cancellationToken);

	/// <summary>Creates a role.</summary>
	/// <param name="creationDto">The new role.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The created role.</returns>
	[Post("/v2/roles")]
	Task<Role> CreateAsync([Body] RoleCreationDto creationDto, CancellationToken cancellationToken);

	/// <summary>
	/// Updates a role; fields left <see langword="null"/> are unchanged. A <see cref="RolePatchDto.UserIds"/> list replaces
	/// the role's users entirely.
	/// </summary>
	/// <param name="id">The role ID.</param>
	/// <param name="patchDto">The fields to change.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The updated role.</returns>
	[Patch("/v2/roles/{id}")]
	Task<Role> UpdateAsync(long id, [Body] RolePatchDto patchDto, CancellationToken cancellationToken);

	/// <summary>Deletes a role, unlinking it from any users it was assigned to.</summary>
	/// <param name="id">The role ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A task that completes when the role is deleted.</returns>
	[Delete("/v2/roles/{id}")]
	System.Threading.Tasks.Task DeleteAsync(long id, CancellationToken cancellationToken);
}