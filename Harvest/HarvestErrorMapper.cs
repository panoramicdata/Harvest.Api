using System.Text.Json;

namespace Harvest;

/// <summary>Turns a non-success response into a <see cref="HarvestApiException"/> carrying Harvest's error text.</summary>
internal static class HarvestErrorMapper
{
	public static async Task<Exception?> CreateAsync(HttpResponseMessage response)
	{
		if (response.IsSuccessStatusCode)
		{
			return null;
		}

		var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
		var (errorCode, errorMessage) = Parse(content);
		var status = $"HTTP {(int)response.StatusCode} ({response.ReasonPhrase ?? response.StatusCode.ToString()})";
		var detail = errorMessage ?? errorCode;
		var message = detail is null ? $"Harvest returned {status}." : $"Harvest returned {status}: {detail}";
		return new HarvestApiException(
			message,
			errorCode,
			errorMessage,
			response.RequestMessage!,
			content,
			response.StatusCode,
			response.ReasonPhrase,
			response.Headers);
	}

	/// <summary>Reads the <c>error</c> code and the <c>message</c> (else <c>error_description</c>) text from a JSON error body.</summary>
	internal static (string? ErrorCode, string? ErrorMessage) Parse(string content)
	{
		try
		{
			using var document = JsonDocument.Parse(content);
			var root = document.RootElement;
			return root.ValueKind == JsonValueKind.Object
				? (ReadString(root, "error"), ReadString(root, "message") ?? ReadString(root, "error_description"))
				: (null, null);
		}
		catch (JsonException)
		{
			return (null, null);
		}
	}

	private static string? ReadString(JsonElement element, string name)
		=> element.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String && value.GetString() is { Length: > 0 } text
			? text
			: null;
}