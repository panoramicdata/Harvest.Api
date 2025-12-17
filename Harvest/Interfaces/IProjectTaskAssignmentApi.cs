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
	}