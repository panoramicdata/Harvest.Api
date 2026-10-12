namespace Harvest.Interfaces;

/// <summary>
/// PTO time off balances (<see href="https://help.getharvest.com/api-v2/pto-api/pto/time-off-balances/"/>).
/// Administrators can retrieve any user's balances; other users only their own.
/// </summary>
/// <remarks>
/// The PTO API is being rolled out gradually: an account without it receives <c>403 Forbidden</c> (a
/// <see cref="HarvestApiException"/>) from every call.
/// </remarks>
public interface IPtoBalanceApi
{
	/// <summary>Gets a user's time off balances for a year, broken down per time off policy.</summary>
	/// <param name="options">
	/// The user, year and evaluation date, or <see langword="null"/> for the authenticated user's balances this year as of
	/// today.
	/// </param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The balances.</returns>
	[Get("/v2/pto/balances")]
	Task<PtoBalances> GetAsync([Query] PtoBalanceOptions? options, CancellationToken cancellationToken);
}