# Invoice Messages API

The Invoice Messages API sends invoices to clients and records state changes (marking an invoice as sent, closing it,
re-opening it or returning it to draft). You must be an Administrator, or a Manager with permission to create and edit
invoices.

Harvest reference: <https://help.getharvest.com/api-v2/invoices-api/invoices/invoice-messages/>

Accessed through `IHarvestClient.InvoiceMessages` (`IInvoiceMessageApi`). See also [Invoices](invoices.md).

## Endpoints

### List All Messages for an Invoice

**GET** `/v2/invoices/{invoiceId}/messages`

Messages are returned most recently created first.

**Parameters:**
- `invoiceId` (long): The invoice ID
- `options` (ListOptions?): Filtering and paging options; pass `null` to send no query parameters
  - `UpdatedSince` (DateTimeOffset?, sent as `updated_since` in ISO 8601 UTC)
  - `Cursor`, `Page`, `PerPage` (paging; cursor paging is preferred, see `ListOptions`)

```csharp
Task<InvoiceMessagesContainer> ListAsync(long invoiceId, [Query] ListOptions? options, CancellationToken cancellationToken);
```

### Get the Subject and Body for a New Message

**GET** `/v2/invoices/{invoiceId}/messages/new`

Returns the subject and body Harvest has configured for a message on this invoice, without creating it.

**Parameters:**
- `options` (InvoiceMessageTemplateOptions?): `null` for a general invoice message, or
  - `ThankYou` (bool?, sent as `thank_you`): a thank-you message
  - `Reminder` (bool?, sent as `reminder`): a reminder message

```csharp
Task<InvoiceMessageTemplate> GetTemplateAsync(long invoiceId, [Query] InvoiceMessageTemplateOptions? options, CancellationToken cancellationToken);
```

### Create an Invoice Message

**POST** `/v2/invoices/{invoiceId}/messages`

One endpoint, two behaviours, chosen by `EventType`:

| `EventType` | Effect |
|-------------|--------|
| `null` (omitted) | Creates the message and **sends** it. Give `Recipients` or `SendMeACopy = true`, or Harvest refuses it. |
| `InvoiceMessageEventType.Send` (`send`) | Marks a draft invoice as sent; nothing is e-mailed |
| `InvoiceMessageEventType.Close` (`close`) | Marks an open invoice as closed (written off) |
| `InvoiceMessageEventType.ReOpen` (`re-open`) | Re-opens a closed invoice |
| `InvoiceMessageEventType.Draft` (`draft`) | Marks an open invoice as a draft |

`IncludeLinkToClientInvoice` is deprecated by Harvest: `true` is ignored and `false` clears the invoice's payment options.

```csharp
Task<InvoiceMessage> CreateAsync(long invoiceId, [Body] InvoiceMessageCreationDto creationDto, CancellationToken cancellationToken);
```

### Delete an Invoice Message

**DELETE** `/v2/invoices/{invoiceId}/messages/{id}`

```csharp
System.Threading.Tasks.Task DeleteAsync(long invoiceId, long id, CancellationToken cancellationToken);
```

## Implementation Status

| Method | Endpoint | Status | Notes |
|--------|----------|--------|-------|
| GET | `/v2/invoices/{invoiceId}/messages` | ✅ Implemented | |
| GET | `/v2/invoices/{invoiceId}/messages/new` | ✅ Implemented | `thank_you` and `reminder` |
| POST | `/v2/invoices/{invoiceId}/messages` | ✅ Implemented | Send, and every `event_type` |
| DELETE | `/v2/invoices/{invoiceId}/messages/{id}` | ✅ Implemented | |

## Usage Examples

```csharp
// Send the invoice, starting from Harvest's configured wording
var template = await harvestClient.InvoiceMessages.GetTemplateAsync(invoiceId, null, cancellationToken);
await harvestClient.InvoiceMessages.CreateAsync(invoiceId, new InvoiceMessageCreationDto
{
	Subject = template.Subject,
	Body = template.Body,
	AttachPdf = true,
	Recipients = [new MessageRecipientCreationDto { Name = "Richard Roe", Email = "richard@example.com" }]
}, cancellationToken);

// Mark a draft invoice as sent without e-mailing it
await harvestClient.InvoiceMessages.CreateAsync(invoiceId,
	new InvoiceMessageCreationDto { EventType = InvoiceMessageEventType.Send },
	cancellationToken);

// Write an open invoice off
await harvestClient.InvoiceMessages.CreateAsync(invoiceId,
	new InvoiceMessageCreationDto { EventType = InvoiceMessageEventType.Close },
	cancellationToken);
```

## Data Models

### InvoiceMessage
```csharp
public class InvoiceMessage
{
	public long Id { get; set; }
	public string? SentBy { get; set; }
	public string? SentByEmail { get; set; }
	public string? SentFrom { get; set; }
	public string? SentFromEmail { get; set; }
	public List<MessageRecipient>? Recipients { get; set; }
	public string? Subject { get; set; }
	public string? Body { get; set; }
	public bool IncludeLinkToClientInvoice { get; set; } // deprecated by Harvest
	public bool AttachPdf { get; set; }
	public bool SendMeACopy { get; set; }
	public bool ThankYou { get; set; }
	public string? EventType { get; set; }               // InvoiceMessageEventType; null = sent
	public bool Reminder { get; set; }
	public DateOnly? SendReminderOn { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
}
```

### InvoiceMessageTemplate
```csharp
public class InvoiceMessageTemplate
{
	public long InvoiceId { get; set; }
	public string? Subject { get; set; }
	public string? Body { get; set; }
	public bool Reminder { get; set; }
	public bool ThankYou { get; set; }
}
```
