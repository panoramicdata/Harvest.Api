using Harvest.UnitTest.Support;

namespace Harvest.UnitTest.Resources;

public class PtoAllocationApiTests
{
	private const string AllocationJson = """
		{"id":780,"year":2026,"accrual_type":null,"accrual_schedule":null,"accrual_first_payday":null,"accrual_leave_hours":null,
		"accrual_worked_hours":null,"accrual_max_hours_per_year":null,"days_per_year":28.0,"carryover_days":2.0,
		"requires_approval":false,"effective_days_per_year":28.0,"effective_accrual_type":"fixed",
		"effective_accrual_schedule":"monthly","effective_requires_approval":false,"user":{"id":1782959,"name":"Kim Allen"},
		"pto_type":{"id":101,"name":"Vacation","color":"blue","icon":"vacation"},"created_at":"2026-09-03T12:00:00Z",
		"updated_at":"2026-09-03T12:00:00Z"}
		""";

	private const string ListJson = $$$"""
		{"pto_allocations":[{{{AllocationJson}}}],"per_page":2000,"total_pages":1,"total_entries":1,"next_page":null,
		"previous_page":null,"page":1,"links":{"first":"https://api.harvestapp.com/v2/pto/allocations?page=1&per_page=2000",
		"next":null,"previous":null,"last":"https://api.harvestapp.com/v2/pto/allocations?page=1&per_page=2000"}}
		""";

	private const string ResetJson = """
		{"user":{"id":1782959,"name":"Kim Allen"},"pto_type":{"id":101,"name":"Vacation","color":"blue","icon":"vacation"},
		"year":2026,"customized":false,"message":"Allocation reset to type defaults"}
		""";

	[Fact]
	public async Task ListAsync_NoOptions_SendsNoQuery()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoAllocations.ListAsync(null, ct), ListJson);

		call.ShouldBe(HttpMethod.Get, "/v2/pto/allocations");
	}

	[Fact]
	public async Task ListAsync_AllOptions_SendsEveryParameter()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoAllocations.ListAsync(new PtoAllocationListOptions
		{
			Year = 2026,
			UserIds = [1782959, 1782884],
			PtoTypeIds = [101],
			Page = 2,
			PerPage = 100
		}, ct), ListJson);

		call.ShouldBe(HttpMethod.Get, "/v2/pto/allocations",
			"?year=2026&user_ids%5B%5D=1782959&user_ids%5B%5D=1782884&pto_type_ids%5B%5D=101&page=2&per_page=100");
	}

	[Fact]
	public async Task ListAsync_MapsTheAllocations()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.PtoAllocations.ListAsync(null, ct), ListJson);

		page.TotalEntries.Should().Be(1);
		ShouldBeTheAllocation(page.PtoAllocations.Should().ContainSingle().Subject);
	}

	[Fact]
	public async Task ListAsync_MapsCustomizedAccrualOverrides()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.PtoAllocations.ListAsync(null, ct), """
			{"pto_allocations":[{"id":781,"accrual_type":"accrued","accrual_schedule":"hourly","accrual_first_payday":"2026-01-09",
			"accrual_leave_hours":1.5,"accrual_worked_hours":30.0,"accrual_max_hours_per_year":60.0,"requires_approval":true}]}
			""");

		var allocation = page.PtoAllocations.Should().ContainSingle().Subject;
		allocation.AccrualType.Should().Be(PtoAccrualType.Accrued);
		allocation.AccrualSchedule.Should().Be(PtoAccrualSchedule.Hourly);
		allocation.AccrualFirstPayday.Should().Be(new DateOnly(2026, 1, 9));
		allocation.AccrualLeaveHours.Should().Be(1.5m);
		allocation.AccrualWorkedHours.Should().Be(30m);
		allocation.AccrualMaxHoursPerYear.Should().Be(60m);
		allocation.RequiresApproval.Should().BeTrue();
		allocation.CreatedAt.Should().BeNull();
	}

	[Fact]
	public async Task UpdateAsync_RequiredOnly_SendsUserAndPolicy()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoAllocations.UpdateAsync(new PtoAllocationPatchDto
		{
			UserId = 1782959,
			PtoTypeId = 101
		}, ct), AllocationJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/pto/allocations", body: """{"user_id":1782959,"pto_type_id":101}""");
	}

	[Fact]
	public async Task UpdateAsync_AllFields_SendsEveryField()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoAllocations.UpdateAsync(new PtoAllocationPatchDto
		{
			UserId = 1782959,
			PtoTypeId = 101,
			Year = 2026,
			AccrualType = PtoAccrualType.Accrued,
			AccrualSchedule = PtoAccrualSchedule.Biweekly,
			AccrualFirstPayday = new DateOnly(2026, 1, 9),
			AccrualLeaveHours = 1.5m,
			AccrualWorkedHours = 30m,
			AccrualMaxHoursPerYear = 60m,
			DaysPerYear = 28m,
			CarryoverDays = 2m,
			RequiresApproval = false,
			ResetToDefault = false
		}, ct), AllocationJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/pto/allocations", body: """
			{"user_id":1782959,"pto_type_id":101,"year":2026,"days_per_year":28,"carryover_days":2,"requires_approval":false,"reset_to_default":false,"accrual_type":"accrued","accrual_schedule":"biweekly","accrual_first_payday":"2026-01-09","accrual_leave_hours":1.5,"accrual_worked_hours":30,"accrual_max_hours_per_year":60}
			""");
	}

	[Fact]
	public async Task UpdateAsync_MapsTheSavedAllocation()
	{
		var result = await TestClient.ReadAsync((c, ct) => c.PtoAllocations.UpdateAsync(new PtoAllocationPatchDto
		{
			UserId = 1782959,
			PtoTypeId = 101,
			Year = 2026,
			DaysPerYear = 28m
		}, ct), AllocationJson);

		ShouldBeTheAllocation(result);
		result.IsReset.Should().BeFalse();
		result.Customized.Should().BeNull();
		result.Message.Should().BeNull();
	}

	[Fact]
	public async Task UpdateAsync_ResetToDefault_SendsResetAndMapsTheConfirmation()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.OK, ResetJson);
		using var client = TestClient.Create(stub);

		var result = await client.PtoAllocations.UpdateAsync(new PtoAllocationPatchDto
		{
			UserId = 1782959,
			PtoTypeId = 101,
			Year = 2026,
			ResetToDefault = true
		}, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Subject.ShouldBe(HttpMethod.Patch, "/v2/pto/allocations",
			body: """{"user_id":1782959,"pto_type_id":101,"year":2026,"reset_to_default":true}""");
		result.IsReset.Should().BeTrue();
		result.Customized.Should().BeFalse();
		result.Message.Should().Be("Allocation reset to type defaults");
		result.Year.Should().Be(2026);
		result.User!.Id.Should().Be(1782959);
		result.PtoType!.Id.Should().Be(101);
		result.Id.Should().Be(0);
	}

	private static void ShouldBeTheAllocation(PtoAllocation allocation)
	{
		allocation.Id.Should().Be(780);
		allocation.Year.Should().Be(2026);
		allocation.AccrualType.Should().BeNull();
		allocation.AccrualSchedule.Should().BeNull();
		allocation.AccrualFirstPayday.Should().BeNull();
		allocation.AccrualLeaveHours.Should().BeNull();
		allocation.AccrualWorkedHours.Should().BeNull();
		allocation.AccrualMaxHoursPerYear.Should().BeNull();
		allocation.DaysPerYear.Should().Be(28m);
		allocation.CarryoverDays.Should().Be(2m);
		allocation.RequiresApproval.Should().BeFalse();
		allocation.EffectiveDaysPerYear.Should().Be(28m);
		allocation.EffectiveAccrualType.Should().Be(PtoAccrualType.Fixed);
		allocation.EffectiveAccrualSchedule.Should().Be(PtoAccrualSchedule.Monthly);
		allocation.EffectiveRequiresApproval.Should().BeFalse();
		allocation.User!.Id.Should().Be(1782959);
		allocation.User.Name.Should().Be("Kim Allen");
		allocation.PtoType!.Id.Should().Be(101);
		allocation.PtoType.Name.Should().Be("Vacation");
		allocation.CreatedAt.Should().Be(new DateTime(2026, 9, 3, 12, 0, 0, DateTimeKind.Utc));
		allocation.UpdatedAt.Should().Be(new DateTime(2026, 9, 3, 12, 0, 0, DateTimeKind.Utc));
	}
}