using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Harvest.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<TimeFormat>))]
public enum TimeFormat
{
	[EnumMember(Value = "decimal")]
	DecimalFormat,

	[EnumMember(Value = "hours_minutes")]
	HoursMinutes
}
