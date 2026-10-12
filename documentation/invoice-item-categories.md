# Invoice Item Categories API

The Invoice Item Categories API manages the categories (the `kind`) of invoice line items, such as `Service` and
`Product`. Admin or Project Manager permissions are required.

Harvest reference: <https://help.getharvest.com/api-v2/invoices-api/invoices/invoice-item-categories/>

Accessed through `IHarvestClient.InvoiceItemCategories` (`IInvoiceItemCategoryApi`).

## Endpoints

### List All Invoice Item Categories

**GET** `/v2/invoice_item_categories`

**Parameters:**
- `options` (ListOptions?): Filtering and paging options; pass `null` to send no query parameters
  - `UpdatedSince` (DateTimeOffset?, sent as `updated_since` in ISO 8601 UTC)
  - `Cursor`, `Page`, `PerPage` (paging; cursor paging is preferred, see `ListOptions`)

```csharp
Task<InvoiceItemCategoriesContainer> ListAsync([Query] ListOptions? options, CancellationToken cancellationToken);
```

### Get an Invoice Item Category

**GET** `/v2/invoice_item_categories/{id}`

```csharp
Task<InvoiceItemCategory> GetAsync(long id, CancellationToken cancellationToken);
```

### Create an Invoice Item Category

**POST** `/v2/invoice_item_categories`

```csharp
Task<InvoiceItemCategory> CreateAsync([Body] InvoiceItemCategoryCreationDto creationDto, CancellationToken cancellationToken);
```

### Update an Invoice Item Category

**PATCH** `/v2/invoice_item_categories/{id}`

```csharp
Task<InvoiceItemCategory> UpdateAsync(long id, [Body] InvoiceItemCategoryPatchDto patchDto, CancellationToken cancellationToken);
```

### Delete an Invoice Item Category

**DELETE** `/v2/invoice_item_categories/{id}`

Only possible when `UseAsService` and `UseAsExpense` are both `false`.

```csharp
System.Threading.Tasks.Task DeleteAsync(long id, CancellationToken cancellationToken);
```

## Implementation Status

| Method | Endpoint | Status |
|--------|----------|--------|
| GET | `/v2/invoice_item_categories` | ✅ Implemented |
| GET | `/v2/invoice_item_categories/{id}` | ✅ Implemented |
| POST | `/v2/invoice_item_categories` | ✅ Implemented |
| PATCH | `/v2/invoice_item_categories/{id}` | ✅ Implemented |
| DELETE | `/v2/invoice_item_categories/{id}` | ✅ Implemented |

## Usage Examples

```csharp
var categories = await harvestClient.InvoiceItemCategories.ListAsync(null, cancellationToken);

var hosting = await harvestClient.InvoiceItemCategories.CreateAsync(
	new InvoiceItemCategoryCreationDto { Name = "Hosting" },
	cancellationToken);

await harvestClient.InvoiceItemCategories.UpdateAsync(
	hosting.Id,
	new InvoiceItemCategoryPatchDto { Name = "Hosting & Support" },
	cancellationToken);

await harvestClient.InvoiceItemCategories.DeleteAsync(hosting.Id, cancellationToken);
```

## Data Models

### InvoiceItemCategory
```csharp
public class InvoiceItemCategory
{
	public long Id { get; set; }
	public string? Name { get; set; }
	public bool UseAsService { get; set; }
	public bool UseAsExpense { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
}
```
