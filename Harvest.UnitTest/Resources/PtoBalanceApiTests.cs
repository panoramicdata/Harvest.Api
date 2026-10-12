using Harvest.UnitTest.Support;

namespace Harvest.UnitTest.Resources;

public class PtoBalanceApiTests
{
	private const string BalancesJson = """
		{"user":{"id":1782959,"name":"Kim Allen"},"year":2026,"as_of":"2026-09-03","balances":[{"pto_type":{"id":101,
		"name":"Vacation","color":"blue","icon":"vacation"},"is_requestable":true,"year":2026,"as_of":"2026-09-03",
		"is_unlimited":false,"requires_approval":true,"effective_accrual_type":"fixed","accrual_schedule":"monthly",
		"accrued_days":"25.0","carryover_days":"2.0","available_days":"27.0","used_days":"10.0","pending_days":"3.0",
		"remaining_days":"14.0","used_hours":"80.0","pending_hours":"24.0","used_fraction":"0.3704","pending_fraction":"0.1111"}]}
		""";

	[Fact]
	public async Task GetAsync_NoOptions_SendsNoQuery()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoBalances.GetAsync(null, ct), BalancesJson);

		call.ShouldBe(HttpMethod.Get, "/v2/pto/balances");
	}

	[Fact]
	public async Task GetAsync_AllOptions_SendsEveryParameter()
	{
		var call = await TestClient.SendAsync((c, ct) => c.PtoBalances.GetAsync(new PtoBalanceOptions
		{
			UserId = 1782959,
			Year = 2026,
			AsOf = new DateOnly(2026, 9, 3)
		}, ct), BalancesJson);

		call.ShouldBe(HttpMethod.Get, "/v2/pto/balances", "?user_id=1782959&year=2026&as_of=2026-09-03");
	}

	[Fact]
	public async Task GetAsync_MapsTheBalances()
	{
		var balances = await TestClient.ReadAsync((c, ct) => c.PtoBalances.GetAsync(null, ct), BalancesJson);

		balances.User!.Id.Should().Be(1782959);
		balances.User.Name.Should().Be("Kim Allen");
		balances.Year.Should().Be(2026);
		balances.AsOf.Should().Be(new DateOnly(2026, 9, 3));
		var balance = balances.Balances.Should().ContainSingle().Subject;
		balance.PtoType!.Id.Should().Be(101);
		balance.PtoType.Name.Should().Be("Vacation");
		balance.PtoType.Color.Should().Be("blue");
		balance.PtoType.Icon.Should().Be("vacation");
		balance.IsRequestable.Should().BeTrue();
		balance.Year.Should().Be(2026);
		balance.AsOf.Should().Be(new DateOnly(2026, 9, 3));
		balance.IsUnlimited.Should().BeFalse();
		balance.RequiresApproval.Should().BeTrue();
		balance.EffectiveAccrualType.Should().Be(PtoAccrualType.Fixed);
		balance.AccrualSchedule.Should().Be(PtoAccrualSchedule.Monthly);
		balance.AccruedDays.Should().Be(25m);
		balance.CarryoverDays.Should().Be(2m);
		balance.AvailableDays.Should().Be(27m);
		balance.UsedDays.Should().Be(10m);
		balance.PendingDays.Should().Be(3m);
		balance.RemainingDays.Should().Be(14m);
		balance.UsedHours.Should().Be(80m);
		balance.PendingHours.Should().Be(24m);
		balance.UsedFraction.Should().Be(0.3704m);
		balance.PendingFraction.Should().Be(0.1111m);
	}

	[Fact]
	public async Task GetAsync_ReadsNumericAmountsAndNulls()
	{
		var balances = await TestClient.ReadAsync((c, ct) => c.PtoBalances.GetAsync(null, ct), """
			{"balances":[{"is_unlimited":true,"accrued_days":25.5,"remaining_days":null}]}
			""");

		var balance = balances.Balances.Should().ContainSingle().Subject;
		balance.IsUnlimited.Should().BeTrue();
		balance.AccruedDays.Should().Be(25.5m);
		balance.RemainingDays.Should().BeNull();
		balance.PtoType.Should().BeNull();
		balances.User.Should().BeNull();
	}
}