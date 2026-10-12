using System.Text.Json.Serialization;

namespace Harvest.Models;

/// <summary>
/// The response of <c>GET /v2/pto/balances</c>: a user's time off balances for a year, one per time off policy
/// (<see href="https://help.getharvest.com/api-v2/pto-api/pto/time-off-balances/"/>).
/// </summary>
public class PtoBalances
{
	/// <summary>The user: its id and name.</summary>
	public UserReference? User { get; set; }

	/// <summary>The reporting year evaluated.</summary>
	public int? Year { get; set; }

	/// <summary>The evaluation date of the balances.</summary>
	public DateOnly? AsOf { get; set; }

	/// <summary>The balances, one per time off policy (<c>balances</c>).</summary>
	public List<PtoBalance> Balances { get; set; } = [];
}

/// <summary>
/// A user's balance for one time off policy. Harvest sends the day and hour amounts as JSON strings (<c>"25.0"</c>); they
/// are read as numbers either way.
/// </summary>
public class PtoBalance
{
	/// <summary>The time off policy: its id, name, color and icon.</summary>
	public PtoTypeReference? PtoType { get; set; }

	/// <summary>Whether the policy is currently active and requestable by the user.</summary>
	public bool IsRequestable { get; set; }

	/// <summary>The reporting year evaluated for the balance.</summary>
	public int? Year { get; set; }

	/// <summary>The evaluation date of the balance.</summary>
	public DateOnly? AsOf { get; set; }

	/// <summary>Whether this policy has unlimited allowance.</summary>
	public bool IsUnlimited { get; set; }

	/// <summary>Whether requests booked under this policy require approval.</summary>
	public bool RequiresApproval { get; set; }

	/// <summary>Effective accrual mode for this user and policy (see <see cref="PtoAccrualType"/>).</summary>
	public string? EffectiveAccrualType { get; set; }

	/// <summary>Effective accrual interval (see <see cref="PtoAccrualSchedule"/>).</summary>
	public string? AccrualSchedule { get; set; }

	/// <summary>Days accrued as of the balance date.</summary>
	[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
	public decimal? AccruedDays { get; set; }

	/// <summary>Carryover days credited from the previous year.</summary>
	[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
	public decimal? CarryoverDays { get; set; }

	/// <summary>Total days available to take for the year.</summary>
	[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
	public decimal? AvailableDays { get; set; }

	/// <summary>Confirmed days already taken.</summary>
	[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
	public decimal? UsedDays { get; set; }

	/// <summary>Days requested that are pending approval.</summary>
	[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
	public decimal? PendingDays { get; set; }

	/// <summary>Available days minus used and pending days.</summary>
	[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
	public decimal? RemainingDays { get; set; }

	/// <summary>Working hours already used.</summary>
	[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
	public decimal? UsedHours { get; set; }

	/// <summary>Working hours pending approval.</summary>
	[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
	public decimal? PendingHours { get; set; }

	/// <summary>Fraction of days used.</summary>
	[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
	public decimal? UsedFraction { get; set; }

	/// <summary>Fraction of days pending.</summary>
	[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
	public decimal? PendingFraction { get; set; }
}