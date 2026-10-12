namespace Harvest.Interfaces;

public interface IProjectApi
{
	[Get("/v2/projects")]
	Task<ProjectsContainer> ListAllAsync([Query] ListOptions? options, CancellationToken cancellationToken);

	[Get("/v2/projects/{id}")]
	Task<Project> GetAsync(long id, CancellationToken cancellationToken);

	[Post("/v2/projects")]
	Task<Project> CreateAsync(ProjectCreationDto creationDto, CancellationToken cancellationToken);

	[Patch("/v2/projects/{id}")]
	Task<Project> UpdateAsync(long id, ProjectPatchDto patchDto, CancellationToken cancellationToken);

	[Delete("/v2/projects/{id}")]
	System.Threading.Tasks.Task DeleteAsync(long id, CancellationToken cancellationToken);
}