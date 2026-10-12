namespace Harvest.UnitTest.Support;

/// <summary>
/// Builds <see cref="HarvestClient"/>s over a <see cref="StubHandler"/> and sends one call through them. Every resource
/// test uses <see cref="SendAsync"/> to pin a request and <see cref="ReadAsync{T}"/> to check how a response is read.
/// </summary>
internal static class TestClient
{
	/// <summary>The account ID every test client sends.</summary>
	public const int AccountId = 12345;

	/// <summary>The access token every test client sends.</summary>
	public const string AccessToken = "test-token";

	/// <summary>The cancellation token of the running test.</summary>
	public static CancellationToken Ct => TestContext.Current.CancellationToken;

	/// <summary>Options for a test client: fixed credentials and no retries.</summary>
	public static HarvestClientOptions Options() => new()
	{
		AccountId = AccountId,
		AccessToken = AccessToken,
		MaxRetries = 0
	};

	/// <summary>Creates a client whose transport is <paramref name="stub"/>.</summary>
	/// <param name="stub">The transport.</param>
	/// <param name="options">The options, or <see langword="null"/> for <see cref="Options"/>.</param>
	public static HarvestClient Create(StubHandler stub, HarvestClientOptions? options = null)
		=> new(options ?? Options(), stub);

	/// <summary>
	/// Sends <paramref name="call"/> through a client whose transport answers <paramref name="status"/> and
	/// <paramref name="response"/>, and returns the one request it made.
	/// </summary>
	public static async Task<RecordedCall> SendAsync(
		Func<HarvestClient, CancellationToken, Task> call,
		string response = "{}",
		HttpStatusCode status = HttpStatusCode.OK)
	{
		var stub = new StubHandler().Enqueue(status, response);
		using var client = Create(stub);
		await call(client, Ct);
		return stub.Calls.Should().ContainSingle().Subject;
	}

	/// <summary>Sends <paramref name="call"/> through a client whose transport answers <paramref name="response"/>, and returns what it read.</summary>
	public static async Task<T> ReadAsync<T>(Func<HarvestClient, CancellationToken, Task<T>> call, string response)
	{
		using var client = Create(new StubHandler().Enqueue(HttpStatusCode.OK, response));
		return await call(client, Ct);
	}
}