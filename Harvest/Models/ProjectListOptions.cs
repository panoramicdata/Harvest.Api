namespace Harvest.Models;

/// <summary>Filtering and paging for <c>GET /v2/projects</c>.</summary>
public class ProjectListOptions : ListOptions
{
	/// <summary>Only return projects belonging to this client (<c>client_id</c>).</summary>
	[AliasAs("client_id")]
	public long? ClientId { get; init; }
}