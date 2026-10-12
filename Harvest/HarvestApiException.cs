using System.Net;
using System.Net.Http.Headers;

namespace Harvest;

/// <summary>
/// Raised when Harvest answers with a non-success status. It is a Refit <see cref="ApiException"/>, so
/// <see cref="ApiException.StatusCode"/>, <see cref="ApiException.Content"/> and the response headers are available;
/// <see cref="Exception.Message"/> is Harvest's own error text where the body carries one.
/// </summary>
/// <remarks>
/// Harvest reports errors as <c>{"message":"..."}</c> (validation failures, HTTP 422),
/// <c>{"error":"invalid_token","error_description":"..."}</c> (authentication, HTTP 401) or
/// <c>{"status":404,"error":"Not Found"}</c>.
/// </remarks>
public sealed class HarvestApiException : ApiException
{
	internal HarvestApiException(
		string message,
		string? errorCode,
		string? errorMessage,
		HttpRequestMessage request,
		string? content,
		HttpStatusCode statusCode,
		string? reasonPhrase,
		HttpResponseHeaders headers)
		: base(message, request, request.Method, content, statusCode, reasonPhrase, headers, HarvestClient.Settings)
	{
		ErrorCode = errorCode;
		ErrorMessage = errorMessage;
	}

	/// <summary>
	/// Harvest's machine-readable error code (the <c>error</c> field, for example <c>invalid_token</c>), or
	/// <see langword="null"/> when the body has none.
	/// </summary>
	public string? ErrorCode { get; }

	/// <summary>
	/// Harvest's human-readable error text (the <c>message</c> or <c>error_description</c> field), or
	/// <see langword="null"/> when the body has none.
	/// </summary>
	public string? ErrorMessage { get; }
}