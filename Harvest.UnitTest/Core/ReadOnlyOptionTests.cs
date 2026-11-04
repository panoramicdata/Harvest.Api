using Harvest.UnitTest.Support;

namespace Harvest.UnitTest.Core;

public class ReadOnlyOptionTests
{
	private static HarvestClientOptions ReadOnlyOptions(bool isReadOnly) => new()
	{
		AccountId = TestClient.AccountId,
		AccessToken = TestClient.AccessToken,
		MaxRetries = 0,
		IsReadOnly = isReadOnly
	};

	[Fact]
	public void IsReadOnly_DefaultsToFalse()
		=> new HarvestClientOptions { AccountId = 1, AccessToken = "t" }.IsReadOnly.Should().BeFalse();

	[Fact]
	public async Task Delete_WhenReadOnly_ThrowsAndSendsNothing()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.OK);
		using var client = TestClient.Create(stub, ReadOnlyOptions(isReadOnly: true));

		var act = () => client.Projects.DeleteAsync(1, TestClient.Ct);

		await act.Should().ThrowAsync<Exception>().Where(e => e is InvalidOperationException || e.InnerException is InvalidOperationException);
		stub.Calls.Should().BeEmpty();
	}

	[Fact]
	public async Task Delete_WhenNotReadOnly_IsSent()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.OK);
		using var client = TestClient.Create(stub, ReadOnlyOptions(isReadOnly: false));

		await client.Projects.DeleteAsync(1, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Which.Method.Should().Be(HttpMethod.Delete);
	}

	[Fact]
	public async Task Get_WhenReadOnly_IsStillSent()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.OK, "{}");
		using var client = TestClient.Create(stub, ReadOnlyOptions(isReadOnly: true));

		await client.Projects.GetAsync(1, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Which.Method.Should().Be(HttpMethod.Get);
	}

	[Fact]
	public async Task ChangingIsReadOnlyAfterConstruction_TakesEffectOnTheNextRequest()
	{
		var options = ReadOnlyOptions(isReadOnly: false);
		var stub = new StubHandler().Enqueue(HttpStatusCode.OK).Enqueue(HttpStatusCode.OK);
		using var client = TestClient.Create(stub, options);

		await client.Projects.DeleteAsync(1, TestClient.Ct);
		options.IsReadOnly = true;
		var act = () => client.Projects.DeleteAsync(2, TestClient.Ct);

		await act.Should().ThrowAsync<Exception>();
		stub.Calls.Should().ContainSingle();
	}
}
