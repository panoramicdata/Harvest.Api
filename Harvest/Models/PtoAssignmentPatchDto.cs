using Harvest.Dtos;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Harvest.Models;

/// <summary>
/// The body of <c>PATCH /v2/pto/assignments/{user_id}</c>. Harvest unassigns on an explicit JSON <c>null</c>, which a
/// <see langword="null"/> property cannot express (it is omitted, leaving the assignment unchanged), so unassigning has
/// its own flags.
/// </summary>
public class PtoAssignmentPatchDto : PatchDto
{
	private static readonly JsonElement JsonNull = JsonSerializer.SerializeToElement<object?>(null);

	/// <summary>ID of the holiday calendar to assign; <see langword="null"/> leaves it unchanged.</summary>
	[JsonIgnore]
	public long? HolidayCalendarId { get; set; }

	/// <summary>
	/// <see langword="true"/> to unassign the holiday calendar (sends <c>"holiday_calendar_id": null</c>); takes precedence
	/// over <see cref="HolidayCalendarId"/>.
	/// </summary>
	[JsonIgnore]
	public bool UnassignHolidayCalendar { get; set; }

	/// <summary>ID of the work schedule to assign; <see langword="null"/> leaves it unchanged.</summary>
	[JsonIgnore]
	public long? WorkScheduleId { get; set; }

	/// <summary>
	/// <see langword="true"/> to unassign the work schedule, reverting to the company default (sends
	/// <c>"work_schedule_id": null</c>); takes precedence over <see cref="WorkScheduleId"/>.
	/// </summary>
	[JsonIgnore]
	public bool UnassignWorkSchedule { get; set; }

	/// <summary>The <c>holiday_calendar_id</c> written to JSON: an ID, an explicit <c>null</c>, or omitted.</summary>
	[JsonInclude]
	[JsonPropertyName("holiday_calendar_id")]
	internal JsonElement? HolidayCalendarIdJson => ToJson(HolidayCalendarId, UnassignHolidayCalendar);

	/// <summary>The <c>work_schedule_id</c> written to JSON: an ID, an explicit <c>null</c>, or omitted.</summary>
	[JsonInclude]
	[JsonPropertyName("work_schedule_id")]
	internal JsonElement? WorkScheduleIdJson => ToJson(WorkScheduleId, UnassignWorkSchedule);

	private static JsonElement? ToJson(long? id, bool unassign)
		=> unassign ? JsonNull : id is { } value ? JsonSerializer.SerializeToElement(value) : null;
}