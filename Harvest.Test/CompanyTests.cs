namespace Harvest.Test;

public class CompanyTests(ITestOutputHelper testOutputHelper) : HarvestTest(testOutputHelper)
{
	[Fact]
	public async System.Threading.Tasks.Task GetCompany()
	{
		var company = await HarvestClient.Companies.GetAsync(CancellationToken);
		company.Should().NotBeNull();
		company.Name.Should().NotBeNullOrEmpty();
		company.IsActive.Should().BeTrue();
	}
}
