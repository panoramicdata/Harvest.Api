namespace Harvest.Models;

/// <summary>A page of <c>GET /v2/invoice_item_categories</c>.</summary>
public class InvoiceItemCategoriesContainer : ListContainerBase
{
	/// <summary>The invoice item categories on this page (<c>invoice_item_categories</c>).</summary>
	public List<InvoiceItemCategory> InvoiceItemCategories { get; set; } = [];
}