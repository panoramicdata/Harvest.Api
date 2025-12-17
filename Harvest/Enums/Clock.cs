using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Harvest.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<Clock>))]
public enum Clock
{
	[EnumMember(Value = "12h")]
	TwelveHour,

	[EnumMember(Value = "24h")]
	TwentyFourHour
}