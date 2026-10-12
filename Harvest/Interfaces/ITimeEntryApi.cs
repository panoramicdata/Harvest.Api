namespace Harvest.Interfaces;

/// <summary>Time entries (<see href="https://help.getharvest.com/api-v2/timesheets-api/timesheets/time-entries/"/>).</summary>
public interface ITimeEntryApi
{
	/// <summary>Lists one page of time entries, most recently spent first.</summary>
	/// <param name="options">Filtering and paging, or <see langword="null"/> for none.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of time entries.</returns>
	[Get("/v2/time_entries")]
	Task<TimeEntriesContainer> ListAsync([Query] TimeEntryListOptions? options, CancellationToken cancellationToken);

	/// <summary>Gets a time entry.</summary>
	/// <param name="id">The time entry ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The time entry.</returns>
	[Get("/v2/time_entries/{id}")]
	Task<TimeEntry> GetAsync(long id, CancellationToken cancellationToken);

	/// <summary>Creates a time entry, by duration or by start and end time depending on the company's settings.</summary>
	/// <param name="creationDto">The new time entry.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The created time entry.</returns>
	[Post("/v2/time_entries")]
	Task<TimeEntry> CreateAsync([Body] TimeEntryCreationDto creationDto, CancellationToken cancellationToken);

	/// <summary>Updates a time entry; fields left <see langword="null"/> are unchanged.</summary>
	/// <param name="id">The time entry ID.</param>
	/// <param name="patchDto">The fields to change.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The updated time entry.</returns>
	[Patch("/v2/time_entries/{id}")]
	Task<TimeEntry> UpdateAsync(long id, [Body] TimeEntryPatchDto patchDto, CancellationToken cancellationToken);

	/// <summary>Deletes a time entry. Harvest refuses (HTTP 422) once it is closed or locked.</summary>
	/// <param name="id">The time entry ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A task that completes when the time entry is deleted.</returns>
	[Delete("/v2/time_entries/{id}")]
	System.Threading.Tasks.Task DeleteAsync(long id, CancellationToken cancellationToken);

	/// <summary>Deletes a time entry's external reference, leaving the time entry itself in place.</summary>
	/// <param name="id">The time entry ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A task that completes when the external reference is deleted.</returns>
	[Delete("/v2/time_entries/{id}/external_reference")]
	System.Threading.Tasks.Task DeleteExternalReferenceAsync(long id, CancellationToken cancellationToken);

	/// <summary>Restarts a stopped time entry's timer. Only possible while the time entry is not running.</summary>
	/// <param name="id">The time entry ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The running time entry.</returns>
	[Patch("/v2/time_entries/{id}/restart")]
	Task<TimeEntry> RestartAsync(long id, CancellationToken cancellationToken);

	/// <summary>Stops a running time entry's timer. Only possible while the time entry is running.</summary>
	/// <param name="id">The time entry ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The stopped time entry.</returns>
	[Patch("/v2/time_entries/{id}/stop")]
	Task<TimeEntry> StopAsync(long id, CancellationToken cancellationToken);
}