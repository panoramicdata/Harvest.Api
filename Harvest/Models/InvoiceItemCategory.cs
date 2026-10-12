namespace Harvest.Models;

/// <summary>An invoice item category (<see href="https://help.getharvest.com/api-v2/invoices-api/invoices/invoice-item-categories/"/>).</summary>
public class InvoiceItemCategory
{
	/// <summary>Unique ID for the invoice item category.</summary>
	public long Id { get; set; }

	/// <summary>The name of the invoice item category.</summary>
	public string? Name { get; set; }

	/// <summary>Whether this invoice item category is used for billable hours when generating an invoice.</summary>
	public bool UseAsService { get; set; }

	/// <summary>Whether this invoice item category is used for expenses when generating an invoice.</summary>
	public bool UseAsExpense { get; set; }

	/// <summary>Date and time the invoice item category was created.</summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>Date and time the invoice item category was last updated.</summary>
	public DateTime UpdatedAt { get; set; }
}