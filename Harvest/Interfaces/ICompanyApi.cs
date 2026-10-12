namespace Harvest.Interfaces;

/// <summary>The company of the authenticated user (<see href="https://help.getharvest.com/api-v2/company-api/company/company/"/>).</summary>
public interface ICompanyApi
{
	/// <summary>Gets the company of the authenticated user.</summary>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The company.</returns>
	[Get("/v2/company")]
	Task<Company> GetAsync(CancellationToken cancellationToken);
}