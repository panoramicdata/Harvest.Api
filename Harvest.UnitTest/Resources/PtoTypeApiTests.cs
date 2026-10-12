using Harvest.UnitTest.Support;

namespace Harvest.UnitTest.Resources;

public class PtoTypeApiTests
{
	private const string PtoTypeJson = """
		{"id":101,"name":"Vacation","color":"blue","icon":"vacation","accrual_type":"fixed","accrual_schedule":"monthly",
		"default_days_per_year":25.0,"max_carryover_days":5.0,"requires_approval":true,"visible_to_all":true,"is_active":true,
		"created_at":"2026-01-01T00:00:00Z","updated_at":"2026-01-01T00:00:00Z"}
		""";

	private const string HourlyPtoTypeJson = """
		{"id":103,"name":"Sick Leave","accrual_type":"accrued","accrual_schedule":"biweekly","accrual_first_payday":"2026-01-09",
		"accrual_leave_hours":1.0,"accrual_worked_hours":30.0,"accrual_max_hours_per_year":48.0,"requires_approval":false,
		"visible_to_all":false,"is_active":false}
		""";

	private const string ListJson = $$$"""
		{"pto_types":[{{{PtoTypeJson}}}],"per_page":2000,"total_pages":1,"total_entries":1,"next_page":null,"previous_page":null,
		"page":1,"links":{"first":"https://api.harvestapp.com/v2/pto/types?page=1&per_page=2000","next":null,"previous":null,
		"last":"https://api.harvestapp.com/v2/pto/types?page=1&per_page=2000"}}
		""";

	[Fact]
	public async Task ListAsync_NoOptions_SendsNoQuery()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoTypes.ListAsync(null, ct), ListJson);

		call.ShouldBe(HttpMethod.Get, "/v2/pto/types");
	}

	[Fact]
	public async Task ListAsync_AllOptions_SendsEveryParameter()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoTypes.ListAsync(new ListOptions
		{
			IsActive = true,
			UpdatedSince = new DateTimeOffset(2026, 9, 1, 12, 0, 0, TimeSpan.Zero),
			Page = 3,
			PerPage = 100
		}, ct), ListJson);

		call.ShouldBe(HttpMethod.Get, "/v2/pto/types", "?is_active=true&updated_since=2026-09-01T12%3A00%3A00Z&page=3&per_page=100");
	}

	[Fact]
	public async Task ListAsync_MapsThePolicies()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.PtoTypes.ListAsync(null, ct), ListJson);

		page.TotalPages.Should().Be(1);
		ShouldBeTheVacationPolicy(page.PtoTypes.Should().ContainSingle().Subject);
	}

	[Fact]
	public async Task GetAsync_SendsGetAndMapsThePolicy()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.OK, PtoTypeJson);
		using var client = TestClient.Create(stub);

		var policy = await client.PtoTypes.GetAsync(101, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Subject.ShouldBe(HttpMethod.Get, "/v2/pto/types/101");
		ShouldBeTheVacationPolicy(policy);
	}

	[Fact]
	public async Task GetAsync_MapsTheHourlyAccrualFields()
	{
		var policy = await TestClient.ReadAsync((c, ct) => c.PtoTypes.GetAsync(103, ct), HourlyPtoTypeJson);

		policy.AccrualType.Should().Be(PtoAccrualType.Accrued);
		policy.AccrualSchedule.Should().Be(PtoAccrualSchedule.Biweekly);
		policy.AccrualFirstPayday.Should().Be(new DateOnly(2026, 1, 9));
		policy.AccrualLeaveHours.Should().Be(1m);
		policy.AccrualWorkedHours.Should().Be(30m);
		policy.AccrualMaxHoursPerYear.Should().Be(48m);
		policy.Color.Should().BeNull();
		policy.DefaultDaysPerYear.Should().BeNull();
		policy.RequiresApproval.Should().BeFalse();
		policy.VisibleToAll.Should().BeFalse();
		policy.IsActive.Should().BeFalse();
	}

	[Fact]
	public async Task CreateAsync_RequiredOnly_SendsName()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoTypes.CreateAsync(new PtoTypeCreationDto { Name = "Floating Holiday" }, ct), PtoTypeJson);

		call.ShouldBe(HttpMethod.Post, "/v2/pto/types", body: """{"name":"Floating Holiday"}""");
	}

	[Fact]
	public async Task CreateAsync_AllFields_SendsEveryField()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoTypes.CreateAsync(new PtoTypeCreationDto
		{
			Name = "Sick",
			Color = "orange",
			Icon = "calendar",
			AccrualType = PtoAccrualType.Accrued,
			AccrualSchedule = PtoAccrualSchedule.Hourly,
			AccrualFirstPayday = new DateOnly(2026, 1, 9),
			AccrualLeaveHours = 1m,
			AccrualWorkedHours = 30m,
			AccrualMaxHoursPerYear = 48m,
			DefaultDaysPerYear = 2.5m,
			MaxCarryoverDays = 0m,
			RequiresApproval = false,
			VisibleToAll = true
		}, ct), PtoTypeJson);

		call.ShouldBe(HttpMethod.Post, "/v2/pto/types", body: """
			{"name":"Sick","color":"orange","icon":"calendar","accrual_type":"accrued","accrual_schedule":"hourly","accrual_first_payday":"2026-01-09","accrual_leave_hours":1,"accrual_worked_hours":30,"accrual_max_hours_per_year":48,"default_days_per_year":2.5,"max_carryover_days":0,"requires_approval":false,"visible_to_all":true}
			""");
	}

	[Fact]
	public async Task UpdateAsync_AllFields_SendsPatch()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoTypes.UpdateAsync(101, new PtoTypePatchDto
		{
			Name = "Vacation",
			Color = "blue",
			Icon = "vacation",
			AccrualType = PtoAccrualType.Unlimited,
			AccrualSchedule = PtoAccrualSchedule.Semimonthly,
			AccrualFirstPayday = new DateOnly(2026, 2, 6),
			AccrualLeaveHours = 2m,
			AccrualWorkedHours = 40m,
			AccrualMaxHoursPerYear = 80m,
			DefaultDaysPerYear = 28m,
			MaxCarryoverDays = 5m,
			RequiresApproval = true,
			VisibleToAll = false,
			Deactivated = false
		}, ct), PtoTypeJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/pto/types/101", body: """
			{"name":"Vacation","color":"blue","icon":"vacation","accrual_type":"unlimited","accrual_schedule":"semimonthly","accrual_first_payday":"2026-02-06","accrual_leave_hours":2,"accrual_worked_hours":40,"accrual_max_hours_per_year":80,"default_days_per_year":28,"max_carryover_days":5,"requires_approval":true,"visible_to_all":false,"deactivated":false}
			""");
	}

	[Fact]
	public async Task UpdateAsync_EmptyPatch_SendsEmptyObject()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoTypes.UpdateAsync(101, new PtoTypePatchDto(), ct), PtoTypeJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/pto/types/101", body: "{}");
	}

	[Fact]
	public async Task DeactivateAsync_SendsDeleteAndMapsThePolicy()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.OK, """{"id":101,"name":"Vacation","is_active":false}""");
		using var client = TestClient.Create(stub);

		var policy = await client.PtoTypes.DeactivateAsync(101, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Subject.ShouldBe(HttpMethod.Delete, "/v2/pto/types/101");
		policy.Id.Should().Be(101);
		policy.IsActive.Should().BeFalse();
	}

	[Fact]
	public async Task DeleteAsync_SendsPermanentDeleteConfirmation()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoTypes.DeleteAsync(101, ct), "", HttpStatusCode.NoContent);

		call.ShouldBe(HttpMethod.Delete, "/v2/pto/types/101", "?keep_history=false&confirm_delete_history=1");
	}

	private static void ShouldBeTheVacationPolicy(PtoType policy)
	{
		policy.Id.Should().Be(101);
		policy.Name.Should().Be("Vacation");
		policy.Color.Should().Be("blue");
		policy.Icon.Should().Be("vacation");
		policy.AccrualType.Should().Be(PtoAccrualType.Fixed);
		policy.AccrualSchedule.Should().Be(PtoAccrualSchedule.Monthly);
		policy.AccrualFirstPayday.Should().BeNull();
		policy.AccrualLeaveHours.Should().BeNull();
		policy.AccrualWorkedHours.Should().BeNull();
		policy.AccrualMaxHoursPerYear.Should().BeNull();
		policy.DefaultDaysPerYear.Should().Be(25m);
		policy.MaxCarryoverDays.Should().Be(5m);
		policy.RequiresApproval.Should().BeTrue();
		policy.VisibleToAll.Should().BeTrue();
		policy.IsActive.Should().BeTrue();
		policy.CreatedAt.Should().Be(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
		policy.UpdatedAt.Should().Be(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
	}
}
