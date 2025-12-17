namespace Harvest.Interfaces;

	public interface IProjectApi
	{
		[Get("/v2/projects")]
		Task<ProjectsContainer> ListAllAsync(
			[AliasAs("updated_since")] DateTime? updatedSince = null,
			int? page = null,
			[AliasAs("per_page")] int? perPage = null
		);

		[Get("/v2/projects/{id}")]
		Task<Project> GetAsync(long id);
	}