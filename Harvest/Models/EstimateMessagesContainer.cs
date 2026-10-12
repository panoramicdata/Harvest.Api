namespace Harvest.Models;

/// <summary>A page of <c>GET /v2/estimates/{estimate_id}/messages</c>.</summary>
public class EstimateMessagesContainer : ListContainerBase
{
	/// <summary>The estimate messages on this page (<c>estimate_messages</c>).</summary>
	public List<EstimateMessage> EstimateMessages { get; set; } = [];
}