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

		[Post("/v2/projects")]
		Task<Project> CreateAsync(ProjectCreationDto creationDto, CancellationToken cancellationToken = default);

		[Patch("/v2/projects/{id}")]
		Task<Project> UpdateAsync(long id, ProjectPatchDto patchDto, CancellationToken cancellationToken = default);

		[Delete("/v2/projects/{id}")]
		System.Threading.Tasks.Task DeleteAsync(long id, CancellationToken cancellationToken = default);
	}