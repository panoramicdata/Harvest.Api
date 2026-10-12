using Harvest.Dtos;
using System.Text.Json.Serialization;

namespace Harvest.Models;

/// <summary>
/// The body of <c>POST /v2/invoices</c>. Harvest documents two variants: a free-form invoice, given its
/// <see cref="LineItems"/>, and an invoice based on tracked time and expenses, given a <see cref="LineItemsImport"/>.
/// </summary>
public class InvoiceCreationDto : CreationDto
{
	/// <summary>The ID of the client this invoice belongs to.</summary>
	public required long ClientId { get; set; }

	/// <summary>
	/// The ID of the retainer you want to add funds to with this invoice (free-form invoices only). Retainers cannot otherwise
	/// be used through the API: adding funds is the only available action.
	/// </summary>
	public long? RetainerId { get; set; }

	/// <summary>The ID of the estimate associated with this invoice.</summary>
	public long? EstimateId { get; set; }

	/// <summary>The invoice number; Harvest generates one if none is set.</summary>
	public string? Number { get; set; }

	/// <summary>The purchase order number.</summary>
	public string? PurchaseOrder { get; set; }

	/// <summary>This percentage is applied to the subtotal, including line items and discounts. Use 10.0 for 10.0%.</summary>
	public decimal? Tax { get; set; }

	/// <summary>This percentage is applied to the subtotal, including line items and discounts. Use 10.0 for 10.0%.</summary>
	public decimal? Tax2 { get; set; }

	/// <summary>This percentage is subtracted from the subtotal. Use 10.0 for 10.0%.</summary>
	public decimal? Discount { get; set; }

	/// <summary>The invoice subject.</summary>
	public string? Subject { get; set; }

	/// <summary>Any additional notes to include on the invoice.</summary>
	public string? Notes { get; set; }

	/// <summary>The currency used by the invoice; Harvest's default is the client's currency.</summary>
	public string? Currency { get; set; }

	/// <summary>Date the invoice was issued; Harvest's default is today.</summary>
	public DateOnly? IssueDate { get; set; }

	/// <summary>
	/// Date the invoice is due; Harvest's default is the issue date if no <see cref="PaymentTerm"/> is specified. Harvest only
	/// honours a custom due date when <see cref="PaymentTerm"/> is <see cref="InvoicePaymentTerm.Custom"/>; otherwise it
	/// calculates the due date from the issue date and payment term.
	/// </summary>
	public DateOnly? DueDate { get; set; }

	/// <summary>
	/// The timeframe in which the invoice should be paid: one of the <see cref="InvoicePaymentTerm"/> values; Harvest's
	/// default is <c>custom</c>.
	/// </summary>
	public string? PaymentTerm { get; set; }

	/// <summary>
	/// The payment options available to pay the invoice: <see cref="InvoicePaymentOption"/> values. The account must be
	/// configured for them under Settings &gt; Integrations &gt; Online payment.
	/// </summary>
	public List<string>? PaymentOptions { get; set; }

	/// <summary>The line items of a free-form invoice.</summary>
	public List<InvoiceLineItemCreationDto>? LineItems { get; set; }

	/// <summary>The time and expenses to import as line items, for an invoice based on tracked time and expenses.</summary>
	public InvoiceLineItemsImportCreationDto? LineItemsImport { get; set; }
}

/// <summary>A line item of a free-form invoice, in <see cref="InvoiceCreationDto.LineItems"/>.</summary>
public class InvoiceLineItemCreationDto : CreationDto
{
	/// <summary>The ID of the project associated with this line item.</summary>
	public long? ProjectId { get; set; }

	/// <summary>The name of an invoice item category.</summary>
	public required string Kind { get; set; }

	/// <summary>Text description of the line item.</summary>
	public string? Description { get; set; }

	/// <summary>The unit quantity of the item; Harvest's default is 1.</summary>
	public decimal? Quantity { get; set; }

	/// <summary>The individual price per unit.</summary>
	public required decimal UnitPrice { get; set; }

	/// <summary>Whether the invoice's tax percentage applies to this line item; Harvest's default is <see langword="false"/>.</summary>
	public bool? Taxed { get; set; }

	/// <summary>Whether the invoice's tax2 percentage applies to this line item; Harvest's default is <see langword="false"/>.</summary>
	public bool? Taxed2 { get; set; }
}

/// <summary>The <c>line_items_import</c> object of <see cref="InvoiceCreationDto"/>: what tracked time and expenses to invoice.</summary>
public class InvoiceLineItemsImportCreationDto : CreationDto
{
	/// <summary>The client's project IDs to include time and expenses from.</summary>
	public required List<long> ProjectIds { get; set; }

	/// <summary>How to import time entries, or <see langword="null"/> to import none.</summary>
	public InvoiceTimeImportCreationDto? Time { get; set; }

	/// <summary>How to import expenses, or <see langword="null"/> to import none.</summary>
	public InvoiceExpenseImportCreationDto? Expenses { get; set; }
}

/// <summary>The <c>time</c> object of <see cref="InvoiceLineItemsImportCreationDto"/>.</summary>
public class InvoiceTimeImportCreationDto : CreationDto
{
	/// <summary>
	/// How to summarize the time entries per line item: one of the <see cref="InvoiceTimeSummaryType"/> values
	/// (<c>project</c>, <c>task</c>, <c>people</c> or <c>detailed</c>).
	/// </summary>
	public required string SummaryType { get; set; }

	/// <summary>
	/// Start date for included time entries; must be given with <see cref="To"/>. With neither, all unbilled time entries are
	/// included.
	/// </summary>
	public DateOnly? From { get; set; }

	/// <summary>
	/// End date for included time entries; must be given with <see cref="From"/>. With neither, all unbilled time entries are
	/// included.
	/// </summary>
	public DateOnly? To { get; set; }
}

/// <summary>The <c>expenses</c> object of <see cref="InvoiceLineItemsImportCreationDto"/>.</summary>
public class InvoiceExpenseImportCreationDto : CreationDto
{
	/// <summary>
	/// How to summarize the expenses per line item: one of the <see cref="InvoiceExpenseSummaryType"/> values
	/// (<c>project</c>, <c>category</c>, <c>people</c> or <c>detailed</c>).
	/// </summary>
	public required string SummaryType { get; set; }

	/// <summary>
	/// Start date for included expenses; must be given with <see cref="To"/>. With neither, all unbilled expenses are
	/// included.
	/// </summary>
	public DateOnly? From { get; set; }

	/// <summary>
	/// End date for included expenses; must be given with <see cref="From"/>. With neither, all unbilled expenses are
	/// included.
	/// </summary>
	public DateOnly? To { get; set; }

	/// <summary>
	/// Whether to attach a PDF expense report with receipts to the invoice; Harvest's default is <see langword="false"/>.
	/// </summary>
	public bool? AttachReceipts { get; set; }
}

/// <summary>
/// The body of <c>PATCH /v2/invoices/{id}</c>. <see cref="LineItems"/> adds, updates and deletes line items: an entry
/// without an <see cref="InvoiceLineItemPatchDto.Id"/> is added, one with an ID updates that line item, and one with an ID
/// and <see cref="InvoiceLineItemPatchDto.Destroy"/> set deletes it. Line items not listed are unchanged.
/// </summary>
public class InvoicePatchDto : PatchDto
{
	/// <summary>The ID of the client this invoice belongs to.</summary>
	public long? ClientId { get; set; }

	/// <summary>The ID of the retainer associated with this invoice.</summary>
	public long? RetainerId { get; set; }

	/// <summary>The ID of the estimate associated with this invoice.</summary>
	public long? EstimateId { get; set; }

	/// <summary>The invoice number.</summary>
	public string? Number { get; set; }

	/// <summary>The purchase order number.</summary>
	public string? PurchaseOrder { get; set; }

	/// <summary>This percentage is applied to the subtotal, including line items and discounts. Use 10.0 for 10.0%.</summary>
	public decimal? Tax { get; set; }

	/// <summary>This percentage is applied to the subtotal, including line items and discounts. Use 10.0 for 10.0%.</summary>
	public decimal? Tax2 { get; set; }

	/// <summary>This percentage is subtracted from the subtotal. Use 10.0 for 10.0%.</summary>
	public decimal? Discount { get; set; }

	/// <summary>The invoice subject.</summary>
	public string? Subject { get; set; }

	/// <summary>Any additional notes to include on the invoice.</summary>
	public string? Notes { get; set; }

	/// <summary>The currency used by the invoice.</summary>
	public string? Currency { get; set; }

	/// <summary>Date the invoice was issued.</summary>
	public DateOnly? IssueDate { get; set; }

	/// <summary>Date the invoice is due.</summary>
	public DateOnly? DueDate { get; set; }

	/// <summary>The timeframe in which the invoice should be paid: one of the <see cref="InvoicePaymentTerm"/> values.</summary>
	public string? PaymentTerm { get; set; }

	/// <summary>The payment options available to pay the invoice: <see cref="InvoicePaymentOption"/> values.</summary>
	public List<string>? PaymentOptions { get; set; }

	/// <summary>The line items to add, update or delete.</summary>
	public List<InvoiceLineItemPatchDto>? LineItems { get; set; }
}

/// <summary>A line item to add, update or delete, in <see cref="InvoicePatchDto.LineItems"/>.</summary>
public class InvoiceLineItemPatchDto : PatchDto
{
	/// <summary>The ID of the line item to update or delete; <see langword="null"/> to add a new line item.</summary>
	public long? Id { get; set; }

	/// <summary>The ID of the project associated with this line item.</summary>
	public long? ProjectId { get; set; }

	/// <summary>The name of an invoice item category. Required by Harvest when adding a line item.</summary>
	public string? Kind { get; set; }

	/// <summary>Text description of the line item.</summary>
	public string? Description { get; set; }

	/// <summary>The unit quantity of the item; Harvest's default for a new line item is 1.</summary>
	public decimal? Quantity { get; set; }

	/// <summary>The individual price per unit. Required by Harvest when adding a line item.</summary>
	public decimal? UnitPrice { get; set; }

	/// <summary>Whether the invoice's tax percentage applies to this line item.</summary>
	public bool? Taxed { get; set; }

	/// <summary>Whether the invoice's tax2 percentage applies to this line item.</summary>
	public bool? Taxed2 { get; set; }

	/// <summary>Set to <see langword="true"/>, with <see cref="Id"/>, to delete the line item (<c>_destroy</c>).</summary>
	[JsonPropertyName("_destroy")]
	public bool? Destroy { get; set; }
}