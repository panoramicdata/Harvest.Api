using Harvest.UnitTest.Support;

namespace Harvest.UnitTest.Resources;

public class PtoWorkScheduleApiTests
{
	private const string WorkScheduleJson = """
		{"id":8,"name":"Standard Full Time","is_default":true,"is_active":true,"weekly_hours":40.0,"monday_hours":8.0,
		"tuesday_hours":8.0,"wednesday_hours":8.0,"thursday_hours":8.0,"friday_hours":8.0,"saturday_hours":0.0,
		"sunday_hours":0.0,"created_at":"2026-01-01T00:00:00Z","updated_at":"2026-01-01T00:00:00Z"}
		""";

	private const string ListJson = $$$"""
		{"pto_work_schedules":[{{{WorkScheduleJson}}}],"per_page":2000,"total_pages":1,"total_entries":1,"next_page":null,
		"previous_page":null,"page":1,"links":{"first":"https://api.harvestapp.com/v2/pto/work_schedules?page=1&per_page=2000",
		"next":null,"previous":null,"last":"https://api.harvestapp.com/v2/pto/work_schedules?page=1&per_page=2000"}}
		""";

	[Fact]
	public async Task ListAsync_NoOptions_SendsNoQuery()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoWorkSchedules.ListAsync(null, ct), ListJson);

		call.ShouldBe(HttpMethod.Get, "/v2/pto/work_schedules");
	}

	[Fact]
	public async Task ListAsync_AllOptions_SendsEveryParameter()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoWorkSchedules.ListAsync(new ListOptions
		{
			IsActive = false,
			Page = 2,
			PerPage = 50
		}, ct), ListJson);

		call.ShouldBe(HttpMethod.Get, "/v2/pto/work_schedules", "?is_active=false&page=2&per_page=50");
	}

	[Fact]
	public async Task ListAsync_MapsTheWorkSchedules()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.PtoWorkSchedules.ListAsync(null, ct), ListJson);

		page.PerPage.Should().Be(2000);
		page.TotalEntries.Should().Be(1);
		page.Links!.First.Should().Be("https://api.harvestapp.com/v2/pto/work_schedules?page=1&per_page=2000");
		ShouldBeTheWorkSchedule(page.PtoWorkSchedules.Should().ContainSingle().Subject);
	}

	[Fact]
	public async Task GetAsync_SendsGetAndMapsTheWorkSchedule()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.OK, WorkScheduleJson);
		using var client = TestClient.Create(stub);

		var schedule = await client.PtoWorkSchedules.GetAsync(8, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Subject.ShouldBe(HttpMethod.Get, "/v2/pto/work_schedules/8");
		ShouldBeTheWorkSchedule(schedule);
	}

	[Fact]
	public async Task CreateAsync_RequiredOnly_SendsName()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoWorkSchedules.CreateAsync(new PtoWorkScheduleCreationDto
		{
			Name = "Part Time 20h"
		}, ct), WorkScheduleJson);

		call.ShouldBe(HttpMethod.Post, "/v2/pto/work_schedules", body: """{"name":"Part Time 20h"}""");
	}

	[Fact]
	public async Task CreateAsync_AllFields_SendsEveryField()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoWorkSchedules.CreateAsync(new PtoWorkScheduleCreationDto
		{
			Name = "Part Time 20h",
			IsDefault = false,
			MondayHours = 4.0m,
			TuesdayHours = 4.5m,
			WednesdayHours = 3m,
			ThursdayHours = 2m,
			FridayHours = 1m,
			SaturdayHours = 0m,
			SundayHours = 0.5m
		}, ct), WorkScheduleJson);

		call.ShouldBe(HttpMethod.Post, "/v2/pto/work_schedules", body: """
			{"name":"Part Time 20h","is_default":false,"monday_hours":4.0,"tuesday_hours":4.5,"wednesday_hours":3,"thursday_hours":2,"friday_hours":1,"saturday_hours":0,"sunday_hours":0.5}
			""");
	}

	[Fact]
	public async Task UpdateAsync_AllFields_SendsPatch()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoWorkSchedules.UpdateAsync(9, new PtoWorkSchedulePatchDto
		{
			Name = "Part Time 24h",
			IsDefault = true,
			MondayHours = 1m,
			TuesdayHours = 2m,
			WednesdayHours = 3m,
			ThursdayHours = 4m,
			FridayHours = 8m,
			SaturdayHours = 5m,
			SundayHours = 6m
		}, ct), WorkScheduleJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/pto/work_schedules/9", body: """
			{"name":"Part Time 24h","is_default":true,"monday_hours":1,"tuesday_hours":2,"wednesday_hours":3,"thursday_hours":4,"friday_hours":8,"saturday_hours":5,"sunday_hours":6}
			""");
	}

	[Fact]
	public async Task UpdateAsync_EmptyPatch_SendsEmptyObject()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoWorkSchedules.UpdateAsync(9, new PtoWorkSchedulePatchDto(), ct), WorkScheduleJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/pto/work_schedules/9", body: "{}");
	}

	[Fact]
	public async Task DeactivateAsync_SendsDeleteAndMapsTheDeactivatedSchedule()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.OK, """{"id":9,"name":"Part Time 24h","is_default":false,"is_active":false}""");
		using var client = TestClient.Create(stub);

		var schedule = await client.PtoWorkSchedules.DeactivateAsync(9, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Subject.ShouldBe(HttpMethod.Delete, "/v2/pto/work_schedules/9");
		schedule.Id.Should().Be(9);
		schedule.IsActive.Should().BeFalse();
		schedule.WeeklyHours.Should().BeNull();
	}

	private static void ShouldBeTheWorkSchedule(PtoWorkSchedule schedule)
	{
		schedule.Id.Should().Be(8);
		schedule.Name.Should().Be("Standard Full Time");
		schedule.IsDefault.Should().BeTrue();
		schedule.IsActive.Should().BeTrue();
		schedule.WeeklyHours.Should().Be(40m);
		schedule.MondayHours.Should().Be(8m);
		schedule.TuesdayHours.Should().Be(8m);
		schedule.WednesdayHours.Should().Be(8m);
		schedule.ThursdayHours.Should().Be(8m);
		schedule.FridayHours.Should().Be(8m);
		schedule.SaturdayHours.Should().Be(0m);
		schedule.SundayHours.Should().Be(0m);
		schedule.CreatedAt.Should().Be(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
		schedule.UpdatedAt.Should().Be(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
	}
}