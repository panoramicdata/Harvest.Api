namespace Harvest.Serialization;

/// <summary>
/// Formats the text of a <c>multipart/form-data</c> field the way Harvest reads it: booleans as lowercase
/// <c>true</c>/<c>false</c>, dates as <c>YYYY-MM-DD</c>, numbers in the invariant culture. A <see langword="null"/> value
/// gives <see langword="null"/>, which Refit omits from the form.
/// </summary>
internal static class HarvestFormValue
{
	/// <summary>Formats an ID or other integer.</summary>
	/// <param name="value">The value.</param>
	/// <returns>The field text, or <see langword="null"/> to omit the field.</returns>
	public static string? Of(long? value) => value?.ToString(CultureInfo.InvariantCulture);

	/// <summary>Formats a decimal.</summary>
	/// <param name="value">The value.</param>
	/// <returns>The field text, or <see langword="null"/> to omit the field.</returns>
	public static string? Of(decimal? value) => value?.ToString(CultureInfo.InvariantCulture);

	/// <summary>Formats a boolean as <c>true</c> or <c>false</c>.</summary>
	/// <param name="value">The value.</param>
	/// <returns>The field text, or <see langword="null"/> to omit the field.</returns>
	public static string? Of(bool? value) => value switch
	{
		true => "true",
		false => "false",
		null => null
	};

	/// <summary>Formats a date as <c>YYYY-MM-DD</c>.</summary>
	/// <param name="value">The value.</param>
	/// <returns>The field text, or <see langword="null"/> to omit the field.</returns>
	public static string? Of(DateOnly? value) => value?.ToString(HarvestUrlParameterFormatter.DateFormat, CultureInfo.InvariantCulture);
}