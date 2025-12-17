namespace Harvest.Interfaces;

public interface ITimeEntryApi
{
	[Get("/v2/time_entries")]
	Task<TimeEntriesContainer> ListAllAsync(
		[AliasAs("user_id")] long? userId = null,
		[AliasAs("client_id")] long? clientId = null,
		[AliasAs("project_id")] long? projectId = null,
		[AliasAs("is_billed")] bool? isBilled = null,
		[AliasAs("is_running")] bool? isRunning = null,
		[AliasAs("updated_since")] DateTime? updatedSince = null,
		string? from = null,
		[AliasAs("to")] string? toDate = null,
		int? page = null,
		[AliasAs("per_page")] int? perPage = null,
		CancellationToken cancellationToken = default
	);

	[Get("/v2/time_entries/{id}")]
	Task<TimeEntry> GetAsync(long id, CancellationToken cancellationToken);

	[Delete("/v2/time_entries/{id}")]
	System.Threading.Tasks.Task DeleteAsync(long id, CancellationToken cancellationToken);

	[Post("/v2/time_entries")]
	Task<TimeEntry> CreateAsync(TimeEntryCreationDto creationDto, CancellationToken cancellationToken);

	[Patch("/v2/time_entries/{id}")]
	Task<TimeEntry> PatchAsync(long id, TimeEntryPatchDto patchDto, CancellationToken cancellationToken);
}