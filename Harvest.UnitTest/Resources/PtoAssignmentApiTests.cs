using Harvest.UnitTest.Support;

namespace Harvest.UnitTest.Resources;

public class PtoAssignmentApiTests
{
	private const string AssignmentJson = """
		{"user":{"id":1782959,"name":"Kim Allen"},"holiday_calendar":{"id":12,"name":"US Holidays"},
		"work_schedule":{"id":8,"name":"Standard Full Time"},"effective_work_schedule":{"name":"Standard Full Time","weekly_hours":40.0}}
		""";

	[Fact]
	public async Task ListAsync_NoOptions_SendsNoQuery()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoAssignments.ListAsync(null, ct), $$"""{"assignments":[{{AssignmentJson}}]}""");

		call.ShouldBe(HttpMethod.Get, "/v2/pto/assignments");
	}

	[Fact]
	public async Task ListAsync_AllOptions_SendsUserIdAndRepeatedUserIds()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoAssignments.ListAsync(new PtoAssignmentListOptions
		{
			UserId = 1782959,
			UserIds = [1782959, 1782884]
		}, ct), """{"assignments":[]}""");

		call.ShouldBe(HttpMethod.Get, "/v2/pto/assignments", "?user_id=1782959&user_ids%5B%5D=1782959&user_ids%5B%5D=1782884");
	}

	[Fact]
	public async Task ListAsync_MapsTheAssignments()
	{
		var container = await TestClient.ReadAsync((c, ct) => c.PtoAssignments.ListAsync(null, ct), $$"""{"assignments":[{{AssignmentJson}}]}""");

		ShouldBeTheAssignment(container.Assignments.Should().ContainSingle().Subject);
	}

	[Fact]
	public async Task ListAsync_MapsUnassignedUsers()
	{
		var container = await TestClient.ReadAsync((c, ct) => c.PtoAssignments.ListAsync(null, ct), """
			{"assignments":[{"user":{"id":1782884,"name":"Bob Powell"},"holiday_calendar":null,"work_schedule":null,
			"effective_work_schedule":{"name":"Standard Full Time","weekly_hours":40.0}}]}
			""");

		var assignment = container.Assignments.Should().ContainSingle().Subject;
		assignment.HolidayCalendar.Should().BeNull();
		assignment.WorkSchedule.Should().BeNull();
		assignment.EffectiveWorkSchedule!.WeeklyHours.Should().Be(40m);
	}

	[Fact]
	public async Task UpdateAsync_Assign_SendsBothIds()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoAssignments.UpdateAsync(1782959, new PtoAssignmentPatchDto
		{
			HolidayCalendarId = 12,
			WorkScheduleId = 8
		}, ct), AssignmentJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/pto/assignments/1782959", body: """{"holiday_calendar_id":12,"work_schedule_id":8}""");
	}

	[Fact]
	public async Task UpdateAsync_Unassign_SendsExplicitNulls()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoAssignments.UpdateAsync(1782959, new PtoAssignmentPatchDto
		{
			HolidayCalendarId = 12,
			UnassignHolidayCalendar = true,
			UnassignWorkSchedule = true
		}, ct), AssignmentJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/pto/assignments/1782959", body: """{"holiday_calendar_id":null,"work_schedule_id":null}""");
	}

	[Fact]
	public async Task UpdateAsync_EmptyPatch_SendsEmptyObject()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoAssignments.UpdateAsync(1782959, new PtoAssignmentPatchDto(), ct), AssignmentJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/pto/assignments/1782959", body: "{}");
	}

	[Fact]
	public async Task UpdateAsync_MapsTheAssignment()
	{
		var assignment = await TestClient.ReadAsync((c, ct) => c.PtoAssignments.UpdateAsync(1782959, new PtoAssignmentPatchDto(), ct), AssignmentJson);

		ShouldBeTheAssignment(assignment);
	}

	private static void ShouldBeTheAssignment(PtoAssignment assignment)
	{
		assignment.User!.Id.Should().Be(1782959);
		assignment.User.Name.Should().Be("Kim Allen");
		assignment.HolidayCalendar!.Id.Should().Be(12);
		assignment.HolidayCalendar.Name.Should().Be("US Holidays");
		assignment.WorkSchedule!.Id.Should().Be(8);
		assignment.WorkSchedule.Name.Should().Be("Standard Full Time");
		assignment.EffectiveWorkSchedule!.Name.Should().Be("Standard Full Time");
		assignment.EffectiveWorkSchedule.WeeklyHours.Should().Be(40m);
	}
}