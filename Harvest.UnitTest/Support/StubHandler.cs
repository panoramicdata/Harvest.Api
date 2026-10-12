using System.Net.Http.Headers;
using System.Text;

namespace Harvest.UnitTest.Support;

/// <summary>One request as <see cref="StubHandler"/> received it.</summary>
/// <param name="Method">The HTTP method.</param>
/// <param name="Uri">The absolute request URI.</param>
/// <param name="Body">The body as UTF-8 text, or <see langword="null"/> when there is none.</param>
/// <param name="Headers">The request headers.</param>
internal sealed record RecordedCall(HttpMethod Method, Uri Uri, string? Body, HttpRequestHeaders Headers)
{
	/// <summary>The body's media type, or <see langword="null"/> when there is none.</summary>
	public string? ContentType { get; init; }
}

/// <summary>
/// A transport that records every request and answers each from a queue of canned responses, so a
/// <see cref="HarvestClient"/> can be exercised without a network. Pass it to
/// <see cref="HarvestClient(HarvestClientOptions, HttpMessageHandler)"/>, usually through <see cref="TestClient"/>.
/// </summary>
internal sealed class StubHandler : HttpMessageHandler
{
	private readonly Queue<Func<HttpResponseMessage>> _responses = new();

	/// <summary>The requests received, in order.</summary>
	public List<RecordedCall> Calls { get; } = [];

	/// <summary>Queues a JSON response.</summary>
	/// <param name="status">The status code.</param>
	/// <param name="json">The body.</param>
	/// <param name="configure">Changes the response further, for example to add a <c>Retry-After</c> header.</param>
	/// <returns>This stub, for chaining.</returns>
	public StubHandler Enqueue(HttpStatusCode status, string json = "{}", Action<HttpResponseMessage>? configure = null)
	{
		_responses.Enqueue(() =>
		{
			var response = new HttpResponseMessage(status)
			{
				Content = new StringContent(json, Encoding.UTF8, "application/json")
			};
			configure?.Invoke(response);
			return response;
		});
		return this;
	}

	/// <inheritdoc />
	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var body = request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
		Calls.Add(new RecordedCall(request.Method, request.RequestUri!, body, request.Headers)
		{
			ContentType = request.Content?.Headers.ContentType?.MediaType
		});
		var response = _responses.Dequeue()();
		response.RequestMessage = request;
		return response;
	}
}