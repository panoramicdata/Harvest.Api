namespace Harvest.Serialization;

/// <summary>Parses the clock times Harvest sends for time entries.</summary>
internal static class HarvestTime
{
	// 12-hour times ("8:00am", "12:30pm") follow the company's 12h clock setting; 24-hour times ("13:00") the 24h one.
	private static readonly string[] Formats = ["h:mmtt", "hh:mmtt", "h:mm tt", "hh:mm tt", "H:mm", "HH:mm"];

	/// <summary>
	/// Parses <paramref name="value"/> as a time of day, tolerating either clock and either case of <c>am</c>/<c>pm</c>.
	/// </summary>
	/// <param name="value">The time as Harvest sent it, for example <c>8:00am</c>, <c>12:30pm</c> or <c>13:00</c>.</param>
	/// <returns>The time since midnight, or <see langword="null"/> when the value is missing or not a recognised time.</returns>
	public static TimeSpan? Parse(string? value)
		=> !string.IsNullOrWhiteSpace(value)
			&& DateTime.TryParseExact(value.Trim(), Formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed)
				? parsed.TimeOfDay
				: null;
}