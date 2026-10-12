namespace Harvest.Models;

/// <summary>Filtering for <c>GET /v2/pto/assignments</c>.</summary>
public class PtoAssignmentListOptions
{
	/// <summary>Filter by a single user ID (<c>user_id</c>).</summary>
	[AliasAs("user_id")]
	public long? UserId { get; init; }

	/// <summary>Filter by multiple user IDs, sent as repeated <c>user_ids[]</c> parameters.</summary>
	[AliasAs("user_ids[]")]
	[Query(CollectionFormat.Multi)]
	public IReadOnlyCollection<long>? UserIds { get; init; }
}