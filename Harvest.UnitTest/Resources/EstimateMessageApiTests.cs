using Harvest.UnitTest.Support;

namespace Harvest.UnitTest.Resources;

public class EstimateMessageApiTests
{
	private const string MessageJson = """
		{"id":2666236,"sent_by":"Bob Powell","sent_by_email":"bob@example.com","sent_from":"Bob Powell",
		"sent_from_email":"bob@example.com","send_me_a_copy":true,"created_at":"2017-08-25T21:23:40Z",
		"updated_at":"2017-08-25T21:23:41Z","recipients":[{"name":"Richard Roe","email":"richard@example.com"},
		{"name":"Bob Powell","email":"bob@example.com"}],"event_type":null,"subject":"Estimate #1001 from API Examples",
		"body":"Estimate Summary\r\nEstimate ID: 1001\r\n\r\n%estimate_url%\r\n\r\nThank you!"}
		""";

	private static string EventMessageJson(string eventType) => $$"""
		{"id":2666241,"sent_by":"Bob Powell","sent_by_email":"bob@example.com","sent_from":"Bob Powell",
		"sent_from_email":"bob@example.com","send_me_a_copy":false,"created_at":"2017-08-23T22:25:59Z",
		"updated_at":"2017-08-23T22:25:59Z","event_type":"{{eventType}}","recipients":[],"subject":null,"body":null}
		""";

	[Fact]
	public async Task ListAsync_NoOptions_SendsNoQuery()
	{
		var call = await TestClient.SendAsync((c, ct) => c.EstimateMessages.ListAsync(1439818, null, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/estimates/1439818/messages");
	}

	[Fact]
	public async Task ListAsync_AllOptions_SendsEveryParameter()
	{
		var call = await TestClient.SendAsync((c, ct) => c.EstimateMessages.ListAsync(1439818, new ListOptions
		{
			UpdatedSince = new DateTimeOffset(2017, 8, 25, 21, 0, 0, TimeSpan.Zero),
			Cursor = "abc",
			Page = 2,
			PerPage = 10
		}, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/estimates/1439818/messages",
			"?updated_since=2017-08-25T21%3A00%3A00Z&cursor=abc&page=2&per_page=10");
	}

	[Fact]
	public async Task ListAsync_MapsTheMessagesAndPaging()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.EstimateMessages.ListAsync(1439818, null, ct), $$$"""
			{"estimate_messages":[{{{MessageJson}}}],"per_page":2000,"total_pages":1,"total_entries":1,"next_page":null,
			"previous_page":null,"page":1,"links":{"first":"https://api.harvestapp.com/v2/estimates/1439818/messages?page=1&per_page=2000",
			"next":null,"previous":null,"last":"https://api.harvestapp.com/v2/estimates/1439818/messages?page=1&per_page=2000"}}
			""");

		ShouldBeTheMessage(page.EstimateMessages.Should().ContainSingle().Subject);
		page.TotalEntries.Should().Be(1);
	}

	[Fact]
	public async Task CreateAsync_DocsExample_SendsMessageAndMapsIt()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.Created, MessageJson);
		using var client = TestClient.Create(stub);

		var message = await client.EstimateMessages.CreateAsync(1439818, new EstimateMessageCreationDto
		{
			Subject = "Estimate #1001",
			Body = "Here is our estimate.",
			SendMeACopy = true,
			Recipients = [new MessageRecipientCreationDto { Name = "Richard Roe", Email = "richard@example.com" }]
		}, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Subject.ShouldBe(HttpMethod.Post, "/v2/estimates/1439818/messages", body: """
			{"recipients":[{"name":"Richard Roe","email":"richard@example.com"}],"subject":"Estimate #1001","body":"Here is our estimate.","send_me_a_copy":true}
			""");
		ShouldBeTheMessage(message);
	}

	[Fact]
	public async Task CreateAsync_AllFields_SendsEveryField()
	{
		var call = await TestClient.SendAsync((c, ct) => c.EstimateMessages.CreateAsync(5, new EstimateMessageCreationDto
		{
			Recipients =
			[
				new MessageRecipientCreationDto { Name = "A", Email = "a@example.com" },
				new MessageRecipientCreationDto { Email = "b@example.com" }
			],
			Subject = "S",
			Body = "B",
			SendMeACopy = false,
			EventType = EstimateMessageEventType.Send
		}, ct), MessageJson);

		call.ShouldBe(HttpMethod.Post, "/v2/estimates/5/messages", body: """
			{"recipients":[{"name":"A","email":"a@example.com"},{"email":"b@example.com"}],"subject":"S","body":"B","send_me_a_copy":false,"event_type":"send"}
			""");
	}

	[Fact]
	public async Task CreateAsync_Empty_SendsEmptyObject()
	{
		var call = await TestClient.SendAsync((c, ct) => c.EstimateMessages.CreateAsync(5, new EstimateMessageCreationDto(), ct), MessageJson);

		call.ShouldBe(HttpMethod.Post, "/v2/estimates/5/messages", body: "{}");
	}

	[Theory]
	[InlineData(EstimateMessageEventType.Send, "send")]
	[InlineData(EstimateMessageEventType.Accept, "accept")]
	[InlineData(EstimateMessageEventType.Decline, "decline")]
	[InlineData(EstimateMessageEventType.ReOpen, "re-open")]
	public async Task CreateAsync_EventOnly_SendsEventTypeAndMapsTheEventMessage(string eventType, string expected)
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.Created, EventMessageJson(expected));
		using var client = TestClient.Create(stub);

		var message = await client.EstimateMessages.CreateAsync(1439818, EstimateMessageCreationDto.ForEvent(eventType), TestClient.Ct);

		stub.Calls.Should().ContainSingle().Subject.ShouldBe(HttpMethod.Post, "/v2/estimates/1439818/messages",
			body: $$"""{"event_type":"{{expected}}"}""");
		message.Id.Should().Be(2666241);
		message.EventType.Should().Be(expected);
		message.SendMeACopy.Should().BeFalse();
		message.Recipients.Should().BeEmpty();
		message.Subject.Should().BeNull();
		message.Body.Should().BeNull();
	}

	[Theory]
	[InlineData(EstimateMessageEventType.View, "view")]
	[InlineData(EstimateMessageEventType.Invoice, "invoice")]
	public async Task ListAsync_ReadOnlyEventTypes_AreRead(string eventType, string wire)
	{
		eventType.Should().Be(wire);
		var page = await TestClient.ReadAsync((c, ct) => c.EstimateMessages.ListAsync(1, null, ct),
			$$"""{"estimate_messages":[{{EventMessageJson(wire)}}]}""");

		page.EstimateMessages.Should().ContainSingle().Which.EventType.Should().Be(eventType);
	}

	[Fact]
	public void ForEvent_SetsOnlyTheEventType()
	{
		var dto = EstimateMessageCreationDto.ForEvent(EstimateMessageEventType.Accept);

		dto.EventType.Should().Be("accept");
		dto.Recipients.Should().BeNull();
		dto.Subject.Should().BeNull();
		dto.Body.Should().BeNull();
		dto.SendMeACopy.Should().BeNull();
	}

	[Fact]
	public async Task DeleteAsync_SendsDelete()
	{
		var call = await TestClient.SendAsync((c, ct) => c.EstimateMessages.DeleteAsync(1439818, 2666240, ct), "");

		call.ShouldBe(HttpMethod.Delete, "/v2/estimates/1439818/messages/2666240");
	}

	private static void ShouldBeTheMessage(EstimateMessage message)
	{
		message.Id.Should().Be(2666236);
		message.SentBy.Should().Be("Bob Powell");
		message.SentByEmail.Should().Be("bob@example.com");
		message.SentFrom.Should().Be("Bob Powell");
		message.SentFromEmail.Should().Be("bob@example.com");
		message.SendMeACopy.Should().BeTrue();
		message.CreatedAt.Should().Be(new DateTime(2017, 8, 25, 21, 23, 40, DateTimeKind.Utc));
		message.UpdatedAt.Should().Be(new DateTime(2017, 8, 25, 21, 23, 41, DateTimeKind.Utc));
		message.Recipients.Should().HaveCount(2);
		message.Recipients[0].Name.Should().Be("Richard Roe");
		message.Recipients[0].Email.Should().Be("richard@example.com");
		message.Recipients[1].Name.Should().Be("Bob Powell");
		message.EventType.Should().BeNull();
		message.Subject.Should().Be("Estimate #1001 from API Examples");
		message.Body.Should().Be("Estimate Summary\r\nEstimate ID: 1001\r\n\r\n%estimate_url%\r\n\r\nThank you!");
	}
}