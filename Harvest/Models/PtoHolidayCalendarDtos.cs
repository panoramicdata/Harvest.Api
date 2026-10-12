using Harvest.Dtos;

namespace Harvest.Models;

/// <summary>The body of <c>POST /v2/pto/holiday_calendars</c>.</summary>
public class PtoHolidayCalendarCreationDto : CreationDto
{
	/// <summary>Name of the calendar.</summary>
	public required string Name { get; set; }

	/// <summary>Optional ISO country code (e.g. <c>US</c>, <c>GB</c>) to auto-populate national public holidays.</summary>
	public string? CountryCode { get; set; }
}

/// <summary>
/// The body of <c>PATCH /v2/pto/holiday_calendars/{id}</c>. Harvest documents the name as required, so unlike most patch
/// bodies it is not optional.
/// </summary>
public class PtoHolidayCalendarPatchDto : PatchDto
{
	/// <summary>Updated name of the calendar.</summary>
	public required string Name { get; set; }
}

/// <summary>
/// The body of <c>POST /v2/pto/holiday_calendars/{id}/entries</c>: set <see cref="Date"/> for a single date, or
/// <see cref="DateMode"/> <c>range</c> with <see cref="StartDate"/> and <see cref="EndDate"/> for a range of closed dates.
/// </summary>
public class PtoHolidayCalendarEntryCreationDto : CreationDto
{
	/// <summary>The <see cref="DateMode"/> value that adds a range of closed dates.</summary>
	public const string RangeDateMode = "range";

	/// <summary>Holiday title.</summary>
	public required string Name { get; set; }

	/// <summary>Holiday date (for single date mode).</summary>
	public DateOnly? Date { get; set; }

	/// <summary>Pass <c>range</c> (<see cref="RangeDateMode"/>) to add a range of closed dates.</summary>
	public string? DateMode { get; set; }

	/// <summary>Start date when <see cref="DateMode"/> is <c>range</c>.</summary>
	public DateOnly? StartDate { get; set; }

	/// <summary>End date when <see cref="DateMode"/> is <c>range</c>.</summary>
	public DateOnly? EndDate { get; set; }
}

/// <summary>The body of <c>PATCH /v2/pto/holiday_calendars/{id}/entries/{entry_id}</c>.</summary>
public class PtoHolidayCalendarEntryPatchDto : PatchDto
{
	/// <summary>Updated title.</summary>
	public string? Name { get; set; }

	/// <summary>Updated date.</summary>
	public DateOnly? Date { get; set; }
}