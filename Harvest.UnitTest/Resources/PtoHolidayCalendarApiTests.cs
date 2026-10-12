using Harvest.UnitTest.Support;

namespace Harvest.UnitTest.Resources;

public class PtoHolidayCalendarApiTests
{
	private const string CalendarJson = """
		{"id":12,"name":"US Holidays","entries_count":1,"entries":[{"id":44,"name":"New Year's Day","date":"2026-01-01"}],
		"created_at":"2026-01-01T00:00:00Z","updated_at":"2026-01-01T00:00:00Z"}
		""";

	private const string CalendarsJson = $$$"""
		{"pto_holiday_calendars":[{{{CalendarJson}}}],"per_page":2000,"total_pages":1,"total_entries":1,"next_page":null,
		"previous_page":null,"page":1,"links":{"first":"https://api.harvestapp.com/v2/pto/holiday_calendars?page=1&per_page=2000",
		"next":null,"previous":null,"last":"https://api.harvestapp.com/v2/pto/holiday_calendars?page=1&per_page=2000"}}
		""";

	private const string EntryJson = """
		{"id":45,"pto_holiday_calendar_id":12,"name":"Memorial Day","date":"2026-05-25","created_at":"2026-09-03T12:00:00Z",
		"updated_at":"2026-09-03T12:05:00Z"}
		""";

	private const string EntriesJson = $$$"""
		{"pto_holiday_calendar_entries":[{{{EntryJson}}}],"per_page":2000,"total_pages":1,"total_entries":1,"next_page":null,
		"previous_page":null,"page":1,"links":{"first":"https://api.harvestapp.com/v2/pto/holiday_calendars/12/entries?page=1&per_page=2000",
		"next":null,"previous":null,"last":"https://api.harvestapp.com/v2/pto/holiday_calendars/12/entries?page=1&per_page=2000"}}
		""";

	[Fact]
	public async Task ListAsync_NoOptions_SendsNoQuery()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoHolidayCalendars.ListAsync(null, ct), CalendarsJson);

		call.ShouldBe(HttpMethod.Get, "/v2/pto/holiday_calendars");
	}

	[Fact]
	public async Task ListAsync_Paging_SendsPageAndPerPage()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoHolidayCalendars.ListAsync(new ListOptions { Page = 2, PerPage = 25 }, ct), CalendarsJson);

		call.ShouldBe(HttpMethod.Get, "/v2/pto/holiday_calendars", "?page=2&per_page=25");
	}

	[Fact]
	public async Task ListAsync_MapsTheCalendars()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.PtoHolidayCalendars.ListAsync(null, ct), CalendarsJson);

		page.TotalEntries.Should().Be(1);
		ShouldBeTheUsCalendar(page.PtoHolidayCalendars.Should().ContainSingle().Subject);
	}

	[Fact]
	public async Task GetAsync_SendsGetAndMapsTheCalendar()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.OK, CalendarJson);
		using var client = TestClient.Create(stub);

		var calendar = await client.PtoHolidayCalendars.GetAsync(12, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Subject.ShouldBe(HttpMethod.Get, "/v2/pto/holiday_calendars/12");
		ShouldBeTheUsCalendar(calendar);
	}

	[Fact]
	public async Task CreateAsync_RequiredOnly_SendsName()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoHolidayCalendars.CreateAsync(new PtoHolidayCalendarCreationDto { Name = "UK Holidays" }, ct), CalendarJson);

		call.ShouldBe(HttpMethod.Post, "/v2/pto/holiday_calendars", body: """{"name":"UK Holidays"}""");
	}

	[Fact]
	public async Task CreateAsync_AllFields_SendsCountryCode()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoHolidayCalendars.CreateAsync(new PtoHolidayCalendarCreationDto
		{
			Name = "UK Holidays",
			CountryCode = "GB"
		}, ct), CalendarJson);

		call.ShouldBe(HttpMethod.Post, "/v2/pto/holiday_calendars", body: """{"name":"UK Holidays","country_code":"GB"}""");
	}

	[Fact]
	public async Task UpdateAsync_SendsName()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoHolidayCalendars.UpdateAsync(13, new PtoHolidayCalendarPatchDto { Name = "UK Office Holidays" }, ct), CalendarJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/pto/holiday_calendars/13", body: """{"name":"UK Office Holidays"}""");
	}

	[Fact]
	public async Task DeleteAsync_SendsDeleteAndMapsTheCalendar()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.OK, CalendarJson);
		using var client = TestClient.Create(stub);

		var calendar = await client.PtoHolidayCalendars.DeleteAsync(12, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Subject.ShouldBe(HttpMethod.Delete, "/v2/pto/holiday_calendars/12");
		calendar.Id.Should().Be(12);
	}

	[Fact]
	public async Task ListEntriesAsync_NoOptions_SendsNoQuery()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoHolidayCalendars.ListEntriesAsync(12, null, ct), EntriesJson);

		call.ShouldBe(HttpMethod.Get, "/v2/pto/holiday_calendars/12/entries");
	}

	[Fact]
	public async Task ListEntriesAsync_Paging_SendsPageAndPerPage()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoHolidayCalendars.ListEntriesAsync(12, new ListOptions { Page = 1, PerPage = 2000 }, ct), EntriesJson);

		call.ShouldBe(HttpMethod.Get, "/v2/pto/holiday_calendars/12/entries", "?page=1&per_page=2000");
	}

	[Fact]
	public async Task ListEntriesAsync_MapsTheEntries()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.PtoHolidayCalendars.ListEntriesAsync(12, null, ct), """
			{"pto_holiday_calendar_entries":[{"id":44,"pto_holiday_calendar_id":12,"name":"New Year's Day","date":"2026-01-01",
			"created_at":"2026-01-01T00:00:00Z","updated_at":"2026-01-01T00:00:00Z"}],"per_page":2000,"total_pages":1,
			"total_entries":1,"next_page":null,"previous_page":null,"page":1}
			""");

		var entry = page.PtoHolidayCalendarEntries.Should().ContainSingle().Subject;
		entry.Id.Should().Be(44);
		entry.PtoHolidayCalendarId.Should().Be(12);
		entry.Name.Should().Be("New Year's Day");
		entry.Date.Should().Be(new DateOnly(2026, 1, 1));
		entry.CreatedAt.Should().Be(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
		entry.UpdatedAt.Should().Be(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
	}

	[Fact]
	public async Task CreateEntryAsync_SingleDate_SendsNameAndDate()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoHolidayCalendars.CreateEntryAsync(12, new PtoHolidayCalendarEntryCreationDto
		{
			Name = "Memorial Day",
			Date = new DateOnly(2026, 5, 25)
		}, ct), EntryJson);

		call.ShouldBe(HttpMethod.Post, "/v2/pto/holiday_calendars/12/entries", body: """{"name":"Memorial Day","date":"2026-05-25"}""");
	}

	[Fact]
	public async Task CreateEntryAsync_RequiredOnly_SendsName()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoHolidayCalendars.CreateEntryAsync(12, new PtoHolidayCalendarEntryCreationDto { Name = "Closure" }, ct), EntryJson);

		call.ShouldBe(HttpMethod.Post, "/v2/pto/holiday_calendars/12/entries", body: """{"name":"Closure"}""");
	}

	[Fact]
	public async Task CreateEntryAsync_AllFields_SendsEveryField()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoHolidayCalendars.CreateEntryAsync(12, new PtoHolidayCalendarEntryCreationDto
		{
			Name = "Winter Closure",
			Date = new DateOnly(2026, 12, 24),
			DateMode = PtoHolidayCalendarEntryCreationDto.RangeDateMode,
			StartDate = new DateOnly(2026, 12, 24),
			EndDate = new DateOnly(2026, 12, 31)
		}, ct), EntryJson);

		call.ShouldBe(HttpMethod.Post, "/v2/pto/holiday_calendars/12/entries", body: """
			{"name":"Winter Closure","date":"2026-12-24","date_mode":"range","start_date":"2026-12-24","end_date":"2026-12-31"}
			""");
	}

	[Fact]
	public async Task CreateEntryAsync_MapsTheEntry()
	{
		var entry = await TestClient.ReadAsync((c, ct) => c.PtoHolidayCalendars.CreateEntryAsync(12, new PtoHolidayCalendarEntryCreationDto { Name = "Memorial Day" }, ct), EntryJson);

		entry.Id.Should().Be(45);
		entry.PtoHolidayCalendarId.Should().Be(12);
		entry.Name.Should().Be("Memorial Day");
		entry.Date.Should().Be(new DateOnly(2026, 5, 25));
		entry.CreatedAt.Should().Be(new DateTime(2026, 9, 3, 12, 0, 0, DateTimeKind.Utc));
		entry.UpdatedAt.Should().Be(new DateTime(2026, 9, 3, 12, 5, 0, DateTimeKind.Utc));
	}

	[Fact]
	public async Task UpdateEntryAsync_AllFields_SendsPatch()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoHolidayCalendars.UpdateEntryAsync(12, 45, new PtoHolidayCalendarEntryPatchDto
		{
			Name = "Memorial Day Observed",
			Date = new DateOnly(2026, 5, 26)
		}, ct), EntryJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/pto/holiday_calendars/12/entries/45", body: """{"name":"Memorial Day Observed","date":"2026-05-26"}""");
	}

	[Fact]
	public async Task UpdateEntryAsync_EmptyPatch_SendsEmptyObject()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoHolidayCalendars.UpdateEntryAsync(12, 45, new PtoHolidayCalendarEntryPatchDto(), ct), EntryJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/pto/holiday_calendars/12/entries/45", body: "{}");
	}

	[Fact]
	public async Task DeleteEntryAsync_SendsDelete()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoHolidayCalendars.DeleteEntryAsync(12, 45, ct), EntryJson);

		call.ShouldBe(HttpMethod.Delete, "/v2/pto/holiday_calendars/12/entries/45");
	}

	private static void ShouldBeTheUsCalendar(PtoHolidayCalendar calendar)
	{
		calendar.Id.Should().Be(12);
		calendar.Name.Should().Be("US Holidays");
		calendar.EntriesCount.Should().Be(1);
		calendar.CreatedAt.Should().Be(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
		calendar.UpdatedAt.Should().Be(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
		var entry = calendar.Entries.Should().ContainSingle().Subject;
		entry.Id.Should().Be(44);
		entry.Name.Should().Be("New Year's Day");
		entry.Date.Should().Be(new DateOnly(2026, 1, 1));
		entry.PtoHolidayCalendarId.Should().BeNull();
		entry.CreatedAt.Should().BeNull();
	}
}