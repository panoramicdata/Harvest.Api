namespace Harvest.Interfaces;

/// <summary>
/// The active project assignments of a user (<see href="https://help.getharvest.com/api-v2/users-api/users/project-assignments/"/>).
/// Harvest documents only these two read endpoints; assignments are created and changed through a project's user
/// assignments.
/// </summary>
public interface IUserProjectAssignmentApi
{
	/// <summary>Lists one page of a user's active project assignments.</summary>
	/// <param name="userId">The user ID.</param>
	/// <param name="options">Paging and <c>updated_since</c>, or <see langword="null"/> for none.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of project assignments.</returns>
	[Get("/v2/users/{userId}/project_assignments")]
	Task<UserProjectAssignmentsContainer> ListAsync(long userId, [Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>Lists one page of the authenticated user's active project assignments.</summary>
	/// <param name="options">Paging, or <see langword="null"/> for none.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of project assignments.</returns>
	[Get("/v2/users/me/project_assignments")]
	Task<UserProjectAssignmentsContainer> ListMineAsync([Query] ListOptions? options, CancellationToken cancellationToken);
}