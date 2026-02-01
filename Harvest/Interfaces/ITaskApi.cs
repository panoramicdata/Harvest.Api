namespace Harvest.Interfaces;

public interface ITaskApi
{
	[Get("/v2/tasks")]
	Task<TasksContainer> ListAllAsync(
		[AliasAs("updated_since")] DateTime? updatedSince = null,
		int? page = null,
		[AliasAs("per_page")] int? perPage = null,
		CancellationToken cancellationToken = default
	);

	[Get("/v2/tasks/{id}")]
	Task<Models.Task> GetAsync(
		long id,
		CancellationToken cancellationToken);

	[Post("/v2/tasks")]
	Task<Models.Task> CreateAsync(TaskCreationDto creationDto, CancellationToken cancellationToken = default);

	[Patch("/v2/tasks/{id}")]
	Task<Models.Task> UpdateAsync(long id, TaskPatchDto patchDto, CancellationToken cancellationToken = default);

	[Delete("/v2/tasks/{id}")]
	System.Threading.Tasks.Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}