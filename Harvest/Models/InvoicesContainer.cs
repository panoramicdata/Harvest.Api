namespace Harvest.Models;

/// <summary>A page of <c>GET /v2/invoices</c>.</summary>
public class InvoicesContainer : ListContainerBase
{
	/// <summary>The invoices on this page (<c>invoices</c>).</summary>
	public List<Invoice> Invoices { get; set; } = [];
}