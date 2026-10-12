namespace Harvest.Models;

/// <summary>The query parameters of <c>GET /v2/pto/balances</c>.</summary>
public class PtoBalanceOptions
{
	/// <summary>
	/// The ID of the user (<c>user_id</c>); Harvest's default is the authenticated user. Non-administrators must leave it
	/// empty or pass their own ID.
	/// </summary>
	[AliasAs("user_id")]
	public long? UserId { get; init; }

	/// <summary>The year to calculate balances for (<c>year</c>); Harvest's default is the current year.</summary>
	[AliasAs("year")]
	public int? Year { get; init; }

	/// <summary>Calculate balances as of this date (<c>as_of</c>); Harvest's default is today.</summary>
	[AliasAs("as_of")]
	public DateOnly? AsOf { get; init; }
}