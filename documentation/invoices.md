# Invoices API

The Invoices API creates, lists, updates and deletes invoices, including their line items. You must be an Administrator,
or a Manager with permission to create and edit invoices; otherwise Harvest answers 403 Forbidden.

Harvest reference: <https://help.getharvest.com/api-v2/invoices-api/invoices/invoices/>

Accessed through `IHarvestClient.Invoices` (`IInvoiceApi`). Related resources: [Invoice Messages](invoice-messages.md),
[Invoice Payments](invoice-payments.md) and [Invoice Item Categories](invoice-item-categories.md).

## Endpoints

### List All Invoices

**GET** `/v2/invoices`

Invoices are returned most recently issued first.

**Parameters:**
- `options` (InvoiceListOptions?): Filtering and paging options; pass `null` to send no query parameters
  - `ClientId`, `ProjectId` (long?, sent as `client_id`, `project_id`)
  - `From`, `To` (DateOnly?, sent as `from`, `to` in `YYYY-MM-DD`): filter on `issue_date`
  - `State` (string?, sent as `state`): one of the `InvoiceState` constants (`draft`, `open`, `paid`, `closed`)
  - `UpdatedSince` (DateTimeOffset?, sent as `updated_since` in ISO 8601 UTC)
  - `Cursor`, `Page`, `PerPage` (paging; cursor paging is preferred, see `ListOptions`)

```csharp
Task<InvoicesContainer> ListAsync([Query] InvoiceListOptions? options, CancellationToken cancellationToken);
```

### Get an Invoice

**GET** `/v2/invoices/{id}`

```csharp
Task<Invoice> GetAsync(long id, CancellationToken cancellationToken);
```

### Create an Invoice

**POST** `/v2/invoices`

Harvest documents two variants of the same endpoint, both served by `InvoiceCreationDto`:

- **Free-form**: set `LineItems` (`InvoiceLineItemCreationDto`: `Kind` and `UnitPrice` are required). `RetainerId`
  (add funds to a retainer) applies to this variant only.
- **Based on tracked time and expenses**: set `LineItemsImport` (`InvoiceLineItemsImportCreationDto`) with the client's
  `ProjectIds` and a `Time` (`InvoiceTimeImportCreationDto`) and/or `Expenses` (`InvoiceExpenseImportCreationDto`)
  import, each with a required `SummaryType` (`InvoiceTimeSummaryType` / `InvoiceExpenseSummaryType`) and an optional
  `From`/`To` range (give both or neither; neither imports everything unbilled).

A custom `DueDate` is honoured only when `PaymentTerm` is `InvoicePaymentTerm.Custom` (Harvest's default).

```csharp
Task<Invoice> CreateAsync(InvoiceCreationDto creationDto, CancellationToken cancellationToken);
```

### Update an Invoice

**PATCH** `/v2/invoices/{id}`

Fields left `null` are unchanged. `LineItems` (`InvoiceLineItemPatchDto`) edits line items:

| Entry | Effect |
|-------|--------|
| No `Id` (give `Kind` and `UnitPrice`) | Adds a line item |
| `Id` plus changed fields | Updates that line item |
| `Id` and `Destroy = true` (sent as `_destroy`) | Deletes that line item |

Line items not listed are left as they are.

```csharp
Task<Invoice> UpdateAsync(long id, InvoicePatchDto patchDto, CancellationToken cancellationToken);
```

### Delete an Invoice

**DELETE** `/v2/invoices/{id}`

```csharp
Task DeleteAsync(long id, CancellationToken cancellationToken);
```

## Implementation Status

| Method | Endpoint | Status | Notes |
|--------|----------|--------|-------|
| GET | `/v2/invoices` | ✅ Implemented | All filters and paging |
| GET | `/v2/invoices/{id}` | ✅ Implemented | |
| POST | `/v2/invoices` | ✅ Implemented | Free-form and time/expense import |
| PATCH | `/v2/invoices/{id}` | ✅ Implemented | Including line item add, update and delete |
| DELETE | `/v2/invoices/{id}` | ✅ Implemented | |

## Usage Examples

```csharp
// Open invoices for a client issued this year
var open = await harvestClient.Invoices.ListAsync(new InvoiceListOptions
{
	ClientId = 5735776,
	State = InvoiceState.Open,
	From = new DateOnly(2026, 1, 1)
}, cancellationToken);

// A free-form invoice
var invoice = await harvestClient.Invoices.CreateAsync(new InvoiceCreationDto
{
	ClientId = 5735774,
	Subject = "ABC Project Quote",
	PaymentTerm = InvoicePaymentTerm.Net30,
	LineItems = [new InvoiceLineItemCreationDto { Kind = "Service", Description = "ABC Project", UnitPrice = 5000m }]
}, cancellationToken);

// An invoice from March's tracked time (by task) and all unbilled expenses (by category)
var fromTime = await harvestClient.Invoices.CreateAsync(new InvoiceCreationDto
{
	ClientId = 5735774,
	LineItemsImport = new InvoiceLineItemsImportCreationDto
	{
		ProjectIds = [14307913],
		Time = new InvoiceTimeImportCreationDto
		{
			SummaryType = InvoiceTimeSummaryType.Task,
			From = new DateOnly(2026, 3, 1),
			To = new DateOnly(2026, 3, 31)
		},
		Expenses = new InvoiceExpenseImportCreationDto { SummaryType = InvoiceExpenseSummaryType.Category }
	}
}, cancellationToken);

// Add one line item, change another and delete a third
await harvestClient.Invoices.UpdateAsync(invoice.Id, new InvoicePatchDto
{
	LineItems =
	[
		new InvoiceLineItemPatchDto { Kind = "Service", Description = "DEF Project", UnitPrice = 1000m },
		new InvoiceLineItemPatchDto { Id = 53341928, Description = "ABC Project Phase 2" },
		new InvoiceLineItemPatchDto { Id = 53341929, Destroy = true }
	]
}, cancellationToken);
```

## Data Models

### Invoice
```csharp
public class Invoice
{
	public long Id { get; set; }
	public ClientReference? Client { get; set; }
	public List<InvoiceLineItem>? LineItems { get; set; }
	public EstimateReference? Estimate { get; set; }
	public RetainerReference? Retainer { get; set; }
	public UserReference? Creator { get; set; }
	public string? ClientKey { get; set; }
	public string? Number { get; set; }
	public string? PurchaseOrder { get; set; }
	public decimal Amount { get; set; }
	public decimal DueAmount { get; set; }
	public decimal? Tax { get; set; }
	public decimal? TaxAmount { get; set; }
	public decimal? Tax2 { get; set; }
	public decimal? Tax2Amount { get; set; }
	public decimal? Discount { get; set; }
	public decimal? DiscountAmount { get; set; }
	public string? Subject { get; set; }
	public string? Notes { get; set; }
	public string? Currency { get; set; }
	public string? State { get; set; }              // InvoiceState
	public DateOnly? PeriodStart { get; set; }
	public DateOnly? PeriodEnd { get; set; }
	public DateOnly? IssueDate { get; set; }
	public DateOnly? DueDate { get; set; }
	public string? PaymentTerm { get; set; }        // InvoicePaymentTerm
	public List<string>? PaymentOptions { get; set; } // InvoicePaymentOption
	public DateTime? SentAt { get; set; }
	public DateTime? PaidAt { get; set; }
	public DateOnly? PaidDate { get; set; }
	public DateTime? ClosedAt { get; set; }
	public long? RecurringInvoiceId { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
}
```

### InvoiceLineItem
```csharp
public class InvoiceLineItem
{
	public long Id { get; set; }
	public ProjectReference? Project { get; set; }
	public string? Kind { get; set; }
	public string? Description { get; set; }
	public decimal? Quantity { get; set; }
	public decimal? UnitPrice { get; set; }
	public decimal? Amount { get; set; }
	public bool Taxed { get; set; }
	public bool Taxed2 { get; set; }
}
```

### InvoicesContainer
```csharp
public class InvoicesContainer : ListContainerBase
{
	public List<Invoice> Invoices { get; set; } = [];
}
```
