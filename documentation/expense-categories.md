# Expense Categories API

The Expense Categories API manages the categories that expenses are tracked against. Admin permissions are required.
See the [Harvest documentation](https://help.getharvest.com/api-v2/expenses-api/expenses/expense-categories/).

## Endpoints

### List All Expense Categories

**GET** `/v2/expense_categories`

Retrieves one page of expense categories, most recently created first.

**Parameters:**
- `options` (ListOptions?): Filtering and paging options; pass `null` to send no query parameters
  - `IsActive` (bool?, sent as `is_active`): Only return active (`true`) or inactive (`false`) expense categories
  - `UpdatedSince` (DateTimeOffset?, sent as `updated_since` in ISO 8601 UTC): Only return expense categories updated since the given date and time
  - `Cursor` (string?, sent as `cursor`): The pagination cursor from the previous response
  - `Page` (int?, sent as `page`): The page number (deprecated by Harvest in favour of `Cursor`; default: 1)
  - `PerPage` (int?, sent as `per_page`): The number of records to return per page (default: 2000, range: 1-2000)

**Response:** `ExpenseCategoriesContainer`
```csharp
Task<ExpenseCategoriesContainer> ListAsync([Query] ListOptions? options, CancellationToken cancellationToken);
```

### Get an Expense Category

**GET** `/v2/expense_categories/{id}`

**Response:** `ExpenseCategory`
```csharp
Task<ExpenseCategory> GetAsync(long id, CancellationToken cancellationToken);
```

### Create an Expense Category

**POST** `/v2/expense_categories`

**Request Body:** `ExpenseCategoryCreationDto`
```csharp
public class ExpenseCategoryCreationDto
{
    public required string Name { get; set; }
    public string? UnitName { get; set; }
    public decimal? UnitPrice { get; set; }
    public bool? IsActive { get; set; }   // Harvest's default: true
}
```

**Response:** `ExpenseCategory`
```csharp
Task<ExpenseCategory> CreateAsync(ExpenseCategoryCreationDto creationDto, CancellationToken cancellationToken);
```

### Update an Expense Category

**PATCH** `/v2/expense_categories/{id}`

Fields left `null` are not sent and stay unchanged.

**Request Body:** `ExpenseCategoryPatchDto`
```csharp
public class ExpenseCategoryPatchDto
{
    public string? Name { get; set; }
    public string? UnitName { get; set; }
    public decimal? UnitPrice { get; set; }
    public bool? IsActive { get; set; }
}
```

**Response:** `ExpenseCategory`
```csharp
Task<ExpenseCategory> UpdateAsync(long id, ExpenseCategoryPatchDto patchDto, CancellationToken cancellationToken);
```

### Delete an Expense Category

**DELETE** `/v2/expense_categories/{id}`

```csharp
Task DeleteAsync(long id, CancellationToken cancellationToken);
```

## Implementation Status

| Method | Endpoint | Status | Notes |
|--------|----------|--------|-------|
| GET | `/v2/expense_categories` | ✅ Implemented | Filtering and cursor/page pagination |
| GET | `/v2/expense_categories/{id}` | ✅ Implemented | |
| POST | `/v2/expense_categories` | ✅ Implemented | |
| PATCH | `/v2/expense_categories/{id}` | ✅ Implemented | |
| DELETE | `/v2/expense_categories/{id}` | ✅ Implemented | |

## Usage Examples

```csharp
// List the active expense categories
var categories = await harvestClient.ExpenseCategories.ListAsync(new ListOptions { IsActive = true }, cancellationToken);

// Create a unit-based category
var mileage = await harvestClient.ExpenseCategories.CreateAsync(new ExpenseCategoryCreationDto
{
    Name = "Mileage",
    UnitName = "mile",
    UnitPrice = 0.535m
}, cancellationToken);

// Archive it
await harvestClient.ExpenseCategories.UpdateAsync(mileage.Id, new ExpenseCategoryPatchDto { IsActive = false }, cancellationToken);

// Delete it
await harvestClient.ExpenseCategories.DeleteAsync(mileage.Id, cancellationToken);
```

## Data Models

### ExpenseCategory
```csharp
public class ExpenseCategory
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? UnitName { get; set; }
    public decimal? UnitPrice { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### ExpenseCategoriesContainer
```csharp
public class ExpenseCategoriesContainer : ListContainerBase
{
    public List<ExpenseCategory> ExpenseCategories { get; set; } = [];
}
```
