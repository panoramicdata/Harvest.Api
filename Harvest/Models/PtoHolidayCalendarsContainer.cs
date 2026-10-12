namespace Harvest.Models;

/// <summary>A page of <c>GET /v2/pto/holiday_calendars</c>.</summary>
public class PtoHolidayCalendarsContainer : ListContainerBase
{
	/// <summary>The holiday calendars on this page (<c>pto_holiday_calendars</c>).</summary>
	public List<PtoHolidayCalendar> PtoHolidayCalendars { get; set; } = [];
}