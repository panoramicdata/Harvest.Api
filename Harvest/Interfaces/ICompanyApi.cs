namespace Harvest.Interfaces;

public interface ICompanyApi
{
	[Get("/v2/company")]
	Task<Company> GetAsync(CancellationToken cancellationToken);
}
