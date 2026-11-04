namespace Harvest.Handlers;

/// <summary>
/// Refuses every request other than GET, HEAD and OPTIONS while <see cref="HarvestClientOptions.IsReadOnly"/> is set. The
/// option is read on each request.
/// </summary>
/// <param name="options">The client options.</param>
internal sealed class ReadOnlyHandler(HarvestClientOptions options) : DelegatingHandler
{
	/// <inheritdoc />
	protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		ThrowIfRefused(request);
		return base.SendAsync(request, cancellationToken);
	}

	private void ThrowIfRefused(HttpRequestMessage request)
	{
		if (options.IsReadOnly
			&& request.Method != HttpMethod.Get
			&& request.Method != HttpMethod.Head
			&& request.Method != HttpMethod.Options)
		{
			throw new InvalidOperationException(
				$"The client is read-only, so {request.Method} {request.RequestUri?.AbsolutePath} was not sent.");
		}
	}
}
