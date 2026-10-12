using Harvest.Interfaces;
using Harvest.UnitTest.Support;
using System.Reflection;

namespace Harvest.UnitTest.Core;

public class HarvestClientTests
{
	[Fact]
	public async Task EveryRequest_CarriesAuthenticationAccountAndJsonHeaders()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Companies.GetAsync(ct));

		call.Uri.GetLeftPart(UriPartial.Authority).Should().Be("https://api.harvestapp.com");
		call.Headers.Authorization!.Scheme.Should().Be("Bearer");
		call.Headers.Authorization.Parameter.Should().Be(TestClient.AccessToken);
		call.Headers.GetValues("Harvest-Account-Id").Should().Equal("12345");
		call.Headers.Accept.Should().ContainSingle().Which.MediaType.Should().Be("application/json");
	}

	[Fact]
	public async Task UserAgent_DefaultsToTheLibraryNameVersionAndRepository()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Companies.GetAsync(ct));

		var expectedVersion = typeof(HarvestClient).Assembly.GetName().Version!.ToString(3);
		HarvestClient.DefaultUserAgent.Should().Be($"Harvest.Api/{expectedVersion} (+https://github.com/panoramicdata/Harvest.Api)");
		string.Join(" ", call.Headers.GetValues("User-Agent")).Should().Be(HarvestClient.DefaultUserAgent);
	}

	[Fact]
	public async Task UserAgent_CanBeOverriddenWithHarvestsAppAndContactForm()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.OK);
		var options = new HarvestClientOptions { AccountId = 1, AccessToken = "t", UserAgent = "MyApp (ops@example.com)" };
		using var client = new HarvestClient(options, stub);

		await client.Companies.GetAsync(TestClient.Ct);

		string.Join(" ", stub.Calls[0].Headers.GetValues("User-Agent")).Should().Be("MyApp (ops@example.com)");
	}

	[Fact]
	public void Constructor_NullOptions_Throws()
	{
		var act = () => new HarvestClient(null!);

		act.Should().Throw<ArgumentNullException>().WithParameterName("options");
	}

	[Fact]
	public void Constructor_NullHandler_Throws()
	{
		var act = () => new HarvestClient(TestClient.Options(), null!);

		act.Should().Throw<ArgumentNullException>().WithParameterName("innerHandler");
	}

	[Fact]
	public void Constructor_InvalidOptions_ThrowsBeforeCreatingATransport()
	{
		var act = () => new HarvestClient(new HarvestClientOptions { AccountId = 1, AccessToken = "" });

		act.Should().Throw<ArgumentException>().WithParameterName("AccessToken");
	}

	[Fact]
	public void Constructor_WithoutHandler_CreatesANetworkClient()
	{
		using var client = new HarvestClient(TestClient.Options());

	}

	[Fact]
	public void CreateTransport_WithoutProxy_UsesNone()
	{
		using var handler = HarvestClient.CreateTransport(TestClient.Options());

		handler.Proxy.Should().BeNull();
	}

	[Fact]
	public void CreateTransport_WithProxy_RoutesThroughIt()
	{
		var options = new HarvestClientOptions { AccountId = 1, AccessToken = "t", ProxyUrl = "http://proxy.example.com:8080" };

		using var handler = HarvestClient.CreateTransport(options);

		var proxy = handler.Proxy.Should().BeOfType<WebProxy>().Subject;
		proxy.Address.Should().Be(new Uri("http://proxy.example.com:8080"));
		proxy.BypassProxyOnLocal.Should().BeFalse();
		proxy.UseDefaultCredentials.Should().BeFalse();
	}

	[Fact]
	public async Task Dispose_DisposesTheTransport()
	{
		var stub = new StubHandler();
		var client = TestClient.Create(stub);

		client.Dispose();

		var act = () => client.Companies.GetAsync(TestClient.Ct);
		await act.Should().ThrowAsync<ObjectDisposedException>();
	}

	[Fact]
	public void HarvestClient_ExposesEveryApiThroughIHarvestClient()
	{
		// IHarvestClient must offer every resource API that HarvestClient has, so callers can depend on the interface.
		static IEnumerable<string> ApiProperties(Type type) => type
			.GetProperties(BindingFlags.Public | BindingFlags.Instance)
			.Where(p => p.PropertyType.IsInterface && p.PropertyType.Name.EndsWith("Api", StringComparison.Ordinal))
			.Select(p => $"{p.PropertyType.Name} {p.Name}");

		ApiProperties(typeof(IHarvestClient)).Should().BeEquivalentTo(ApiProperties(typeof(HarvestClient)));
		ApiProperties(typeof(IHarvestClient)).Should().HaveCount(32);
	}

	[Fact]
	public void HarvestClient_PopulatesEveryApi()
	{
		using var client = TestClient.Create(new StubHandler());

		client.Should().BeAssignableTo<IHarvestClient>();
		client.Accounts.Should().NotBeNull();
		client.BillableRates.Should().NotBeNull();
		client.Clients.Should().NotBeNull();
		client.Companies.Should().NotBeNull();
		client.Contacts.Should().NotBeNull();
		client.CostRates.Should().NotBeNull();
		client.EstimateItemCategories.Should().NotBeNull();
		client.EstimateMessages.Should().NotBeNull();
		client.Estimates.Should().NotBeNull();
		client.ExpenseCategories.Should().NotBeNull();
		client.Expenses.Should().NotBeNull();
		client.InvoiceItemCategories.Should().NotBeNull();
		client.InvoiceMessages.Should().NotBeNull();
		client.InvoicePayments.Should().NotBeNull();
		client.Invoices.Should().NotBeNull();
		client.ProjectTaskAssignments.Should().NotBeNull();
		client.ProjectUserAssignments.Should().NotBeNull();
		client.Projects.Should().NotBeNull();
		client.PtoAllocations.Should().NotBeNull();
		client.PtoAssignments.Should().NotBeNull();
		client.PtoBalances.Should().NotBeNull();
		client.PtoHolidayCalendars.Should().NotBeNull();
		client.PtoRequests.Should().NotBeNull();
		client.PtoTypes.Should().NotBeNull();
		client.PtoWorkSchedules.Should().NotBeNull();
		client.Reports.Should().NotBeNull();
		client.Roles.Should().NotBeNull();
		client.Tasks.Should().NotBeNull();
		client.Teammates.Should().NotBeNull();
		client.TimeEntries.Should().NotBeNull();
		client.UserProjectAssignments.Should().NotBeNull();
		client.Users.Should().NotBeNull();
	}

	[Fact]
	public void EveryApiMethod_TakesACancellationTokenLast()
	{
		var methods = typeof(IHarvestClient).Assembly.GetTypes()
			.Where(t => t.IsInterface && t.Namespace == "Harvest.Interfaces" && t.Name.EndsWith("Api", StringComparison.Ordinal))
			.SelectMany(t => t.GetMethods())
			.ToList();

		methods.Should().NotBeEmpty();
		methods.Should().AllSatisfy(m => m.GetParameters()[^1].ParameterType.Should().Be<CancellationToken>(m.Name));
	}
}