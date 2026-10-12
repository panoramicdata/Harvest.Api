namespace Harvest.Models;

/// <summary>A page of <c>GET /v2/pto/requests</c>.</summary>
public class PtoRequestsContainer : ListContainerBase
{
	/// <summary>The time off requests on this page (<c>pto_requests</c>).</summary>
	public List<PtoRequest> PtoRequests { get; set; } = [];
}