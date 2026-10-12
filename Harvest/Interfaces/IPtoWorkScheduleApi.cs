namespace Harvest.Interfaces;

/// <summary>
/// PTO work schedules (<see href="https://help.getharvest.com/api-v2/pto-api/pto/work-schedules/"/>). Only
/// administrators can use them.
/// </summary>
/// <remarks>
/// The PTO API is being rolled out gradually: an account without it receives <c>403 Forbidden</c> (a
/// <see cref="HarvestApiException"/>) from every call.
/// </remarks>
public interface IPtoWorkScheduleApi
{
	/// <summary>Lists one page of work schedules.</summary>
	/// <param name="options">
	/// Filtering and paging (<c>is_active</c>, Harvest's default <see langword="true"/>; <c>page</c>; <c>per_page</c>), or
	/// <see langword="null"/> for none.
	/// </param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of work schedules.</returns>
	[Get("/v2/pto/work_schedules")]
	Task<PtoWorkSchedulesContainer> ListAsync([Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>Gets a work schedule.</summary>
	/// <param name="id">The work schedule ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The work schedule.</returns>
	[Get("/v2/pto/work_schedules/{id}")]
	Task<PtoWorkSchedule> GetAsync(long id, CancellationToken cancellationToken);

	/// <summary>Creates a work schedule.</summary>
	/// <param name="creationDto">The new work schedule.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The created work schedule.</returns>
	[Post("/v2/pto/work_schedules")]
	Task<PtoWorkSchedule> CreateAsync([Body] PtoWorkScheduleCreationDto creationDto, CancellationToken cancellationToken);

	/// <summary>Updates a work schedule; fields left <see langword="null"/> are unchanged.</summary>
	/// <param name="id">The work schedule ID.</param>
	/// <param name="patchDto">The fields to change.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The updated work schedule.</returns>
	[Patch("/v2/pto/work_schedules/{id}")]
	Task<PtoWorkSchedule> UpdateAsync(long id, [Body] PtoWorkSchedulePatchDto patchDto, CancellationToken cancellationToken);

	/// <summary>
	/// Deactivates a non-default work schedule (<c>DELETE /v2/pto/work_schedules/{id}</c>). The schedule is not deleted.
	/// </summary>
	/// <param name="id">The work schedule ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The deactivated work schedule, with <see cref="PtoWorkSchedule.IsActive"/> <see langword="false"/>.</returns>
	[Delete("/v2/pto/work_schedules/{id}")]
	Task<PtoWorkSchedule> DeactivateAsync(long id, CancellationToken cancellationToken);
}