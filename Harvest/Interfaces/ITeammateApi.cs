namespace Harvest.Interfaces;

/// <summary>
/// The teammates assigned to a manager (<see href="https://help.getharvest.com/api-v2/users-api/users/teammates/"/>).
/// Only Administrators may use this endpoint (anyone else gets HTTP 403), and the user must be a Manager (otherwise
/// HTTP 422).
/// </summary>
public interface ITeammateApi
{
	/// <summary>Lists one page of the teammates assigned to a manager.</summary>
	/// <param name="userId">The ID of the manager.</param>
	/// <param name="options">Paging, or <see langword="null"/> for none. Harvest documents no filters for teammates.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of teammates.</returns>
	[Get("/v2/users/{userId}/teammates")]
	Task<TeammatesContainer> ListAsync(long userId, [Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>
	/// Replaces the teammates assigned to a manager. Adding teammates for the first time gives the manager the
	/// <c>people_manager</c> access role; an empty list unassigns everyone and removes that role.
	/// </summary>
	/// <param name="userId">The ID of the manager.</param>
	/// <param name="patchDto">The full list of teammates.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The teammates now assigned (Harvest sends no paging fields with this response).</returns>
	[Patch("/v2/users/{userId}/teammates")]
	Task<TeammatesContainer> UpdateAsync(long userId, [Body] TeammatePatchDto patchDto, CancellationToken cancellationToken);
}