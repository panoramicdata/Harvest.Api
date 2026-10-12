using Harvest.Handlers;
using Harvest.UnitTest.Support;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace Harvest.UnitTest.Core;

public class RetryHandlerTests
{
	private const string Url = "https://api.harvestapp.com/v2/time_entries?user_id=1";

	/// <summary>A retry handler over a stub, recording the delays it would wait instead of waiting.</summary>
	private sealed class Harness : IDisposable
	{
		private readonly HttpMessageInvoker _invoker;

		public Harness(int maxRetries = 3, TimeSpan? maxRetryDelay = null)
		{
			Handler = new RetryHandler(Logger, maxRetries, TimeSpan.FromSeconds(1), maxRetryDelay ?? TimeSpan.FromSeconds(30))
			{
				InnerHandler = Stub,
				Delay = (delay, _) =>
				{
					Delays.Add(delay);
					return Task.CompletedTask;
				},
				TimeProvider = Clock
			};
			_invoker = new HttpMessageInvoker(Handler);
		}

		public RetryHandler Handler { get; }

		public StubHandler Stub { get; } = new();

		public List<TimeSpan> Delays { get; } = [];

		public CapturingLogger Logger { get; } = new();

		public FakeTimeProvider Clock { get; } = new(new DateTimeOffset(2025, 1, 1, 12, 0, 0, TimeSpan.Zero));

		public Harness Respond(params HttpStatusCode[] statuses)
		{
			foreach (var status in statuses)
			{
				Stub.Enqueue(status);
			}

			return this;
		}

		public Task<HttpResponseMessage> SendAsync(HttpMethod method, HttpContent? content = null)
			=> _invoker.SendAsync(new HttpRequestMessage(method, Url) { Content = content }, TestClient.Ct);

		public void Dispose() => _invoker.Dispose();
	}

	[Theory]
	[InlineData("GET", HttpStatusCode.InternalServerError)]
	[InlineData("PUT", HttpStatusCode.BadGateway)]
	[InlineData("DELETE", HttpStatusCode.GatewayTimeout)]
	[InlineData("POST", HttpStatusCode.ServiceUnavailable)]
	[InlineData("POST", HttpStatusCode.TooManyRequests)]
	[InlineData("PATCH", HttpStatusCode.TooManyRequests)]
	public async Task RetriesTransientFailures_WithExponentialBackoff(string method, HttpStatusCode status)
	{
		using var harness = new Harness().Respond(status, status, HttpStatusCode.OK);

		using var response = await harness.SendAsync(new HttpMethod(method));

		response.StatusCode.Should().Be(HttpStatusCode.OK);
		harness.Stub.Calls.Should().HaveCount(3);
		harness.Delays.Should().Equal(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));
	}

	[Theory]
	[InlineData("POST", HttpStatusCode.InternalServerError)]
	[InlineData("PATCH", HttpStatusCode.BadGateway)]
	[InlineData("GET", HttpStatusCode.NotFound)]
	[InlineData("GET", HttpStatusCode.UnprocessableEntity)]
	public async Task DoesNotRetry_NonTransientOrUnsafeFailures(string method, HttpStatusCode status)
	{
		using var harness = new Harness().Respond(status);

		using var response = await harness.SendAsync(new HttpMethod(method));

		response.StatusCode.Should().Be(status);
		harness.Stub.Calls.Should().ContainSingle();
		harness.Delays.Should().BeEmpty();
	}

	[Fact]
	public async Task GivesUp_AfterMaxRetries_ReturningTheLastResponse()
	{
		using var harness = new Harness(maxRetries: 2).Respond(HttpStatusCode.TooManyRequests, HttpStatusCode.TooManyRequests, HttpStatusCode.TooManyRequests);

		using var response = await harness.SendAsync(HttpMethod.Get);

		response.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
		harness.Stub.Calls.Should().HaveCount(3);
		harness.Delays.Should().HaveCount(2);
	}

	[Fact]
	public async Task ZeroRetries_SendsOnce()
	{
		using var harness = new Harness(maxRetries: 0).Respond(HttpStatusCode.TooManyRequests);

		using var response = await harness.SendAsync(HttpMethod.Get);

		response.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
		harness.Delays.Should().BeEmpty();
	}

	[Fact]
	public async Task HonoursRetryAfterSeconds()
	{
		using var harness = new Harness();
		harness.Stub
			.Enqueue(HttpStatusCode.TooManyRequests, configure: r => r.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(15)))
			.Enqueue(HttpStatusCode.OK);

		using var response = await harness.SendAsync(HttpMethod.Get);

		response.StatusCode.Should().Be(HttpStatusCode.OK);
		harness.Delays.Should().Equal(TimeSpan.FromSeconds(15));
	}

	[Fact]
	public async Task HonoursRetryAfterDate_MeasuredAgainstTheClock()
	{
		using var harness = new Harness();
		harness.Stub
			.Enqueue(HttpStatusCode.TooManyRequests, configure: r => r.Headers.RetryAfter = new RetryConditionHeaderValue(harness.Clock.GetUtcNow().AddSeconds(7)))
			.Enqueue(HttpStatusCode.OK);

		using var response = await harness.SendAsync(HttpMethod.Get);

		harness.Delays.Should().Equal(TimeSpan.FromSeconds(7));
	}

	[Fact]
	public async Task RetryAfterDateInThePast_RetriesImmediately()
	{
		using var harness = new Harness();
		harness.Stub
			.Enqueue(HttpStatusCode.ServiceUnavailable, configure: r => r.Headers.RetryAfter = new RetryConditionHeaderValue(harness.Clock.GetUtcNow().AddSeconds(-5)))
			.Enqueue(HttpStatusCode.OK);

		using var response = await harness.SendAsync(HttpMethod.Get);

		harness.Delays.Should().Equal(TimeSpan.Zero);
	}

	[Fact]
	public async Task RetryAfter_IsCappedAtMaxRetryDelay()
	{
		using var harness = new Harness(maxRetryDelay: TimeSpan.FromSeconds(10));
		harness.Stub
			.Enqueue(HttpStatusCode.TooManyRequests, configure: r => r.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromMinutes(15)))
			.Enqueue(HttpStatusCode.OK);

		using var response = await harness.SendAsync(HttpMethod.Get);

		harness.Delays.Should().Equal(TimeSpan.FromSeconds(10));
	}

	[Fact]
	public async Task Backoff_DoublesUpToMaxRetryDelay()
	{
		using var harness = new Harness(maxRetries: 4, maxRetryDelay: TimeSpan.FromSeconds(3))
			.Respond(HttpStatusCode.BadGateway, HttpStatusCode.BadGateway, HttpStatusCode.BadGateway, HttpStatusCode.BadGateway, HttpStatusCode.OK);

		using var response = await harness.SendAsync(HttpMethod.Get);

		harness.Delays.Should().Equal(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(3));
	}

	[Fact]
	public async Task LogsEachRetry_WithThePathButNotTheQuery()
	{
		using var harness = new Harness().Respond(HttpStatusCode.TooManyRequests, HttpStatusCode.OK);

		using var response = await harness.SendAsync(HttpMethod.Get);

		var (level, message) = harness.Logger.Entries.Should().ContainSingle().Subject;
		level.Should().Be(LogLevel.Warning);
		message.Should().Contain("429").And.Contain("GET /v2/time_entries").And.NotContain("user_id");
	}

	[Fact]
	public async Task JsonBody_IsResentOnRetry()
	{
		using var harness = new Harness().Respond(HttpStatusCode.TooManyRequests, HttpStatusCode.Created);

		using var response = await harness.SendAsync(HttpMethod.Post, JsonContent.Create(new { name = "x" }));

		response.StatusCode.Should().Be(HttpStatusCode.Created);
		harness.Stub.Calls.Select(c => c.Body).Should().Equal("""{"name":"x"}""", """{"name":"x"}""");
	}

	[Fact]
	public async Task StreamBody_IsNotRetried()
	{
		using var harness = new Harness().Respond(HttpStatusCode.TooManyRequests);

		using var response = await harness.SendAsync(HttpMethod.Post, new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes("x"))));

		response.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
		harness.Stub.Calls.Should().ContainSingle();
	}

	[Fact]
	public void IsReplayable_ClassifiesContent()
	{
		RetryHandler.IsReplayable(null).Should().BeTrue();
		RetryHandler.IsReplayable(new StringContent("x")).Should().BeTrue();
		RetryHandler.IsReplayable(new ReadOnlyMemoryContent(new byte[1])).Should().BeTrue();
		RetryHandler.IsReplayable(JsonContent.Create(1)).Should().BeTrue();
		RetryHandler.IsReplayable(new StreamContent(Stream.Null)).Should().BeFalse();
	}

	[Fact]
	public async Task ThroughTheClient_A429IsRetriedThenReadNormally()
	{
		var stub = new StubHandler()
			.Enqueue(HttpStatusCode.TooManyRequests, """{"message":"slow down"}""", r => r.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.Zero))
			.Enqueue(HttpStatusCode.Created, """{"id":7,"name":"Acme"}""");
		var options = new HarvestClientOptions { AccountId = 1, AccessToken = "t", MaxRetries = 1, Logger = NullLogger.Instance };
		using var client = new HarvestClient(options, stub);

		var created = await client.Clients.CreateAsync(new ClientCreationDto { Name = "Acme" }, TestClient.Ct);

		created.Id.Should().Be(7);
		stub.Calls.Should().HaveCount(2);
		stub.Calls.Select(c => c.Body).Should().AllBe("""{"name":"Acme"}""");
	}
}