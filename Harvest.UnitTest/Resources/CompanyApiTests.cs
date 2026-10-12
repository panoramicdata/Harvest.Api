using Harvest.UnitTest.Support;

namespace Harvest.UnitTest.Resources;

public class CompanyApiTests
{
	private const string CompanyJson = """
		{"base_uri":"https://acme.harvestapp.com","full_domain":"acme.harvestapp.com","name":"API Examples","is_active":true,
		"week_start_day":"Monday","wants_timestamp_timers":true,"time_format":"hours_minutes","date_format":"%Y-%m-%d",
		"plan_type":"sponsored","expense_feature":true,"invoice_feature":true,"estimate_feature":true,"approval_feature":true,
		"team_feature":true,"clock":"12h","currency_code_display":"iso_code_none","currency_symbol_display":"symbol_before",
		"decimal_symbol":".","thousands_separator":",","color_scheme":"orange","weekly_capacity":126000}
		""";

	[Fact]
	public async Task GetAsync_SendsGet()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Companies.GetAsync(ct), CompanyJson);

		call.ShouldBe(HttpMethod.Get, "/v2/company");
	}

	[Fact]
	public async Task GetAsync_MapsEveryField()
	{
		var company = await TestClient.ReadAsync((c, ct) => c.Companies.GetAsync(ct), CompanyJson);

		company.BaseUri.Should().Be("https://acme.harvestapp.com");
		company.FullDomain.Should().Be("acme.harvestapp.com");
		company.Name.Should().Be("API Examples");
		company.IsActive.Should().BeTrue();
		company.WeekStartDay.Should().Be(DayOfWeek.Monday);
		company.WantsTimestampTimers.Should().BeTrue();
		company.TimeFormat.Should().Be("hours_minutes");
		company.DateFormat.Should().Be("%Y-%m-%d");
		company.PlanType.Should().Be("sponsored");
		company.Clock.Should().Be("12h");
		company.CurrencyCodeDisplay.Should().Be("iso_code_none");
		company.CurrencySymbolDisplay.Should().Be("symbol_before");
		company.DecimalSymbol.Should().Be(".");
		company.ThousandsSeparator.Should().Be(",");
		company.ColorScheme.Should().Be("orange");
		company.WeeklyCapacity.Should().Be(126000);
		company.ExpenseFeature.Should().BeTrue();
		company.InvoiceFeature.Should().BeTrue();
		company.EstimateFeature.Should().BeTrue();
		company.ApprovalFeature.Should().BeTrue();
		company.TeamFeature.Should().BeTrue();
	}

	[Theory]
	[InlineData("Saturday", DayOfWeek.Saturday)]
	[InlineData("Sunday", DayOfWeek.Sunday)]
	public async Task GetAsync_ReadsEachWeekStartDay(string wire, DayOfWeek expected)
	{
		var company = await TestClient.ReadAsync((c, ct) => c.Companies.GetAsync(ct), $$"""{"week_start_day":"{{wire}}"}""");

		company.WeekStartDay.Should().Be(expected);
	}

	[Fact]
	public async Task UpdateAsync_AllFields_SendsPatchAndMapsTheCompany()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.OK, """
			{"base_uri":"https://{ACCOUNT_SUBDOMAIN}.harvestapp.com","full_domain":"{ACCOUNT_SUBDOMAIN}.harvestapp.com","name":"API Examples",
			"is_active":true,"week_start_day":"Monday","wants_timestamp_timers":false,"time_format":"hours_minutes","date_format":"%Y-%m-%d",
			"plan_type":"sponsored","expense_feature":true,"invoice_feature":true,"estimate_feature":true,"approval_feature":true,"clock":"12h",
			"currency_code_display":"iso_code_none","currency_symbol_display":"symbol_before","decimal_symbol":".","thousands_separator":",",
			"color_scheme":"orange","weekly_capacity": 108000}
			""");
		using var client = TestClient.Create(stub);

		var company = await client.Companies.UpdateAsync(new CompanyPatchDto { WeeklyCapacity = 108000, WantsTimestampTimers = false }, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Subject.ShouldBe(HttpMethod.Patch, "/v2/company",
			body: """{"wants_timestamp_timers":false,"weekly_capacity":108000}""");
		company.WantsTimestampTimers.Should().BeFalse();
		company.WeeklyCapacity.Should().Be(108000);
		company.Name.Should().Be("API Examples");
		company.TeamFeature.Should().BeFalse();
	}

	[Fact]
	public async Task UpdateAsync_OneField_SendsOnlyThatField()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Companies.UpdateAsync(new CompanyPatchDto { WantsTimestampTimers = true }, ct), CompanyJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/company", body: """{"wants_timestamp_timers":true}""");
	}

	[Fact]
	public async Task GetAsync_ToleratesMissingFields()
	{
		var company = await TestClient.ReadAsync((c, ct) => c.Companies.GetAsync(ct), "{}");

		company.WeekStartDay.Should().BeNull();
		company.WeeklyCapacity.Should().BeNull();
		company.Name.Should().BeNull();
	}
}