namespace Harvest.Interfaces;

public interface ITimeEntryApi
{
	[Get("/v2/time_entries")]
	Task<TimeEntriesContainer> ListAllAsync([Query] TimeEntryListOptions? options, CancellationToken cancellationToken);

	[Get("/v2/time_entries/{id}")]
	Task<TimeEntry> GetAsync(long id, CancellationToken cancellationToken);

	[Delete("/v2/time_entries/{id}")]
	System.Threading.Tasks.Task DeleteAsync(long id, CancellationToken cancellationToken);

	[Post("/v2/time_entries")]
	Task<TimeEntry> CreateAsync(TimeEntryCreationDto creationDto, CancellationToken cancellationToken);

	[Patch("/v2/time_entries/{id}")]
	Task<TimeEntry> PatchAsync(long id, TimeEntryPatchDto patchDto, CancellationToken cancellationToken);
}