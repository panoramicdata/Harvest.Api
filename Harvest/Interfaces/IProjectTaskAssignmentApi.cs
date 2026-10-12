namespace Harvest.Interfaces;

public interface IProjectTaskAssignmentApi
{
	[Get("/v2/projects/{id}/task_assignments")]
	Task<ProjectTaskAssignmentsContainer> ListAllAsync(long id, [Query] ListOptions? options, CancellationToken cancellationToken);

	[Get("/v2/projects/{projectId}/task_assignments/{id}")]
	Task<ProjectTaskAssignment> GetAsync(long projectId, long id, CancellationToken cancellationToken);

	[Post("/v2/projects/{projectId}/task_assignments")]
	Task<ProjectTaskAssignment> CreateAsync(long projectId, ProjectTaskAssignmentCreationDto creationDto, CancellationToken cancellationToken);

	[Patch("/v2/projects/{projectId}/task_assignments/{id}")]
	Task<ProjectTaskAssignment> UpdateAsync(long projectId, long id, ProjectTaskAssignmentPatchDto patchDto, CancellationToken cancellationToken);

	[Delete("/v2/projects/{projectId}/task_assignments/{id}")]
	System.Threading.Tasks.Task DeleteAsync(long projectId, long id, CancellationToken cancellationToken);
}