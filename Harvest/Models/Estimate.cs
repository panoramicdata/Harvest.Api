namespace Harvest.Models;

/// <summary>An estimate (<see href="https://help.getharvest.com/api-v2/estimates-api/estimates/estimates/"/>).</summary>
public class Estimate
{
	/// <summary>Unique ID for the estimate.</summary>
	public long Id { get; set; }

	/// <summary>The estimate's client: its id and name.</summary>
	public ClientReference? Client { get; set; }

	/// <summary>The estimate's line items.</summary>
	public List<EstimateLineItem>? LineItems { get; set; }

	/// <summary>The id and name of the person that created the estimate.</summary>
	public UserReference? Creator { get; set; }

	/// <summary>
	/// Used to build a URL to the public web estimate for your client:
	/// <c>https://{ACCOUNT_SUBDOMAIN}.harvestapp.com/client/estimates/{client_key}</c>.
	/// </summary>
	public string? ClientKey { get; set; }

	/// <summary>The estimate number; generated automatically if none was set.</summary>
	public string? Number { get; set; }

	/// <summary>The purchase order number.</summary>
	public string? PurchaseOrder { get; set; }

	/// <summary>The total amount for the estimate, including any discounts and taxes.</summary>
	public decimal? Amount { get; set; }

	/// <summary>This percentage is applied to the subtotal, including line items and discounts.</summary>
	public decimal? Tax { get; set; }

	/// <summary>The first amount of tax included, calculated from <see cref="Tax"/>; <see langword="null"/> if no tax is defined.</summary>
	public decimal? TaxAmount { get; set; }

	/// <summary>This second percentage is applied to the subtotal, including line items and discounts.</summary>
	public decimal? Tax2 { get; set; }

	/// <summary>The amount calculated from <see cref="Tax2"/>.</summary>
	public decimal? Tax2Amount { get; set; }

	/// <summary>This percentage is subtracted from the subtotal.</summary>
	public decimal? Discount { get; set; }

	/// <summary>The amount calculated from <see cref="Discount"/>.</summary>
	public decimal? DiscountAmount { get; set; }

	/// <summary>The estimate subject.</summary>
	public string? Subject { get; set; }

	/// <summary>Any additional notes included on the estimate.</summary>
	public string? Notes { get; set; }

	/// <summary>The currency code associated with this estimate.</summary>
	public string? Currency { get; set; }

	/// <summary>The current state of the estimate: one of the <see cref="EstimateState"/> values.</summary>
	public string? State { get; set; }

	/// <summary>Date the estimate was issued.</summary>
	public DateOnly? IssueDate { get; set; }

	/// <summary>Date and time the estimate was sent.</summary>
	public DateTime? SentAt { get; set; }

	/// <summary>Date and time the estimate was accepted.</summary>
	public DateTime? AcceptedAt { get; set; }

	/// <summary>Date and time the estimate was declined.</summary>
	public DateTime? DeclinedAt { get; set; }

	/// <summary>Date and time the estimate was created.</summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>Date and time the estimate was last updated.</summary>
	public DateTime UpdatedAt { get; set; }
}