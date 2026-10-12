namespace Harvest.Models;

/// <summary>
/// Filtering and paging for <c>GET /v2/contacts</c>. Harvest documents <c>client_id</c>, <c>updated_since</c>,
/// <c>cursor</c>, <c>page</c> and <c>per_page</c> for this endpoint; it does not document <see cref="ListOptions.IsActive"/>.
/// </summary>
public class ContactListOptions : ListOptions
{
	/// <summary>Only return contacts belonging to this client (<c>client_id</c>).</summary>
	[AliasAs("client_id")]
	public long? ClientId { get; init; }
}