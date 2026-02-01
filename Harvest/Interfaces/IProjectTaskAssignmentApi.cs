namespace Harvest.Interfaces;

	public interface IProjectTaskAssignmentApi
	{
		[Get("/v2/projects/{id}/task_assignments")]
		Task<ProjectTaskAssignmentsContainer> ListAllAsync(
			long id,
			[AliasAs("updated_since")] DateTime? updatedSince = null,
			int? page = null,
			[AliasAs("per_page")] int? perPage = null
		);

		[Get("/v2/projects/{projectId}/task_assignments/{id}")]
		Task<ProjectTaskAssignment> GetAsync(long projectId, long id, CancellationToken cancellationToken = default);

		[Post("/v2/projects/{projectId}/task_assignments")]
		Task<ProjectTaskAssignment> CreateAsync(long projectId, ProjectTaskAssignmentCreationDto creationDto, CancellationToken cancellationToken = default);

		[Patch("/v2/projects/{projectId}/task_assignments/{id}")]
		Task<ProjectTaskAssignment> UpdateAsync(long projectId, long id, ProjectTaskAssignmentPatchDto patchDto, CancellationToken cancellationToken = default);

		[Delete("/v2/projects/{projectId}/task_assignments/{id}")]
		System.Threading.Tasks.Task DeleteAsync(long projectId, long id, CancellationToken cancellationToken = default);
	}