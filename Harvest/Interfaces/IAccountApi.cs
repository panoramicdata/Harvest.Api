namespace Harvest.Interfaces;

/// <summary>
/// The Harvest and Forecast accounts that the access token can reach, from Harvest ID
/// (<see href="https://help.getharvest.com/api-v2/authentication-api/authentication/authentication/"/>, "Scopes and account
/// access"). These requests go to <see cref="HarvestClient.IdBaseAddress"/> rather than the Harvest API, and do not send
/// the <c>Harvest-Account-Id</c> header.
/// </summary>
public interface IAccountApi
{
	/// <summary>Lists the user behind the access token and every account it can access.</summary>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The user and their accounts.</returns>
	[Get("/api/v2/accounts")]
	Task<AccountsResponse> ListAsync(CancellationToken cancellationToken);
}