namespace Harvest.Interfaces;

public interface IUserProjectAssignmentApi
{
	[Get("/v2/users/{id}/project_assignments")]
	Task<UserProjectAssignmentsContainer> ListAllAsync(long id, [Query] ListOptions? options, CancellationToken cancellationToken);

	[Get("/v2/users/me/project_assignments")]
	Task<UserProjectAssignmentsContainer> ListAllMineAsync([Query] ListOptions? options, CancellationToken cancellationToken);

	[Get("/v2/users/{userId}/project_assignments/{id}")]
	Task<UserProjectAssignment> GetAsync(long userId, long id, CancellationToken cancellationToken);

	[Post("/v2/users/{userId}/project_assignments")]
	Task<UserProjectAssignment> CreateAsync(long userId, UserProjectAssignmentCreationDto creationDto, CancellationToken cancellationToken);

	[Patch("/v2/users/{userId}/project_assignments/{id}")]
	Task<UserProjectAssignment> UpdateAsync(long userId, long id, UserProjectAssignmentPatchDto patchDto, CancellationToken cancellationToken);

	[Delete("/v2/users/{userId}/project_assignments/{id}")]
	System.Threading.Tasks.Task DeleteAsync(long userId, long id, CancellationToken cancellationToken);
}