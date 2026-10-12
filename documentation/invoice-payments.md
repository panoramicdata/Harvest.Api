# Invoice Payments API

The Invoice Payments API lists, records and deletes payments on an invoice. You must be an Administrator, or a Manager
with permission to create and edit invoices.

Harvest reference: <https://help.getharvest.com/api-v2/invoices-api/invoices/invoice-payments/>

Accessed through `IHarvestClient.InvoicePayments` (`IInvoicePaymentApi`). See also [Invoices](invoices.md).

## Endpoints

### List All Payments for an Invoice

**GET** `/v2/invoices/{invoiceId}/payments`

Payments are returned most recently created first.

**Parameters:**
- `invoiceId` (long): The invoice ID
- `options` (ListOptions?): Filtering and paging options; pass `null` to send no query parameters
  - `UpdatedSince` (DateTimeOffset?, sent as `updated_since` in ISO 8601 UTC)
  - `Cursor`, `Page`, `PerPage` (paging; cursor paging is preferred, see `ListOptions`)

```csharp
Task<InvoicePaymentsContainer> ListAsync(long invoiceId, [Query] ListOptions? options, CancellationToken cancellationToken);
```

### Create an Invoice Payment

**POST** `/v2/invoices/{invoiceId}/payments`

`Amount` is required. Pass either `PaidAt` (a UTC `DateTime`) or `PaidDate`, not both. `SendThankYou` defaults to `true`
in Harvest, and only sends an e-mail if the invoice is fully paid after this payment.

```csharp
Task<InvoicePayment> CreateAsync(long invoiceId, InvoicePaymentCreationDto creationDto, CancellationToken cancellationToken);
```

### Delete an Invoice Payment

**DELETE** `/v2/invoices/{invoiceId}/payments/{id}`

```csharp
Task DeleteAsync(long invoiceId, long id, CancellationToken cancellationToken);
```

## Implementation Status

| Method | Endpoint | Status |
|--------|----------|--------|
| GET | `/v2/invoices/{invoiceId}/payments` | ✅ Implemented |
| POST | `/v2/invoices/{invoiceId}/payments` | ✅ Implemented |
| DELETE | `/v2/invoices/{invoiceId}/payments/{id}` | ✅ Implemented |

Harvest documents no retrieve or update endpoint for invoice payments.

## Usage Examples

```csharp
var payments = await harvestClient.InvoicePayments.ListAsync(invoiceId, null, cancellationToken);

var payment = await harvestClient.InvoicePayments.CreateAsync(invoiceId, new InvoicePaymentCreationDto
{
	Amount = 1575.86m,
	PaidDate = new DateOnly(2026, 7, 24),
	Notes = "Paid by phone",
	SendThankYou = false
}, cancellationToken);

await harvestClient.InvoicePayments.DeleteAsync(invoiceId, payment.Id, cancellationToken);
```

## Data Models

### InvoicePayment
```csharp
public class InvoicePayment
{
	public long Id { get; set; }
	public decimal Amount { get; set; }
	public DateTime? PaidAt { get; set; }
	public DateOnly? PaidDate { get; set; }
	public string? RecordedBy { get; set; }
	public string? RecordedByEmail { get; set; }
	public string? Notes { get; set; }
	public string? TransactionId { get; set; }
	public PaymentGatewayReference? PaymentGateway { get; set; } // Id and Name are null for hand-recorded payments
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
}
```
