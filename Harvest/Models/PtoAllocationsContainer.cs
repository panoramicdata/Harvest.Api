namespace Harvest.Models;

/// <summary>A page of <c>GET /v2/pto/allocations</c>.</summary>
public class PtoAllocationsContainer : ListContainerBase
{
	/// <summary>The allocations on this page (<c>pto_allocations</c>).</summary>
	public List<PtoAllocation> PtoAllocations { get; set; } = [];
}