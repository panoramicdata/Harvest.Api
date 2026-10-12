namespace Harvest.Models;

/// <summary>A PTO holiday calendar (<see href="https://help.getharvest.com/api-v2/pto-api/pto/holiday-calendars/"/>).</summary>
public class PtoHolidayCalendar
{
	/// <summary>Unique ID for the holiday calendar.</summary>
	public long Id { get; set; }

	/// <summary>Name of the calendar.</summary>
	public string? Name { get; set; }

	/// <summary>Total count of scheduled holidays in the calendar.</summary>
	public int? EntriesCount { get; set; }

	/// <summary>
	/// Sorted list of the calendar's holiday entries. Harvest nests only their id, name and date here; the other entry
	/// fields are <see langword="null"/>.
	/// </summary>
	public List<PtoHolidayCalendarEntry>? Entries { get; set; }

	/// <summary>Date and time the calendar was created.</summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>Date and time the calendar was last updated.</summary>
	public DateTime UpdatedAt { get; set; }
}

/// <summary>A holiday entry in a PTO holiday calendar.</summary>
public class PtoHolidayCalendarEntry
{
	/// <summary>Unique ID for the holiday entry.</summary>
	public long Id { get; set; }

	/// <summary>The parent holiday calendar ID (absent when nested in a calendar).</summary>
	public long? PtoHolidayCalendarId { get; set; }

	/// <summary>Name of the holiday.</summary>
	public string? Name { get; set; }

	/// <summary>Date of the holiday closure.</summary>
	public DateOnly? Date { get; set; }

	/// <summary>Date and time the entry was created (absent when nested in a calendar).</summary>
	public DateTime? CreatedAt { get; set; }

	/// <summary>Date and time the entry was last updated (absent when nested in a calendar).</summary>
	public DateTime? UpdatedAt { get; set; }
}