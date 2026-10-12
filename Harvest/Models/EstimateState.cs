namespace Harvest.Models;

/// <summary>
/// The values of an estimate's <c>state</c>, also accepted by <see cref="EstimateListOptions.State"/>. They are strings
/// rather than an enum so that a value Harvest adds later is still read.
/// </summary>
public static class EstimateState
{
	/// <summary>Not yet sent.</summary>
	public const string Draft = "draft";

	/// <summary>Sent to the client.</summary>
	public const string Sent = "sent";

	/// <summary>Accepted by the client.</summary>
	public const string Accepted = "accepted";

	/// <summary>Declined by the client.</summary>
	public const string Declined = "declined";
}