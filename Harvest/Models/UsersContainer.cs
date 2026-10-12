namespace Harvest.Models;

/// <summary>A page of <c>GET /v2/users</c>.</summary>
public class UsersContainer : ListContainerBase
{
	/// <summary>The users on this page (<c>users</c>).</summary>
	public List<User> Users { get; set; } = [];
}