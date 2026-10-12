namespace Harvest.Models;

/// <summary>A page of <c>GET /v2/pto/types</c>.</summary>
public class PtoTypesContainer : ListContainerBase
{
	/// <summary>The time off policies on this page (<c>pto_types</c>).</summary>
	public List<PtoType> PtoTypes { get; set; } = [];
}
