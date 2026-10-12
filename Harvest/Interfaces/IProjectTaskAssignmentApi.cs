namespace Harvest.Interfaces;

/// <summary>The task assignments of a project (<see href="https://help.getharvest.com/api-v2/projects-api/projects/task-assignments/"/>).</summary>
public interface IProjectTaskAssignmentApi
{
	/// <summary>Lists one page of the task assignments of every project, most recently created first.</summary>
	/// <param name="options">
	/// Filtering (<c>is_active</c>, <c>updated_since</c>) and paging, or <see langword="null"/> for none.
	/// </param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of task assignments.</returns>
	[Get("/v2/task_assignments")]
	Task<ProjectTaskAssignmentsContainer> ListAsync([Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>Lists one page of a project's task assignments, most recently created first.</summary>
	/// <param name="projectId">The project ID.</param>
	/// <param name="options">
	/// Filtering (<c>is_active</c>, <c>updated_since</c>) and paging, or <see langword="null"/> for none.
	/// </param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of task assignments.</returns>
	[Get("/v2/projects/{projectId}/task_assignments")]
	Task<ProjectTaskAssignmentsContainer> ListAsync(long projectId, [Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>Gets a task assignment.</summary>
	/// <param name="projectId">The project ID.</param>
	/// <param name="id">The task assignment ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The task assignment.</returns>
	[Get("/v2/projects/{projectId}/task_assignments/{id}")]
	Task<ProjectTaskAssignment> GetAsync(long projectId, long id, CancellationToken cancellationToken);

	/// <summary>Assigns a task to a project.</summary>
	/// <param name="projectId">The project ID.</param>
	/// <param name="creationDto">The new task assignment.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The created task assignment.</returns>
	[Post("/v2/projects/{projectId}/task_assignments")]
	Task<ProjectTaskAssignment> CreateAsync(long projectId, [Body] ProjectTaskAssignmentCreationDto creationDto, CancellationToken cancellationToken);

	/// <summary>Updates a task assignment; fields left <see langword="null"/> are unchanged.</summary>
	/// <param name="projectId">The project ID.</param>
	/// <param name="id">The task assignment ID.</param>
	/// <param name="patchDto">The fields to change.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The updated task assignment.</returns>
	[Patch("/v2/projects/{projectId}/task_assignments/{id}")]
	Task<ProjectTaskAssignment> UpdateAsync(long projectId, long id, [Body] ProjectTaskAssignmentPatchDto patchDto, CancellationToken cancellationToken);

	/// <summary>Deletes a task assignment. Harvest refuses (HTTP 422) while it has time entries.</summary>
	/// <param name="projectId">The project ID.</param>
	/// <param name="id">The task assignment ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A task that completes when the task assignment is deleted.</returns>
	[Delete("/v2/projects/{projectId}/task_assignments/{id}")]
	System.Threading.Tasks.Task DeleteAsync(long projectId, long id, CancellationToken cancellationToken);
}