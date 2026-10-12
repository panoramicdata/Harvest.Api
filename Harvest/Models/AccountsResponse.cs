namespace Harvest.Models;

/// <summary>The response of <c>GET https://id.getharvest.com/api/v2/accounts</c>. It is not paged.</summary>
public class AccountsResponse
{
	/// <summary>The user behind the access token.</summary>
	public AccountUser? User { get; set; }

	/// <summary>The accounts the access token can reach (<c>accounts</c>).</summary>
	public List<Account> Accounts { get; set; } = [];
}