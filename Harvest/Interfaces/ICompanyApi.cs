namespace Harvest.Interfaces;

/// <summary>The company of the authenticated user (<see href="https://help.getharvest.com/api-v2/company-api/company/company/"/>).</summary>
public interface ICompanyApi
{
	/// <summary>Gets the company of the authenticated user.</summary>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The company.</returns>
	[Get("/v2/company")]
	Task<Company> GetAsync(CancellationToken cancellationToken);

	/// <summary>Updates the company's settings; fields left <see langword="null"/> are unchanged.</summary>
	/// <param name="patchDto">The fields to change.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The updated company.</returns>
	[Patch("/v2/company")]
	Task<Company> UpdateAsync([Body] CompanyPatchDto patchDto, CancellationToken cancellationToken);
}