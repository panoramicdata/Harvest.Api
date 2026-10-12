# Expenses API

The Expenses API manages expenses and their receipts. Administrators see every expense in the account; Managers see the
expenses of their assigned teammates and managed projects.
See the [Harvest documentation](https://help.getharvest.com/api-v2/expenses-api/expenses/expenses/).

## Endpoints

### List All Expenses

**GET** `/v2/expenses`

Retrieves one page of expenses, sorted by spent date, most recent first.

**Parameters:**
- `options` (ExpenseListOptions?): Filtering and paging options; pass `null` to send no query parameters
  - `UserId` (long?, sent as `user_id`): Only return expenses belonging to the given user
  - `ClientId` (long?, sent as `client_id`): Only return expenses belonging to the given client
  - `ProjectId` (long?, sent as `project_id`): Only return expenses belonging to the given project
  - `IsBilled` (bool?, sent as `is_billed`): `true` for invoiced expenses only, `false` for uninvoiced only
  - `ApprovalStatus` (string?, sent as `approval_status`): `unsubmitted`, `submitted` or `approved` (see the `ApprovalStatus` constants)
  - `From` (DateOnly?, sent as `from`): Only return expenses with a spent date on or after the given date
  - `To` (DateOnly?, sent as `to`): Only return expenses with a spent date on or before the given date
  - `UpdatedSince` (DateTimeOffset?, sent as `updated_since` in ISO 8601 UTC): Only return expenses updated since the given date and time
  - `Cursor` (string?, sent as `cursor`): The pagination cursor from the previous response
  - `Page` (int?, sent as `page`): The page number (default: 1)
  - `PerPage` (int?, sent as `per_page`): The number of records to return per page (default: 2000, range: 1-2000)

**Response:** `ExpensesContainer`
```csharp
Task<ExpensesContainer> ListAsync([Query] ExpenseListOptions? options, CancellationToken cancellationToken);
```

### Get an Expense

**GET** `/v2/expenses/{id}`

**Response:** `Expense`
```csharp
Task<Expense> GetAsync(long id, CancellationToken cancellationToken);
```

### Create an Expense

**POST** `/v2/expenses`

Either `Units` or `TotalCost` is required: `Units` for a unit-based expense category, `TotalCost` otherwise.

**Request Body:** `ExpenseCreationDto`
```csharp
public class ExpenseCreationDto : CreationDto
{
    public long? UserId { get; set; }             // Harvest's default: the authenticated user
    public required long ProjectId { get; set; }
    public required long ExpenseCategoryId { get; set; }
    public required DateOnly SpentDate { get; set; }
    public decimal? Units { get; set; }
    public decimal? TotalCost { get; set; }
    public string? Notes { get; set; }
    public bool? Billable { get; set; }           // Harvest's default: true
}
```

Without a receipt the request is JSON; with a receipt it is `multipart/form-data`, the DTO's fields sent as form fields
and the file as `receipt`:

```csharp
Task<Expense> CreateAsync([Body] ExpenseCreationDto creationDto, CancellationToken cancellationToken);
Task<Expense> CreateAsync(ExpenseCreationDto creationDto, StreamPart receipt, CancellationToken cancellationToken);
```

### Update an Expense

**PATCH** `/v2/expenses/{id}`

Fields left `null` are not sent and stay unchanged. Harvest silently drops changes to `ProjectId` and `ExpenseCategoryId`
if the expense is locked.

**Request Body:** `ExpensePatchDto`
```csharp
public class ExpensePatchDto : PatchDto
{
    public long? ProjectId { get; set; }
    public long? ExpenseCategoryId { get; set; }
    public DateOnly? SpentDate { get; set; }
    public decimal? Units { get; set; }
    public decimal? TotalCost { get; set; }
    public string? Notes { get; set; }
    public bool? Billable { get; set; }
    public bool? DeleteReceipt { get; set; }      // true deletes the attached receipt
}
```

```csharp
Task<Expense> UpdateAsync(long id, [Body] ExpensePatchDto patchDto, CancellationToken cancellationToken);
Task<Expense> UpdateAsync(long id, ExpensePatchDto patchDto, StreamPart receipt, CancellationToken cancellationToken);
```

The receipt overloads are implemented on the interface and call `CreateWithReceiptAsync` / `UpdateWithReceiptAsync`, the
raw Refit `[Multipart]` methods that take each form field as text. Those are hidden from IntelliSense; prefer the
overloads.

### Delete an Expense

**DELETE** `/v2/expenses/{id}`

```csharp
System.Threading.Tasks.Task DeleteAsync(long id, CancellationToken cancellationToken);
```

## Implementation Status

| Method | Endpoint | Status | Notes |
|--------|----------|--------|-------|
| GET | `/v2/expenses` | ✅ Implemented | All documented filters, cursor/page pagination |
| GET | `/v2/expenses/{id}` | ✅ Implemented | |
| POST | `/v2/expenses` | ✅ Implemented | JSON, or multipart/form-data with a receipt |
| PATCH | `/v2/expenses/{id}` | ✅ Implemented | JSON, or multipart/form-data with a receipt; `DeleteReceipt` removes one |
| DELETE | `/v2/expenses/{id}` | ✅ Implemented | |

## Usage Examples

```csharp
// List this month's unsubmitted expenses for a project
var expenses = await harvestClient.Expenses.ListAsync(new ExpenseListOptions
{
    ProjectId = projectId,
    ApprovalStatus = ApprovalStatus.Unsubmitted,
    From = new DateOnly(2025, 3, 1),
    To = new DateOnly(2025, 3, 31)
}, cancellationToken);

// Create an expense
var expense = await harvestClient.Expenses.CreateAsync(new ExpenseCreationDto
{
    ProjectId = projectId,
    ExpenseCategoryId = categoryId,
    SpentDate = new DateOnly(2025, 3, 1),
    TotalCost = 13.59m
}, cancellationToken);

// Attach a receipt while changing the notes
await using var file = File.OpenRead("dinner_receipt.gif");
expense = await harvestClient.Expenses.UpdateAsync(
    expense.Id,
    new ExpensePatchDto { Notes = "Dinner" },
    new StreamPart(file, "dinner_receipt.gif", "image/gif"),
    cancellationToken);

// Remove the receipt
await harvestClient.Expenses.UpdateAsync(expense.Id, new ExpensePatchDto { DeleteReceipt = true }, cancellationToken);

// Delete the expense
await harvestClient.Expenses.DeleteAsync(expense.Id, cancellationToken);
```

## Data Models

### Expense
```csharp
public class Expense
{
    public long Id { get; set; }
    public ClientReference? Client { get; set; }                     // id, name, currency
    public ProjectReference? Project { get; set; }                   // id, name, code
    public ExpenseCategoryReference? ExpenseCategory { get; set; }   // id, name, unit_price, unit_name
    public UserReference? User { get; set; }
    public UserAssignment? UserAssignment { get; set; }
    public ExpenseReceipt? Receipt { get; set; }                     // url, file_name, file_size, content_type
    public InvoiceReference? Invoice { get; set; }                   // set once invoiced
    public string? Notes { get; set; }
    public decimal? Units { get; set; }
    public decimal TotalCost { get; set; }
    public bool Billable { get; set; }
    public bool IsClosed { get; set; }                               // deprecated: use ApprovalStatus
    public string? ApprovalStatus { get; set; }
    public bool IsLocked { get; set; }
    public bool IsBilled { get; set; }
    public string? LockedReason { get; set; }
    public DateOnly SpentDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### ExpensesContainer
```csharp
public class ExpensesContainer : ListContainerBase
{
    public List<Expense> Expenses { get; set; } = [];
}
```
