# Estimates API

The Estimates API covers estimates and their line items, estimate messages (which also move an estimate between
states) and estimate item categories. It is exposed through three properties of `HarvestClient` / `IHarvestClient`:

| Property | Interface | Harvest page |
|----------|-----------|--------------|
| `Estimates` | `IEstimateApi` | [Estimates](https://help.getharvest.com/api-v2/estimates-api/estimates/estimates/) |
| `EstimateMessages` | `IEstimateMessageApi` | [Estimate Messages](https://help.getharvest.com/api-v2/estimates-api/estimates/estimate-messages/) |
| `EstimateItemCategories` | `IEstimateItemCategoryApi` | [Estimate Item Categories](https://help.getharvest.com/api-v2/estimates-api/estimates/estimate-item-categories/) |

Estimates and estimate messages require an Administrator, or a Manager with permission to create and edit estimates;
estimate item categories require Admin permissions. Harvest answers `403 Forbidden` otherwise.

## Endpoints

### List Estimates

**GET** `/v2/estimates`

Lists one page of estimates, most recently issued first.

**Parameters:**
- `options` (EstimateListOptions?): Filtering and paging options; pass `null` to send no query parameters
  - `ClientId` (long?, sent as `client_id`): Only return estimates belonging to this client
  - `From` (DateOnly?, sent as `from`): Only return estimates with an `issue_date` on or after this date
  - `To` (DateOnly?, sent as `to`): Only return estimates with an `issue_date` on or before this date
  - `State` (string?, sent as `state`): `draft`, `sent`, `accepted` or `declined` (see `EstimateState`)
  - `UpdatedSince` (DateTimeOffset?, sent as `updated_since` in ISO 8601 UTC): Only return estimates updated since then
  - `Cursor`, `Page`, `PerPage`: Paging (see the README)

**Response:** `EstimatesContainer`
```csharp
Task<EstimatesContainer> ListAsync([Query] EstimateListOptions? options, CancellationToken cancellationToken);
```

### Get an Estimate

**GET** `/v2/estimates/{id}`

```csharp
Task<Estimate> GetAsync(long id, CancellationToken cancellationToken);
```

### Create an Estimate

**POST** `/v2/estimates`

`ClientId` is required. Each line item in `LineItems` requires `Kind` (an estimate item category name) and `UnitPrice`;
`Quantity` defaults to 1 and `Taxed` / `Taxed2` to `false`. `IssueDate` defaults to today and `Currency` to the
client's currency.

```csharp
Task<Estimate> CreateAsync([Body] EstimateCreationDto creationDto, CancellationToken cancellationToken);
```

### Update an Estimate (and its Line Items)

**PATCH** `/v2/estimates/{id}`

Fields left `null` are unchanged. `EstimatePatchDto.LineItems` adds, updates and deletes line items in the same call;
line items not listed are left alone:

| Change | `EstimateLineItemPatchDto` | Wire |
|--------|----------------------------|------|
| Add | no `Id`; `Kind` and `UnitPrice` set | `{"kind":"Service","unit_price":1000.0}` |
| Update | `Id` plus the fields to change | `{"id":53339199,"description":"Project Phase 2"}` |
| Delete | `EstimateLineItemPatchDto.Destroying(id)` | `{"id":53339199,"_destroy":true}` |

```csharp
Task<Estimate> UpdateAsync(long id, [Body] EstimatePatchDto patchDto, CancellationToken cancellationToken);
```

### Delete an Estimate

**DELETE** `/v2/estimates/{id}`

```csharp
System.Threading.Tasks.Task DeleteAsync(long id, CancellationToken cancellationToken);
```

### List an Estimate's Messages

**GET** `/v2/estimates/{estimateId}/messages`

Takes `ListOptions` (`UpdatedSince`, `Cursor`, `Page`, `PerPage`).

```csharp
Task<EstimateMessagesContainer> ListAsync(long estimateId, [Query] ListOptions? options, CancellationToken cancellationToken);
```

### Create an Estimate Message / Run an Estimate Event

**POST** `/v2/estimates/{estimateId}/messages`

A message emails its `Recipients` (each needs an `Email`). Setting `EventType` also runs an event against the estimate.
To change state without emailing anyone, send only the event with `EstimateMessageCreationDto.ForEvent(...)`:

| Event (`EstimateMessageEventType`) | Wire value | Effect |
|------------------------------------|------------|--------|
| `Send` | `send` | Marks a draft estimate as sent |
| `Accept` | `accept` | Marks an open estimate as accepted |
| `Decline` | `decline` | Marks an open estimate as declined |
| `ReOpen` | `re-open` | Re-opens a closed estimate |

`View` (`view`) and `Invoice` (`invoice`) appear on messages Harvest records itself; they cannot be sent.

```csharp
Task<EstimateMessage> CreateAsync(long estimateId, [Body] EstimateMessageCreationDto creationDto, CancellationToken cancellationToken);
```

### Delete an Estimate Message

**DELETE** `/v2/estimates/{estimateId}/messages/{id}`

```csharp
System.Threading.Tasks.Task DeleteAsync(long estimateId, long id, CancellationToken cancellationToken);
```

### Estimate Item Categories

| Method | Endpoint | C# |
|--------|----------|----|
| GET | `/v2/estimate_item_categories` | `ListAsync([Query] ListOptions? options, CancellationToken)` |
| GET | `/v2/estimate_item_categories/{id}` | `GetAsync(long id, CancellationToken)` |
| POST | `/v2/estimate_item_categories` | `CreateAsync(EstimateItemCategoryCreationDto, CancellationToken)` (`Name` required) |
| PATCH | `/v2/estimate_item_categories/{id}` | `UpdateAsync(long id, EstimateItemCategoryPatchDto, CancellationToken)` |
| DELETE | `/v2/estimate_item_categories/{id}` | `DeleteAsync(long id, CancellationToken)` |

## Implementation Status

| Method | Endpoint | Status | Notes |
|--------|----------|--------|-------|
| GET | `/v2/estimates` | ✅ Implemented | All filters: `client_id`, `from`, `to`, `state`, `updated_since`, paging |
| GET | `/v2/estimates/{id}` | ✅ Implemented | |
| POST | `/v2/estimates` | ✅ Implemented | With line items |
| PATCH | `/v2/estimates/{id}` | ✅ Implemented | Line item add, update and delete (`_destroy`) |
| DELETE | `/v2/estimates/{id}` | ✅ Implemented | |
| GET | `/v2/estimates/{estimateId}/messages` | ✅ Implemented | |
| POST | `/v2/estimates/{estimateId}/messages` | ✅ Implemented | Messages and `send` / `accept` / `decline` / `re-open` events |
| DELETE | `/v2/estimates/{estimateId}/messages/{id}` | ✅ Implemented | |
| GET | `/v2/estimate_item_categories` | ✅ Implemented | |
| GET | `/v2/estimate_item_categories/{id}` | ✅ Implemented | |
| POST | `/v2/estimate_item_categories` | ✅ Implemented | |
| PATCH | `/v2/estimate_item_categories/{id}` | ✅ Implemented | |
| DELETE | `/v2/estimate_item_categories/{id}` | ✅ Implemented | |

## Missing Functionality

None: every documented estimates operation is implemented.

## Usage Examples

```csharp
// Sent estimates for one client issued this year
var sent = await harvestClient.Estimates.ListAsync(new EstimateListOptions
{
    ClientId = 5735776,
    State = EstimateState.Sent,
    From = new DateOnly(2026, 1, 1)
}, cancellationToken);

// Create a draft estimate with one line item
var estimate = await harvestClient.Estimates.CreateAsync(new EstimateCreationDto
{
    ClientId = 5735774,
    Subject = "ABC Project Quote",
    LineItems = [new EstimateLineItemCreationDto { Kind = "Service", Description = "ABC Project Quote", UnitPrice = 5000m }]
}, cancellationToken);

// Add one line item, change another and delete a third in one call
await harvestClient.Estimates.UpdateAsync(estimate.Id, new EstimatePatchDto
{
    LineItems =
    [
        new EstimateLineItemPatchDto { Kind = "Service", Description = "Another Project", UnitPrice = 1000m },
        new EstimateLineItemPatchDto { Id = 53339199, Description = "Project Phase 2" },
        EstimateLineItemPatchDto.Destroying(53339200)
    ]
}, cancellationToken);

// Email the estimate to the client
await harvestClient.EstimateMessages.CreateAsync(estimate.Id, new EstimateMessageCreationDto
{
    Subject = "Estimate #1001",
    Body = "Here is our estimate.",
    SendMeACopy = true,
    Recipients = [new EstimateMessageRecipientCreationDto { Name = "Richard Roe", Email = "richard@example.com" }]
}, cancellationToken);

// Mark it accepted without emailing anyone
await harvestClient.EstimateMessages.CreateAsync(estimate.Id,
    EstimateMessageCreationDto.ForEvent(EstimateMessageEventType.Accept), cancellationToken);

// Estimate item categories
var category = await harvestClient.EstimateItemCategories.CreateAsync(
    new EstimateItemCategoryCreationDto { Name = "Hosting" }, cancellationToken);
```

## Data Models

### Estimate
```csharp
public class Estimate
{
    public long Id { get; set; }
    public ClientReference? Client { get; set; }           // id, name
    public List<EstimateLineItem>? LineItems { get; set; }
    public UserReference? Creator { get; set; }            // id, name
    public string? ClientKey { get; set; }
    public string? Number { get; set; }
    public string? PurchaseOrder { get; set; }
    public decimal? Amount { get; set; }
    public decimal? Tax { get; set; }
    public decimal? TaxAmount { get; set; }
    public decimal? Tax2 { get; set; }
    public decimal? Tax2Amount { get; set; }
    public decimal? Discount { get; set; }
    public decimal? DiscountAmount { get; set; }
    public string? Subject { get; set; }
    public string? Notes { get; set; }
    public string? Currency { get; set; }
    public string? State { get; set; }                     // EstimateState values
    public DateOnly? IssueDate { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? DeclinedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### EstimateLineItem
```csharp
public class EstimateLineItem
{
    public long Id { get; set; }
    public string? Kind { get; set; }
    public string? Description { get; set; }
    public decimal? Quantity { get; set; }                 // documented as integer, returned as 1.0
    public decimal? UnitPrice { get; set; }
    public decimal? Amount { get; set; }
    public bool Taxed { get; set; }
    public bool Taxed2 { get; set; }
}
```

### EstimateMessage
```csharp
public class EstimateMessage
{
    public long Id { get; set; }
    public string? SentBy { get; set; }
    public string? SentByEmail { get; set; }
    public string? SentFrom { get; set; }
    public string? SentFromEmail { get; set; }
    public List<EstimateMessageRecipient>? Recipients { get; set; }  // name, email
    public string? Subject { get; set; }
    public string? Body { get; set; }
    public bool SendMeACopy { get; set; }
    public string? EventType { get; set; }                 // EstimateMessageEventType values, or null
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### EstimateItemCategory
```csharp
public class EstimateItemCategory
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### Containers
```csharp
public class EstimatesContainer : ListContainerBase { public List<Estimate> Estimates { get; set; } = []; }
public class EstimateMessagesContainer : ListContainerBase { public List<EstimateMessage> EstimateMessages { get; set; } = []; }
public class EstimateItemCategoriesContainer : ListContainerBase { public List<EstimateItemCategory> EstimateItemCategories { get; set; } = []; }
```
