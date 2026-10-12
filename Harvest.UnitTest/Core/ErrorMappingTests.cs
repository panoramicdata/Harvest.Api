using Harvest.UnitTest.Support;

namespace Harvest.UnitTest.Core;

public class ErrorMappingTests
{
	private static async Task<HarvestApiException> FailAsync(HttpStatusCode status, string body)
	{
		using var client = TestClient.Create(new StubHandler().Enqueue(status, body));
		var act = () => client.Clients.GetAsync(1, TestClient.Ct);
		return (await act.Should().ThrowAsync<HarvestApiException>()).Which;
	}

	[Fact]
	public async Task ValidationError_SurfacesHarvestsMessage()
	{
		var exception = await FailAsync(HttpStatusCode.UnprocessableEntity, """{"message":"Name has already been taken"}""");

		exception.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
		exception.ErrorMessage.Should().Be("Name has already been taken");
		exception.ErrorCode.Should().BeNull();
		exception.Message.Should().Be("Harvest returned HTTP 422 (Unprocessable Entity): Name has already been taken");
		exception.Content.Should().Be("""{"message":"Name has already been taken"}""");
		exception.HttpMethod.Should().Be(HttpMethod.Get);
		exception.Uri!.AbsolutePath.Should().Be("/v2/clients/1");
	}

	[Fact]
	public async Task AuthenticationError_SurfacesCodeAndDescription()
	{
		var exception = await FailAsync(HttpStatusCode.Unauthorized, """{"error":"invalid_token","error_description":"The access token provided is expired"}""");

		exception.ErrorCode.Should().Be("invalid_token");
		exception.ErrorMessage.Should().Be("The access token provided is expired");
		exception.Message.Should().Be("Harvest returned HTTP 401 (Unauthorized): The access token provided is expired");
	}

	[Fact]
	public async Task NotFound_WithOnlyAnErrorCode_UsesTheCode()
	{
		var exception = await FailAsync(HttpStatusCode.NotFound, """{"status":404,"error":"Not Found"}""");

		exception.ErrorCode.Should().Be("Not Found");
		exception.ErrorMessage.Should().BeNull();
		exception.Message.Should().Be("Harvest returned HTTP 404 (Not Found): Not Found");
	}

	[Theory]
	[InlineData("")]
	[InlineData("<html>Bad gateway</html>")]
	[InlineData("[1,2]")]
	[InlineData("""{"message":""}""")]
	[InlineData("""{"message":42}""")]
	public async Task UnreadableBody_FallsBackToTheStatus(string body)
	{
		var exception = await FailAsync(HttpStatusCode.BadGateway, body);

		exception.ErrorCode.Should().BeNull();
		exception.ErrorMessage.Should().BeNull();
		exception.Message.Should().Be("Harvest returned HTTP 502 (Bad Gateway).");
	}

	[Fact]
	public async Task MissingReasonPhrase_UsesTheStatusName()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.Forbidden, "{}", r => r.ReasonPhrase = null);
		using var client = TestClient.Create(stub);

		var act = () => client.Clients.GetAsync(1, TestClient.Ct);

		(await act.Should().ThrowAsync<HarvestApiException>()).Which.Message.Should().Be("Harvest returned HTTP 403 (Forbidden).");
	}

	[Fact]
	public async Task HarvestApiException_IsARefitApiException()
	{
		using var client = TestClient.Create(new StubHandler().Enqueue(HttpStatusCode.NotFound, "{}"));

		var act = () => client.Clients.DeleteAsync(1, TestClient.Ct);

		await act.Should().ThrowAsync<Refit.ApiException>();
	}

	[Fact]
	public async Task Success_IsNotAnError()
	{
		var exception = await HarvestErrorMapper.CreateAsync(new HttpResponseMessage(HttpStatusCode.OK));

		exception.Should().BeNull();
	}
}