using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Harvest.Serialization;

/// <summary>
/// Reads a string property that Harvest documents as a string but may send as a number (external reference IDs, for
/// example): a JSON number is kept as its literal text, so <c>123</c> and <c>"123"</c> both read as <c>"123"</c>. Writes a
/// plain JSON string.
/// </summary>
internal sealed class FlexibleStringConverter : JsonConverter<string>
{
	/// <inheritdoc />
	public override bool HandleNull => false;

	/// <inheritdoc />
	public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> reader.TokenType == JsonTokenType.String ? reader.GetString()
			: reader.TokenType == JsonTokenType.Number ? Encoding.UTF8.GetString(reader.ValueSpan)
			: reader.TokenType == JsonTokenType.True ? "true"
			: reader.TokenType == JsonTokenType.False ? "false"
			: throw new JsonException($"Expected a string or number but found {reader.TokenType}.");

	/// <inheritdoc />
	public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
		=> writer.WriteStringValue(value);
}