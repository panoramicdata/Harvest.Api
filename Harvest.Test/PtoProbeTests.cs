using System.Net;

namespace Harvest.Test;

public class PtoProbeTests(ITestOutputHelper testOutputHelper) : HarvestTest(testOutputHelper)
{
	[Fact]
	public async System.Threading.Tasks.Task EveryPtoListEndpointIsEitherAvailableOrForbidden()
	{
		var me = await HarvestClient.Users.GetMeAsync(CancellationToken);
		var year = DateTime.UtcNow.Year;
		var available = 0;

		available += await ProbeAsync("types", () => HarvestClient.PtoTypes.ListAsync(options: null, CancellationToken));
		available += await ProbeAsync("work schedules", () => HarvestClient.PtoWorkSchedules.ListAsync(options: null, CancellationToken));
		available += await ProbeAsync("holiday calendars", () => HarvestClient.PtoHolidayCalendars.ListAsync(options: null, CancellationToken));
		available += await ProbeAsync("requests", () => HarvestClient.PtoRequests.ListAsync(options: null, CancellationToken));
		available += await ProbeAsync("assignments", () => HarvestClient.PtoAssignments.ListAsync(new() { UserIds = [me.Id] }, CancellationToken));
		available += await ProbeAsync("allocations", () => HarvestClient.PtoAllocations.ListAsync(new() { Year = year, UserIds = [me.Id] }, CancellationToken));
		available += await ProbeAsync("balances", () => HarvestClient.PtoBalances.GetAsync(new() { UserId = me.Id, Year = year }, CancellationToken));

		Log.PtoSummary(Logger, available);
		(available is 0 or 7).Should().BeTrue("PTO is either on for the account or off for all of it");
	}

	/// <summary>Calls a PTO endpoint: 1 when it answers, 0 when Harvest says 403 (feature off), which must map to <see cref="HarvestApiException"/>.</summary>
	private async Task<int> ProbeAsync<T>(string name, Func<Task<T>> call)
	{
		try
		{
			var result = await call();
			result.Should().NotBeNull();
			Log.PtoAvailable(Logger, name);
			return 1;
		}
		catch (HarvestApiException ex)
		{
			ex.StatusCode.Should().Be(HttpStatusCode.Forbidden, $"PTO {name} may only be refused as a disabled feature");
			ex.Message.Should().NotBeNullOrEmpty();
			Log.PtoUnavailable(Logger, name, ex.Message);
			return 0;
		}
	}
}