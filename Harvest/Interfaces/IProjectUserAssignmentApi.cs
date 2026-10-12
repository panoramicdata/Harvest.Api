namespace Harvest.Interfaces;

/// <summary>
/// The user assignments of projects (<see href="https://help.getharvest.com/api-v2/projects-api/projects/user-assignments/"/>).
/// Requires an Administrator, or a Manager with assigned teammates or managed projects; anyone else gets HTTP 403.
/// </summary>
public interface IProjectUserAssignmentApi
{
	/// <summary>Lists one page of the user assignments of every project, active and archived, most recently created first.</summary>
	/// <param name="options">Filtering and paging, or <see langword="null"/> for none.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of user assignments.</returns>
	[Get("/v2/user_assignments")]
	Task<UserAssignmentsContainer> ListAsync([Query] UserAssignmentListOptions? options, CancellationToken cancellationToken);

	/// <summary>Lists one page of a project's user assignments, most recently created first.</summary>
	/// <param name="projectId">The project ID.</param>
	/// <param name="options">Filtering and paging, or <see langword="null"/> for none.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of user assignments.</returns>
	[Get("/v2/projects/{projectId}/user_assignments")]
	Task<UserAssignmentsContainer> ListAsync(long projectId, [Query] UserAssignmentListOptions? options, CancellationToken cancellationToken);

	/// <summary>Gets a user assignment.</summary>
	/// <param name="projectId">The project ID.</param>
	/// <param name="id">The user assignment ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The user assignment.</returns>
	[Get("/v2/projects/{projectId}/user_assignments/{id}")]
	Task<UserAssignment> GetAsync(long projectId, long id, CancellationToken cancellationToken);

	/// <summary>Assigns a user to a project.</summary>
	/// <param name="projectId">The project ID.</param>
	/// <param name="creationDto">The new user assignment.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The created user assignment.</returns>
	[Post("/v2/projects/{projectId}/user_assignments")]
	Task<UserAssignment> CreateAsync(long projectId, [Body] UserAssignmentCreationDto creationDto, CancellationToken cancellationToken);

	/// <summary>Updates a user assignment; fields left <see langword="null"/> are unchanged.</summary>
	/// <param name="projectId">The project ID.</param>
	/// <param name="id">The user assignment ID.</param>
	/// <param name="patchDto">The fields to change.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The updated user assignment.</returns>
	[Patch("/v2/projects/{projectId}/user_assignments/{id}")]
	Task<UserAssignment> UpdateAsync(long projectId, long id, [Body] UserAssignmentPatchDto patchDto, CancellationToken cancellationToken);

	/// <summary>Deletes a user assignment. Harvest refuses while it has time entries or expenses.</summary>
	/// <param name="projectId">The project ID.</param>
	/// <param name="id">The user assignment ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A task that completes when the user assignment is deleted.</returns>
	[Delete("/v2/projects/{projectId}/user_assignments/{id}")]
	System.Threading.Tasks.Task DeleteAsync(long projectId, long id, CancellationToken cancellationToken);
}