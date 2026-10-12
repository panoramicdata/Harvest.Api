namespace Harvest.Models;

/// <summary>A page of <c>GET /v2/roles</c>.</summary>
public class RolesContainer : ListContainerBase
{
	/// <summary>The roles on this page (<c>roles</c>).</summary>
	public List<Role> Roles { get; set; } = [];
}