namespace Harvest.Models;

/// <summary>The response of <c>GET /v2/pto/assignments</c>. Harvest documents no paging for it.</summary>
public class PtoAssignmentsContainer
{
	/// <summary>The assignments (<c>assignments</c>).</summary>
	public List<PtoAssignment> Assignments { get; set; } = [];
}