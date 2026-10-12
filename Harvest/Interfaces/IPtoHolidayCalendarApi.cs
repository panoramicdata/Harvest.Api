namespace Harvest.Interfaces;

/// <summary>
/// PTO holiday calendars and their entries (<see href="https://help.getharvest.com/api-v2/pto-api/pto/holiday-calendars/"/>).
/// Only administrators can use them.
/// </summary>
/// <remarks>
/// The PTO API is being rolled out gradually: an account without it receives <c>403 Forbidden</c> (a
/// <see cref="HarvestApiException"/>) from every call.
/// </remarks>
public interface IPtoHolidayCalendarApi
{
	/// <summary>Lists one page of holiday calendars.</summary>
	/// <param name="options">Paging (<c>page</c>, <c>per_page</c>), or <see langword="null"/> for none.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of holiday calendars.</returns>
	[Get("/v2/pto/holiday_calendars")]
	Task<PtoHolidayCalendarsContainer> ListAsync([Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>Gets a holiday calendar.</summary>
	/// <param name="id">The holiday calendar ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The holiday calendar.</returns>
	[Get("/v2/pto/holiday_calendars/{id}")]
	Task<PtoHolidayCalendar> GetAsync(long id, CancellationToken cancellationToken);

	/// <summary>Creates a holiday calendar, optionally populated with a country's national public holidays.</summary>
	/// <param name="creationDto">The new holiday calendar.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The created holiday calendar.</returns>
	[Post("/v2/pto/holiday_calendars")]
	Task<PtoHolidayCalendar> CreateAsync([Body] PtoHolidayCalendarCreationDto creationDto, CancellationToken cancellationToken);

	/// <summary>Renames a holiday calendar.</summary>
	/// <param name="id">The holiday calendar ID.</param>
	/// <param name="patchDto">The new name.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The updated holiday calendar.</returns>
	[Patch("/v2/pto/holiday_calendars/{id}")]
	Task<PtoHolidayCalendar> UpdateAsync(long id, [Body] PtoHolidayCalendarPatchDto patchDto, CancellationToken cancellationToken);

	/// <summary>Deletes a holiday calendar.</summary>
	/// <param name="id">The holiday calendar ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The deleted holiday calendar.</returns>
	[Delete("/v2/pto/holiday_calendars/{id}")]
	Task<PtoHolidayCalendar> DeleteAsync(long id, CancellationToken cancellationToken);

	/// <summary>Lists one page of the holiday entries of a calendar.</summary>
	/// <param name="holidayCalendarId">The holiday calendar ID.</param>
	/// <param name="options">Paging (<c>page</c>, <c>per_page</c>), or <see langword="null"/> for none.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of holiday entries.</returns>
	[Get("/v2/pto/holiday_calendars/{holidayCalendarId}/entries")]
	Task<PtoHolidayCalendarEntriesContainer> ListEntriesAsync(long holidayCalendarId, [Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>Creates a holiday entry in a calendar, for a single date or (with <c>date_mode</c> <c>range</c>) a range of dates.</summary>
	/// <param name="holidayCalendarId">The holiday calendar ID.</param>
	/// <param name="creationDto">The new holiday entry.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The created holiday entry.</returns>
	[Post("/v2/pto/holiday_calendars/{holidayCalendarId}/entries")]
	Task<PtoHolidayCalendarEntry> CreateEntryAsync(long holidayCalendarId, [Body] PtoHolidayCalendarEntryCreationDto creationDto, CancellationToken cancellationToken);

	/// <summary>Updates a holiday entry; fields left <see langword="null"/> are unchanged.</summary>
	/// <param name="holidayCalendarId">The holiday calendar ID.</param>
	/// <param name="id">The holiday entry ID.</param>
	/// <param name="patchDto">The fields to change.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The updated holiday entry.</returns>
	[Patch("/v2/pto/holiday_calendars/{holidayCalendarId}/entries/{id}")]
	Task<PtoHolidayCalendarEntry> UpdateEntryAsync(long holidayCalendarId, long id, [Body] PtoHolidayCalendarEntryPatchDto patchDto, CancellationToken cancellationToken);

	/// <summary>Deletes a holiday entry.</summary>
	/// <param name="holidayCalendarId">The holiday calendar ID.</param>
	/// <param name="id">The holiday entry ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The deleted holiday entry.</returns>
	[Delete("/v2/pto/holiday_calendars/{holidayCalendarId}/entries/{id}")]
	Task<PtoHolidayCalendarEntry> DeleteEntryAsync(long holidayCalendarId, long id, CancellationToken cancellationToken);
}