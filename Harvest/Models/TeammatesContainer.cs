namespace Harvest.Models;

/// <summary>
/// A page of <c>GET /v2/users/{userId}/teammates</c>, and the response of <c>PATCH /v2/users/{userId}/teammates</c> (which
/// carries no paging fields).
/// </summary>
public class TeammatesContainer : ListContainerBase
{
	/// <summary>The teammates on this page (<c>teammates</c>).</summary>
	public List<Teammate> Teammates { get; set; } = [];
}