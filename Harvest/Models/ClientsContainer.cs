namespace Harvest.Models;

/// <summary>A page of <c>GET /v2/clients</c>.</summary>
public class ClientsContainer : ListContainerBase
{
	/// <summary>The clients on this page (<c>clients</c>).</summary>
	public List<Client> Clients { get; set; } = [];
}