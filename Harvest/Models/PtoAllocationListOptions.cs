namespace Harvest.Models;

/// <summary>
/// Filtering and paging for <c>GET /v2/pto/allocations</c>. <see cref="ListOptions.IsActive"/> and
/// <see cref="ListOptions.UpdatedSince"/> are not documented for it.
/// </summary>
public class PtoAllocationListOptions : ListOptions
{
	/// <summary>The allocation year (<c>year</c>); Harvest's default is the current year.</summary>
	[AliasAs("year")]
	public int? Year { get; init; }

	/// <summary>Filter allocations by user IDs, sent as repeated <c>user_ids[]</c> parameters.</summary>
	[AliasAs("user_ids[]")]
	[Query(CollectionFormat.Multi)]
	public IReadOnlyCollection<long>? UserIds { get; init; }

	/// <summary>Filter allocations by time off policy IDs, sent as repeated <c>pto_type_ids[]</c> parameters.</summary>
	[AliasAs("pto_type_ids[]")]
	[Query(CollectionFormat.Multi)]
	public IReadOnlyCollection<long>? PtoTypeIds { get; init; }
}