using Harvest.UnitTest.Support;

namespace Harvest.UnitTest.Resources;

public class PtoRequestApiTests
{
	private const string RequestJson = """
		{"id":501,"status":"pending","start_date":"2026-10-01","end_date":"2026-10-02","days":2.0,"business_days_at_submission":2.0,
		"start_time_minutes":null,"end_time_minutes":null,"notes":"Fall break","review_reason":null,"reviewed_at":null,
		"created_at":"2026-09-01T12:00:00Z","updated_at":"2026-09-01T12:00:00Z","user":{"id":1782959,"name":"Kim Allen"},
		"pto_type":{"id":101,"name":"Vacation","color":"blue","icon":"vacation"},"reviewer":null,"request_days":[
		{"id":1001,"date":"2026-10-01","hours":8.0,"day_fraction":1.0,"created_at":"2026-09-01T12:00:00Z","updated_at":"2026-09-01T12:00:00Z"},
		{"id":1002,"date":"2026-10-02","hours":8.0,"day_fraction":1.0,"created_at":"2026-09-01T12:00:00Z","updated_at":"2026-09-01T12:00:00Z"}]}
		""";

	private const string DeclinedJson = """
		{"id":501,"status":"declined","start_date":"2026-10-01","end_date":"2026-10-02","days":2.0,"business_days_at_submission":2.0,
		"start_time_minutes":540,"end_time_minutes":780,"notes":"Fall break","review_reason":"Team coverage needed during that week",
		"reviewed_at":"2026-09-03T12:30:00Z","created_at":"2026-09-01T12:00:00Z","updated_at":"2026-09-03T12:30:00Z",
		"user":{"id":1782959,"name":"Kim Allen"},"pto_type":{"id":101,"name":"Vacation","color":"blue","icon":"vacation"},
		"reviewer":{"id":1782884,"name":"Bob Powell"},"request_days":[]}
		""";

	private const string ListJson = $$$"""
		{"pto_requests":[{{{RequestJson}}}],"per_page":2000,"total_pages":1,"total_entries":1,"next_page":null,"previous_page":null,
		"page":1,"links":{"first":"https://api.harvestapp.com/v2/pto/requests?page=1&per_page=2000","next":null,"previous":null,
		"last":"https://api.harvestapp.com/v2/pto/requests?page=1&per_page=2000"}}
		""";

	[Fact]
	public async Task ListAsync_NoOptions_SendsNoQuery()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoRequests.ListAsync(null, ct), ListJson);

		call.ShouldBe(HttpMethod.Get, "/v2/pto/requests");
	}

	[Fact]
	public async Task ListAsync_AllOptions_SendsEveryParameter()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoRequests.ListAsync(new PtoRequestListOptions
		{
			UserId = 1782959,
			Status = PtoRequestStatus.ChangesRequested,
			PtoTypeId = 101,
			From = new DateOnly(2026, 10, 1),
			To = new DateOnly(2026, 10, 31),
			UpdatedSince = new DateTimeOffset(2026, 9, 1, 12, 0, 0, TimeSpan.Zero),
			Page = 2,
			PerPage = 10
		}, ct), ListJson);

		call.ShouldBe(HttpMethod.Get, "/v2/pto/requests",
			"?user_id=1782959&status=changes_requested&pto_type_id=101&from=2026-10-01&to=2026-10-31&updated_since=2026-09-01T12%3A00%3A00Z&page=2&per_page=10");
	}

	[Fact]
	public async Task ListAsync_MapsTheRequests()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.PtoRequests.ListAsync(null, ct), ListJson);

		page.TotalEntries.Should().Be(1);
		ShouldBeThePendingRequest(page.PtoRequests.Should().ContainSingle().Subject);
	}

	[Fact]
	public async Task GetAsync_SendsGetAndMapsTheRequest()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.OK, RequestJson);
		using var client = TestClient.Create(stub);

		var request = await client.PtoRequests.GetAsync(501, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Subject.ShouldBe(HttpMethod.Get, "/v2/pto/requests/501");
		ShouldBeThePendingRequest(request);
	}

	[Fact]
	public async Task CreateAsync_RequiredOnly_SendsRequiredFields()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoRequests.CreateAsync(new PtoRequestCreationDto
		{
			PtoTypeId = 101,
			StartDate = new DateOnly(2026, 11, 5),
			EndDate = new DateOnly(2026, 11, 6)
		}, ct), RequestJson);

		call.ShouldBe(HttpMethod.Post, "/v2/pto/requests",
			body: """{"pto_type_id":101,"start_date":"2026-11-05","end_date":"2026-11-06"}""");
	}

	[Fact]
	public async Task CreateAsync_AllFields_SendsEveryField()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoRequests.CreateAsync(new PtoRequestCreationDto
		{
			PtoTypeId = 101,
			StartDate = new DateOnly(2026, 11, 5),
			EndDate = new DateOnly(2026, 11, 5),
			UserId = 1782959,
			Notes = "Doctor appointment",
			StartTimeMinutes = 540,
			EndTimeMinutes = 780,
			ConflictResolution = PtoConflictResolution.Replace
		}, ct), RequestJson);

		call.ShouldBe(HttpMethod.Post, "/v2/pto/requests", body: """
			{"pto_type_id":101,"start_date":"2026-11-05","end_date":"2026-11-05","user_id":1782959,"notes":"Doctor appointment","start_time_minutes":540,"end_time_minutes":780,"conflict_resolution":"replace"}
			""");
	}

	[Fact]
	public async Task UpdateAsync_AllFields_SendsPatch()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoRequests.UpdateAsync(503, new PtoRequestPatchDto
		{
			PtoTypeId = 102,
			StartDate = new DateOnly(2026, 11, 6),
			EndDate = new DateOnly(2026, 11, 7),
			Notes = "Updated note",
			StartTimeMinutes = 600,
			EndTimeMinutes = 720,
			ConflictResolution = PtoConflictResolution.Skip
		}, ct), RequestJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/pto/requests/503", body: """
			{"pto_type_id":102,"start_date":"2026-11-06","end_date":"2026-11-07","notes":"Updated note","start_time_minutes":600,"end_time_minutes":720,"conflict_resolution":"skip"}
			""");
	}

	[Fact]
	public async Task UpdateAsync_EmptyPatch_SendsEmptyObject()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoRequests.UpdateAsync(503, new PtoRequestPatchDto(), ct), RequestJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/pto/requests/503", body: "{}");
	}

	[Fact]
	public async Task CancelAsync_SendsDelete()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoRequests.CancelAsync(503, ct), RequestJson);

		call.ShouldBe(HttpMethod.Delete, "/v2/pto/requests/503");
	}

	[Fact]
	public async Task ApproveAsync_SendsPostWithoutBody()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoRequests.ApproveAsync(501, ct), RequestJson);

		call.ShouldBe(HttpMethod.Post, "/v2/pto/requests/501/approve");
	}

	[Fact]
	public async Task DeclineAsync_WithReason_SendsReason()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoRequests.DeclineAsync(501,
			new PtoRequestDeclineDto { Reason = "Team coverage needed during that week" }, ct), DeclinedJson);

		call.ShouldBe(HttpMethod.Post, "/v2/pto/requests/501/decline", body: """{"reason":"Team coverage needed during that week"}""");
	}

	[Fact]
	public async Task DeclineAsync_WithoutReason_SendsEmptyObject()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoRequests.DeclineAsync(501, new PtoRequestDeclineDto(), ct), DeclinedJson);

		call.ShouldBe(HttpMethod.Post, "/v2/pto/requests/501/decline", body: "{}");
	}

	[Fact]
	public async Task DeclineAsync_MapsTheReviewedRequest()
	{
		var request = await TestClient.ReadAsync((c, ct) => c.PtoRequests.DeclineAsync(501, new PtoRequestDeclineDto(), ct), DeclinedJson);

		request.Status.Should().Be(PtoRequestStatus.Declined);
		request.StartTimeMinutes.Should().Be(540);
		request.EndTimeMinutes.Should().Be(780);
		request.ReviewReason.Should().Be("Team coverage needed during that week");
		request.ReviewedAt.Should().Be(new DateTime(2026, 9, 3, 12, 30, 0, DateTimeKind.Utc));
		request.Reviewer!.Id.Should().Be(1782884);
		request.Reviewer.Name.Should().Be("Bob Powell");
		request.RequestDays.Should().BeEmpty();
	}

	[Fact]
	public async Task RequestChangesAsync_SendsReason()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoRequests.RequestChangesAsync(501,
			new PtoRequestChangesDto { Reason = "Please adjust dates to start on Friday" }, ct), RequestJson);

		call.ShouldBe(HttpMethod.Post, "/v2/pto/requests/501/request_changes",
			body: """{"reason":"Please adjust dates to start on Friday"}""");
	}

	private static void ShouldBeThePendingRequest(PtoRequest request)
	{
		request.Id.Should().Be(501);
		request.Status.Should().Be(PtoRequestStatus.Pending);
		request.StartDate.Should().Be(new DateOnly(2026, 10, 1));
		request.EndDate.Should().Be(new DateOnly(2026, 10, 2));
		request.Days.Should().Be(2m);
		request.BusinessDaysAtSubmission.Should().Be(2m);
		request.StartTimeMinutes.Should().BeNull();
		request.EndTimeMinutes.Should().BeNull();
		request.Notes.Should().Be("Fall break");
		request.ReviewReason.Should().BeNull();
		request.ReviewedAt.Should().BeNull();
		request.CreatedAt.Should().Be(new DateTime(2026, 9, 1, 12, 0, 0, DateTimeKind.Utc));
		request.UpdatedAt.Should().Be(new DateTime(2026, 9, 1, 12, 0, 0, DateTimeKind.Utc));
		request.User!.Id.Should().Be(1782959);
		request.User.Name.Should().Be("Kim Allen");
		request.PtoType!.Id.Should().Be(101);
		request.PtoType.Name.Should().Be("Vacation");
		request.PtoType.Color.Should().Be("blue");
		request.PtoType.Icon.Should().Be("vacation");
		request.Reviewer.Should().BeNull();
		request.RequestDays.Should().HaveCount(2);
		var day = request.RequestDays![0];
		day.Id.Should().Be(1001);
		day.Date.Should().Be(new DateOnly(2026, 10, 1));
		day.Hours.Should().Be(8m);
		day.DayFraction.Should().Be(1m);
		day.CreatedAt.Should().Be(new DateTime(2026, 9, 1, 12, 0, 0, DateTimeKind.Utc));
		day.UpdatedAt.Should().Be(new DateTime(2026, 9, 1, 12, 0, 0, DateTimeKind.Utc));
		request.RequestDays[1].Id.Should().Be(1002);
	}
}