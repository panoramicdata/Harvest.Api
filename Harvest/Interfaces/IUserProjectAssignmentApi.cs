namespace Harvest.Interfaces;

public interface IUserProjectAssignmentApi
{
	[Get("/v2/users/{id}/project_assignments")]
	System.Threading.Tasks.Task<UserProjectAssignmentsContainer> ListAllAsync(
		long id,
		[AliasAs("updated_since")] DateTime? updatedSince = null,
		int? page = null, // Defaults to 1
		[AliasAs("per_page")] int? perPage = null, // Defaults to 100
		CancellationToken cancellationToken = default
	);

	[Get("/v2/users/me/project_assignments")]
	System.Threading.Tasks.Task<UserProjectAssignmentsContainer> ListAllMineAsync(
		[AliasAs("updated_since")] DateTime? updatedSince = null,
		int? page = null, // Defaults to 1
		[AliasAs("per_page")] int? perPage = null, // Defaults to 100
		CancellationToken cancellationToken = default
	);

	[Get("/v2/users/{userId}/project_assignments/{id}")]
	Task<UserProjectAssignment> GetAsync(long userId, long id, CancellationToken cancellationToken = default);

	[Post("/v2/users/{userId}/project_assignments")]
	Task<UserProjectAssignment> CreateAsync(long userId, UserProjectAssignmentCreationDto creationDto, CancellationToken cancellationToken = default);

	[Patch("/v2/users/{userId}/project_assignments/{id}")]
	Task<UserProjectAssignment> UpdateAsync(long userId, long id, UserProjectAssignmentPatchDto patchDto, CancellationToken cancellationToken = default);

	[Delete("/v2/users/{userId}/project_assignments/{id}")]
	System.Threading.Tasks.Task DeleteAsync(long userId, long id, CancellationToken cancellationToken = default);
}