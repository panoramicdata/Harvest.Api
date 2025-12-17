namespace Harvest.Interfaces;

public interface IUserProjectAssignmentApi
{
	[Get("/v2/users/{id}/project_assignments")]
	Task<UserProjectAssignmentsContainer> ListAllAsync(
		long id,
		[AliasAs("updated_since")] DateTime? updatedSince = null,
		int? page = null, // Defaults to 1
		[AliasAs("per_page")] int? perPage = null, // Defaults to 100
		CancellationToken cancellationToken = default
	);

	[Get("/v2/users/me/project_assignments")]
	Task<UserProjectAssignmentsContainer> ListAllMineAsync(
		[AliasAs("updated_since")] DateTime? updatedSince = null,
		int? page = null, // Defaults to 1
		[AliasAs("per_page")] int? perPage = null, // Defaults to 100
		CancellationToken cancellationToken = default
	);
}