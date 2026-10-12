namespace Harvest.Interfaces;

public interface ITaskApi
{
	[Get("/v2/tasks")]
	Task<TasksContainer> ListAllAsync([Query] ListOptions? options, CancellationToken cancellationToken);

	[Get("/v2/tasks/{id}")]
	Task<Models.Task> GetAsync(long id, CancellationToken cancellationToken);

	[Post("/v2/tasks")]
	Task<Models.Task> CreateAsync(TaskCreationDto creationDto, CancellationToken cancellationToken);

	[Patch("/v2/tasks/{id}")]
	Task<Models.Task> UpdateAsync(long id, TaskPatchDto patchDto, CancellationToken cancellationToken);

	[Delete("/v2/tasks/{id}")]
	System.Threading.Tasks.Task DeleteAsync(long id, CancellationToken cancellationToken);
}