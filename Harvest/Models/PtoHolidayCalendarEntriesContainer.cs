namespace Harvest.Models;

/// <summary>A page of <c>GET /v2/pto/holiday_calendars/{id}/entries</c>.</summary>
public class PtoHolidayCalendarEntriesContainer : ListContainerBase
{
	/// <summary>The holiday entries on this page (<c>pto_holiday_calendar_entries</c>).</summary>
	public List<PtoHolidayCalendarEntry> PtoHolidayCalendarEntries { get; set; } = [];
}