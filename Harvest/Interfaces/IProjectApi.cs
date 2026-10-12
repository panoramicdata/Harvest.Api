namespace Harvest.Interfaces;

/// <summary>Projects (<see href="https://help.getharvest.com/api-v2/projects-api/projects/projects/"/>).</summary>
public interface IProjectApi
{
	/// <summary>Lists one page of projects, most recently created first.</summary>
	/// <param name="options">Filtering and paging, or <see langword="null"/> for none.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of projects.</returns>
	[Get("/v2/projects")]
	Task<ProjectsContainer> ListAsync([Query] ProjectListOptions? options, CancellationToken cancellationToken);

	/// <summary>Gets a project.</summary>
	/// <param name="id">The project ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The project.</returns>
	[Get("/v2/projects/{id}")]
	Task<Project> GetAsync(long id, CancellationToken cancellationToken);

	/// <summary>Creates a project.</summary>
	/// <param name="creationDto">The new project.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The created project.</returns>
	[Post("/v2/projects")]
	Task<Project> CreateAsync([Body] ProjectCreationDto creationDto, CancellationToken cancellationToken);

	/// <summary>Updates a project; fields left <see langword="null"/> are unchanged.</summary>
	/// <param name="id">The project ID.</param>
	/// <param name="patchDto">The fields to change.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The updated project.</returns>
	[Patch("/v2/projects/{id}")]
	Task<Project> UpdateAsync(long id, [Body] ProjectPatchDto patchDto, CancellationToken cancellationToken);

	/// <summary>Deletes a project, with its time entries, expenses and assignments.</summary>
	/// <param name="id">The project ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A task that completes when the project is deleted.</returns>
	[Delete("/v2/projects/{id}")]
	System.Threading.Tasks.Task DeleteAsync(long id, CancellationToken cancellationToken);
}