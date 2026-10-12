namespace Harvest.UnitTest.Support;

/// <summary>Assertions on a request recorded by <see cref="StubHandler"/>, shared by every resource's tests.</summary>
internal static class RecordedCallAssertions
{
	/// <summary>
	/// Asserts the request's method, escaped path, query string (including the leading <c>?</c>, or empty for none) and
	/// exact JSON body (<see langword="null"/> for none). The body is compared as text, so the assertion also proves that
	/// <see langword="null"/> fields were omitted and that names are snake_case.
	/// </summary>
	public static void ShouldBe(this RecordedCall call, HttpMethod method, string path, string query = "", string? body = null)
	{
		call.Method.Should().Be(method);
		call.Uri.AbsolutePath.Should().Be(path);
		call.Uri.Query.Should().Be(query);
		call.Body.Should().Be(body);
		if (body is not null)
		{
			call.ContentType.Should().Be("application/json");
		}
	}
}