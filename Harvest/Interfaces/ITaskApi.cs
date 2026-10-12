namespace Harvest.Interfaces;

/// <summary>Tasks (<see href="https://help.getharvest.com/api-v2/tasks-api/tasks/tasks/"/>).</summary>
public interface ITaskApi
{
	/// <summary>Lists one page of tasks, most recently created first.</summary>
	/// <param name="options">Filtering and paging, or <see langword="null"/> for none.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of tasks.</returns>
	[Get("/v2/tasks")]
	Task<TasksContainer> ListAsync([Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>Gets a task.</summary>
	/// <param name="id">The task ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The task.</returns>
	[Get("/v2/tasks/{id}")]
	Task<Models.Task> GetAsync(long id, CancellationToken cancellationToken);

	/// <summary>Creates a task.</summary>
	/// <param name="creationDto">The new task.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The created task.</returns>
	[Post("/v2/tasks")]
	Task<Models.Task> CreateAsync([Body] TaskCreationDto creationDto, CancellationToken cancellationToken);

	/// <summary>Updates a task; fields left <see langword="null"/> are unchanged.</summary>
	/// <param name="id">The task ID.</param>
	/// <param name="patchDto">The fields to change.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The updated task.</returns>
	[Patch("/v2/tasks/{id}")]
	Task<Models.Task> UpdateAsync(long id, [Body] TaskPatchDto patchDto, CancellationToken cancellationToken);

	/// <summary>Deletes a task. Harvest refuses (HTTP 422) while it has time entries.</summary>
	/// <param name="id">The task ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A task that completes when the task is deleted.</returns>
	[Delete("/v2/tasks/{id}")]
	System.Threading.Tasks.Task DeleteAsync(long id, CancellationToken cancellationToken);
}