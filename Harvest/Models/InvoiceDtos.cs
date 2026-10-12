using Harvest.Dtos;

namespace Harvest.Models;

/// <summary>The optional fields the bodies that create and update an invoice share, beyond those of any document.</summary>
/// <typeparam name="TLineItem">The body's line item type.</typeparam>
public abstract class InvoiceDtoBase<TLineItem> : BillingDocumentDto<TLineItem> where TLineItem : class
{
	/// <summary>
	/// The ID of the retainer you want to add funds to with this invoice (free-form invoices only). Retainers cannot otherwise
	/// be used through the API: adding funds is the only available action.
	/// </summary>
	public long? RetainerId { get; set; }

	/// <summary>The ID of the estimate associated with this invoice.</summary>
	public long? EstimateId { get; set; }

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
}

/// <summary>
/// The body of <c>POST /v2/invoices</c>. Harvest documents two variants: a free-form invoice, given its line items, and an
/// invoice based on tracked time and expenses, given a <see cref="LineItemsImport"/>.
/// </summary>
public class InvoiceCreationDto : InvoiceDtoBase<InvoiceLineItemCreationDto>
{
	/// <summary>The ID of the client this invoice belongs to.</summary>
	public required long ClientId { get; set; }

	/// <summary>The time and expenses to import as line items, for an invoice based on tracked time and expenses.</summary>
	public InvoiceLineItemsImportCreationDto? LineItemsImport { get; set; }
}

/// <summary>A line item of a free-form invoice, in <see cref="InvoiceCreationDto"/>'s line items.</summary>
public class InvoiceLineItemCreationDto : LineItemCreationDto
{
	/// <summary>The ID of the project associated with this line item.</summary>
	public long? ProjectId { get; set; }
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
/// The body of <c>PATCH /v2/invoices/{id}</c>. Its line items add, update and delete line items: an entry without an
/// <see cref="LineItemPatchDto.Id"/> is added, one with an ID updates that line item, and one with an ID and
/// <see cref="LineItemPatchDto.Destroy"/> set deletes it. Line items not listed are unchanged.
/// </summary>
public class InvoicePatchDto : InvoiceDtoBase<InvoiceLineItemPatchDto>
{
	/// <summary>The ID of the client this invoice belongs to.</summary>
	public long? ClientId { get; set; }
}

/// <summary>A line item to add, update or delete, in <see cref="InvoicePatchDto"/>'s line items.</summary>
public class InvoiceLineItemPatchDto : LineItemPatchDto
{
	/// <summary>The ID of the project associated with this line item.</summary>
	public long? ProjectId { get; set; }
}