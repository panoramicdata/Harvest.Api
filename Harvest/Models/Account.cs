namespace Harvest.Models;

/// <summary>
/// A Harvest or Forecast account that an access token can reach, from Harvest ID's
/// <c>GET https://id.getharvest.com/api/v2/accounts</c>.
/// </summary>
public class Account
{
	/// <summary>The account ID: pass it as <see cref="HarvestClientOptions.AccountId"/> to work with a Harvest account.</summary>
	public long Id { get; set; }

	/// <summary>The name of the account.</summary>
	public string? Name { get; set; }

	/// <summary>The product the account belongs to: <c>harvest</c> or <c>forecast</c> (see <see cref="AccountProduct"/>).</summary>
	public string? Product { get; set; }
}