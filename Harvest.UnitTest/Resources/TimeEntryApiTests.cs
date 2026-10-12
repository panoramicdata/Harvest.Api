using Harvest.UnitTest.Support;

namespace Harvest.UnitTest.Resources;

public class TimeEntryApiTests
{
	private const string TimeEntryJson = """
		{"id":636708723,"spent_date":"2017-03-01","user":{"id":1782959,"name":"Kim Allen"},"client":{"id":5735776,"name":"123 Industries"},
		"project":{"id":14308069,"name":"Online Store - Phase 1"},"task":{"id":8083366,"name":"Programming"},
		"user_assignment":{"id":125068553,"is_project_manager":true,"is_active":true,"budget":null,"created_at":"2017-06-26T22:32:52Z","updated_at":"2017-06-26T22:32:52Z","hourly_rate":100.0},
		"task_assignment":{"id":155502709,"billable":true,"is_active":true,"created_at":"2017-06-26T21:36:23Z","updated_at":"2017-06-26T21:36:23Z","hourly_rate":100.0,"budget":null},
		"external_reference":{"id":"1234","group_id":"5678","account_id":"91011","permalink":"https://github.com/x/y/issues/1","service":"github.com","service_icon_url":"https://proxy.harvestfiles.com/github.png"},
		"invoice":{"id":13150403,"number":"1001"},
		"hours":1.35,"hours_without_timer":1.25,"rounded_hours":1.5,"notes":"Importing products","is_locked":true,"locked_reason":"Item Approved and Locked for this Time Period",
		"approval_status":"approved","is_closed":true,"is_billed":true,"timer_started_at":"2017-06-27T16:40:00Z","started_time":"1:00pm","ended_time":"2:00pm",
		"is_running":false,"billable":true,"budgeted":true,"billable_rate":100.0,"cost_rate":50.0,"created_at":"2017-06-27T15:49:28Z","updated_at":"2017-06-27T16:47:14Z"}
		""";

	[Fact]
	public async Task ListAsync_NoOptions_SendsNoQuery()
	{
		var call = await TestClient.SendAsync((c, ct) => c.TimeEntries.ListAsync(null, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/time_entries");
	}

	[Fact]
	public async Task ListAsync_AllFilters_SendsEveryParameter()
	{
		var call = await TestClient.SendAsync((c, ct) => c.TimeEntries.ListAsync(new TimeEntryListOptions
		{
			UserId = 1,
			ClientId = 2,
			ProjectId = 3,
			TaskId = 4,
			ExternalReferenceId = "ext 5",
			IsBilled = false,
			IsRunning = true,
			ApprovalStatus = ApprovalStatus.Submitted,
			From = new DateOnly(2025, 1, 13),
			To = new DateOnly(2025, 1, 19),
			UpdatedSince = new DateTimeOffset(2025, 1, 1, 9, 30, 0, TimeSpan.FromHours(1)),
			Cursor = "c1",
			PerPage = 100
		}, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/time_entries",
			"?user_id=1&client_id=2&project_id=3&task_id=4&external_reference_id=ext%205&is_billed=false&is_running=true&approval_status=submitted&from=2025-01-13&to=2025-01-19&updated_since=2025-01-01T08%3A30%3A00Z&cursor=c1&per_page=100");
	}

	[Fact]
	public async Task ListAsync_MapsTheTimeEntries()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.TimeEntries.ListAsync(null, ct), $$$"""{"time_entries":[{{{TimeEntryJson}}}]}""");

		ShouldBeTheTimeEntry(page.TimeEntries.Should().ContainSingle().Subject);
	}

	[Fact]
	public async Task GetAsync_SendsGetAndMapsTheTimeEntry()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.OK, TimeEntryJson);
		using var client = TestClient.Create(stub);

		var entry = await client.TimeEntries.GetAsync(636708723, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Subject.ShouldBe(HttpMethod.Get, "/v2/time_entries/636708723");
		ShouldBeTheTimeEntry(entry);
	}

	[Fact]
	public async Task GetAsync_NumericExternalReferenceIds_AreReadAsText()
	{
		var entry = await TestClient.ReadAsync(
			(c, ct) => c.TimeEntries.GetAsync(1, ct),
			"""{"id":1,"spent_date":"2017-03-01","external_reference":{"id":1234,"group_id":56.5,"account_id":null}}""");

		entry.ExternalReference!.Id.Should().Be("1234");
		entry.ExternalReference.GroupId.Should().Be("56.5");
		entry.ExternalReference.AccountId.Should().BeNull();
	}

	[Fact]
	public async Task GetAsync_TimesInAnUnknownFormat_ReadAsNullTimeSpans()
	{
		var entry = await TestClient.ReadAsync(
			(c, ct) => c.TimeEntries.GetAsync(1, ct),
			"""{"id":1,"spent_date":"2017-03-01","started_time":"noon","ended_time":null}""");

		entry.StartedTime.Should().Be("noon");
		entry.StartedTimeSpan.Should().BeNull();
		entry.EndedTimeSpan.Should().BeNull();
	}

	[Fact]
	public async Task CreateAsync_Duration_SendsRequiredFieldsAndHours()
	{
		var call = await TestClient.SendAsync((c, ct) => c.TimeEntries.CreateAsync(new TimeEntryCreationDto
		{
			ProjectId = 14308069,
			TaskId = 8083365,
			SpentDate = new DateOnly(2017, 3, 21),
			Hours = 1.0m
		}, ct), TimeEntryJson);

		call.ShouldBe(HttpMethod.Post, "/v2/time_entries",
			body: """{"project_id":14308069,"task_id":8083365,"spent_date":"2017-03-21","hours":1.0}""");
	}

	[Fact]
	public async Task CreateAsync_AllFields_SendsEveryFieldWithTheExternalReference()
	{
		var call = await TestClient.SendAsync((c, ct) => c.TimeEntries.CreateAsync(new TimeEntryCreationDto
		{
			UserId = 1782959,
			ProjectId = 14308069,
			TaskId = 8083365,
			SpentDate = new DateOnly(2017, 3, 21),
			StartedTime = "8:00am",
			EndedTime = "9:00am",
			Hours = 1m,
			Notes = "n",
			ExternalReference = new ExternalReferenceCreationDto
			{
				Id = "1234",
				GroupId = "5678",
				AccountId = "91011",
				Permalink = "https://github.com/x/y/issues/1"
			}
		}, ct), TimeEntryJson);

		call.ShouldBe(HttpMethod.Post, "/v2/time_entries", body: """
			{"user_id":1782959,"project_id":14308069,"task_id":8083365,"spent_date":"2017-03-21","started_time":"8:00am","ended_time":"9:00am","hours":1,"notes":"n","external_reference":{"id":"1234","group_id":"5678","account_id":"91011","permalink":"https://github.com/x/y/issues/1"}}
			""");
	}

	[Fact]
	public async Task UpdateAsync_AllFields_SendsPatchWithTheCreationExternalReference()
	{
		var call = await TestClient.SendAsync((c, ct) => c.TimeEntries.UpdateAsync(636708723, new TimeEntryPatchDto
		{
			ProjectId = 1,
			TaskId = 2,
			SpentDate = new DateOnly(2017, 3, 22),
			StartedTime = "13:00",
			EndedTime = "14:30",
			Hours = 1.5m,
			Notes = "Updated",
			ExternalReference = new ExternalReferenceCreationDto { Id = "9", Permalink = "https://example.com/9" }
		}, ct), TimeEntryJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/time_entries/636708723", body: """
			{"project_id":1,"task_id":2,"spent_date":"2017-03-22","started_time":"13:00","ended_time":"14:30","hours":1.5,"notes":"Updated","external_reference":{"id":"9","permalink":"https://example.com/9"}}
			""");
	}

	[Fact]
	public async Task UpdateAsync_NotesOnly_SendsOnlyNotes()
	{
		var call = await TestClient.SendAsync((c, ct) => c.TimeEntries.UpdateAsync(5, new TimeEntryPatchDto { Notes = "x" }, ct), TimeEntryJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/time_entries/5", body: """{"notes":"x"}""");
	}

	[Fact]
	public async Task DeleteAsync_SendsDelete()
	{
		var call = await TestClient.SendAsync((c, ct) => c.TimeEntries.DeleteAsync(5, ct), "");

		call.ShouldBe(HttpMethod.Delete, "/v2/time_entries/5");
	}

	[Fact]
	public async Task DeleteExternalReferenceAsync_SendsDeleteToTheExternalReference()
	{
		var call = await TestClient.SendAsync((c, ct) => c.TimeEntries.DeleteExternalReferenceAsync(636718192, ct), "");

		call.ShouldBe(HttpMethod.Delete, "/v2/time_entries/636718192/external_reference");
	}

	[Fact]
	public async Task RestartAsync_SendsPatchWithNoBodyAndMapsTheRunningEntry()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.OK, """
			{"id":662204379,"spent_date":"2017-03-21","user":{"id":1795925,"name":"Jane Smith"},"client":{"id":5735776,"name":"123 Industries"},
			"project":{"id":14808188,"name":"Task Force"},"task":{"id":8083366,"name":"Programming"},
			"user_assignment":{"id":130403296,"is_project_manager":true,"is_active":true,"budget":null,"created_at":"2017-08-22T17:36:54Z","updated_at":"2017-08-22T17:36:54Z","hourly_rate":100},
			"task_assignment":{"id":160726645,"billable":true,"is_active":true,"created_at":"2017-08-22T17:36:54Z","updated_at":"2017-08-22T17:36:54Z","hourly_rate":100,"budget":null},
			"hours":0,"hours_without_timer":0,"rounded_hours":0,"notes":null,"created_at":"2017-08-22T17:40:24Z","updated_at":"2017-08-22T17:40:24Z",
			"is_locked":false,"locked_reason":null,"is_closed":false,"approval_status":"unsubmitted","is_billed":false,"timer_started_at":"2017-08-22T17:40:24Z",
			"started_time":"11:40am","ended_time":null,"is_running":true,"invoice":null,"external_reference":null,"billable":true,"budgeted":false,
			"billable_rate":100,"cost_rate":75}
			""");
		using var client = TestClient.Create(stub);

		var entry = await client.TimeEntries.RestartAsync(662202797, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Subject.ShouldBe(HttpMethod.Patch, "/v2/time_entries/662202797/restart");
		entry.Id.Should().Be(662204379);
		entry.IsRunning.Should().BeTrue();
		entry.TimerStartedAt.Should().Be(new DateTime(2017, 8, 22, 17, 40, 24, DateTimeKind.Utc));
		entry.StartedTime.Should().Be("11:40am");
		entry.EndedTime.Should().BeNull();
		entry.Hours.Should().Be(0m);
		entry.ApprovalStatus.Should().Be(ApprovalStatus.Unsubmitted);
		entry.ExternalReference.Should().BeNull();
		entry.Invoice.Should().BeNull();
		entry.CostRate.Should().Be(75m);
	}

	[Fact]
	public async Task StopAsync_SendsPatchWithNoBodyAndMapsTheStoppedEntry()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.OK, """
			{"id":662202797,"spent_date":"2017-03-21","user":{"id":1795925,"name":"Jane Smith"},"client":{"id":5735776,"name":"123 Industries"},
			"project":{"id":14808188,"name":"Task Force"},"task":{"id":8083366,"name":"Programming"},
			"hours":0.02,"hours_without_timer":0.02,"rounded_hours":0.25,"notes":null,"created_at":"2017-08-22T17:37:13Z","updated_at":"2017-08-22T17:38:31Z",
			"is_locked":false,"locked_reason":null,"is_closed":false,"approval_status":"unsubmitted","is_billed":false,"timer_started_at":null,
			"started_time":"11:37am","ended_time":"11:38am","is_running":false,"invoice":null,"external_reference":null,"billable":true,"budgeted":false,
			"billable_rate":100,"cost_rate":75}
			""");
		using var client = TestClient.Create(stub);

		var entry = await client.TimeEntries.StopAsync(662202797, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Subject.ShouldBe(HttpMethod.Patch, "/v2/time_entries/662202797/stop");
		entry.Id.Should().Be(662202797);
		entry.IsRunning.Should().BeFalse();
		entry.TimerStartedAt.Should().BeNull();
		entry.Hours.Should().Be(0.02m);
		entry.RoundedHours.Should().Be(0.25m);
		entry.EndedTime.Should().Be("11:38am");
		entry.EndedTimeSpan.Should().Be(new TimeSpan(11, 38, 0));
	}

	[Fact]
	public void TimeEntry_SerializesWithoutTheComputedTimeSpans()
	{
		// The computed properties are [JsonIgnore]: serializing a time entry (to cache it, say) must never throw on them.
		var json = System.Text.Json.JsonSerializer.Serialize(
			new TimeEntry { Id = 1, StartedTime = "not a time", EndedTime = "2:00pm" },
			HarvestClient.JsonOptions);

		json.Should().Contain("\"started_time\":\"not a time\"").And.NotContain("time_span");
	}

	private static void ShouldBeTheTimeEntry(TimeEntry entry)
	{
		entry.Id.Should().Be(636708723);
		entry.SpentDate.Should().Be(new DateOnly(2017, 3, 1));
		entry.User!.Id.Should().Be(1782959);
		entry.User.Name.Should().Be("Kim Allen");
		entry.Client!.Id.Should().Be(5735776);
		entry.Client.Name.Should().Be("123 Industries");
		entry.Project!.Id.Should().Be(14308069);
		entry.Project.Name.Should().Be("Online Store - Phase 1");
		entry.Task!.Id.Should().Be(8083366);
		entry.Task.Name.Should().Be("Programming");
		entry.UserAssignment!.Id.Should().Be(125068553);
		entry.UserAssignment.IsProjectManager.Should().BeTrue();
		entry.UserAssignment.IsActive.Should().BeTrue();
		entry.UserAssignment.HourlyRate.Should().Be(100m);
		entry.UserAssignment.Project.Should().BeNull();
		entry.TaskAssignment!.Id.Should().Be(155502709);
		entry.TaskAssignment.Billable.Should().BeTrue();
		entry.TaskAssignment.Task.Should().BeNull();
		entry.ExternalReference!.Id.Should().Be("1234");
		entry.ExternalReference.GroupId.Should().Be("5678");
		entry.ExternalReference.AccountId.Should().Be("91011");
		entry.ExternalReference.Permalink.Should().Be("https://github.com/x/y/issues/1");
		entry.ExternalReference.Service.Should().Be("github.com");
		entry.ExternalReference.ServiceIconUrl.Should().Be("https://proxy.harvestfiles.com/github.png");
		entry.Invoice!.Id.Should().Be(13150403);
		entry.Invoice.Number.Should().Be("1001");
		entry.Hours.Should().Be(1.35m);
		entry.HoursWithoutTimer.Should().Be(1.25m);
		entry.RoundedHours.Should().Be(1.5m);
		entry.Notes.Should().Be("Importing products");
		entry.IsLocked.Should().BeTrue();
		entry.LockedReason.Should().Be("Item Approved and Locked for this Time Period");
		entry.ApprovalStatus.Should().Be(ApprovalStatus.Approved);
		entry.IsClosed.Should().BeTrue();
		entry.IsBilled.Should().BeTrue();
		entry.TimerStartedAt.Should().Be(new DateTime(2017, 6, 27, 16, 40, 0, DateTimeKind.Utc));
		entry.StartedTime.Should().Be("1:00pm");
		entry.StartedTimeSpan.Should().Be(new TimeSpan(13, 0, 0));
		entry.EndedTime.Should().Be("2:00pm");
		entry.EndedTimeSpan.Should().Be(new TimeSpan(14, 0, 0));
		entry.IsRunning.Should().BeFalse();
		entry.Billable.Should().BeTrue();
		entry.Budgeted.Should().BeTrue();
		entry.BillableRate.Should().Be(100m);
		entry.CostRate.Should().Be(50m);
		entry.CreatedAt.Should().Be(new DateTime(2017, 6, 27, 15, 49, 28, DateTimeKind.Utc));
		entry.UpdatedAt.Should().Be(new DateTime(2017, 6, 27, 16, 47, 14, DateTimeKind.Utc));
	}
}