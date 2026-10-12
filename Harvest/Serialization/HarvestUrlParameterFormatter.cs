using System.Reflection;

namespace Harvest.Serialization;

/// <summary>
/// Formats path and query values the way Harvest expects them: booleans as lowercase <c>true</c>/<c>false</c>, date-times
/// as ISO 8601 in UTC (<c>2017-06-26T21:52:18Z</c>), dates as <c>YYYY-MM-DD</c>, everything else in the invariant culture.
/// </summary>
internal sealed class HarvestUrlParameterFormatter : DefaultUrlParameterFormatter
{
	/// <summary>The format of a date-time query value, applied after conversion to UTC.</summary>
	internal const string DateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss'Z'";

	/// <summary>The format of a date value (<c>YYYY-MM-DD</c>), shared with <see cref="HarvestFormValue"/>.</summary>
	internal const string DateFormat = "yyyy-MM-dd";

	/// <inheritdoc />
	public override string? Format(object? parameterValue, ICustomAttributeProvider attributeProvider, Type type)
		=> parameterValue switch
		{
			bool value => value ? "true" : "false",
			DateTimeOffset value => value.UtcDateTime.ToString(DateTimeFormat, CultureInfo.InvariantCulture),
			DateOnly value => value.ToString(DateFormat, CultureInfo.InvariantCulture),
			_ => base.Format(parameterValue, attributeProvider, type)
		};
}