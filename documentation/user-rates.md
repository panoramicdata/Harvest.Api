# User Billable Rates and Cost Rates APIs

These APIs read and create the dated billable rates
([Harvest documentation](https://help.getharvest.com/api-v2/users-api/users/billable-rates/)) and cost rates
([Harvest documentation](https://help.getharvest.com/api-v2/users-api/users/cost-rates/)) of a user. The two resources
have the same shape.

Exposed as `harvestClient.BillableRates` (`IBillableRateApi`) and `harvestClient.CostRates` (`ICostRateApi`).

Billable rates need an Administrator, or a Manager with permission to edit billable rates. Cost rates need an
Administrator. Anyone else gets `403 Forbidden`.

## Endpoints

### List a User's Rates

**GET** `/v2/users/{userId}/billable_rates` and `/v2/users/{userId}/cost_rates`

Rates are returned oldest start date first.

**Parameters:**
- `userId` (long): The user ID
- `options` (ListOptions?): Paging options; pass `null` to send no query parameters. Harvest documents no filters.
  - `Cursor` (string?, sent as `cursor`): The cursor from the previous page's `Links.NextCursor`
  - `Page` (int?, sent as `page`): The page number to use in pagination (default: 1, deprecated by Harvest)
  - `PerPage` (int?, sent as `per_page`): The number of records to return per page (default: 2000, range: 1-2000)

```csharp
Task<BillableRatesContainer> ListAsync(long userId, [Query] ListOptions? options, CancellationToken cancellationToken);
Task<CostRatesContainer> ListAsync(long userId, [Query] ListOptions? options, CancellationToken cancellationToken);
```

### Get a Rate

**GET** `/v2/users/{userId}/billable_rates/{id}` and `/v2/users/{userId}/cost_rates/{id}`

```csharp
Task<BillableRate> GetAsync(long userId, long id, CancellationToken cancellationToken);
Task<CostRate> GetAsync(long userId, long id, CancellationToken cancellationToken);
```

### Create a Rate

**POST** `/v2/users/{userId}/billable_rates` and `/v2/users/{userId}/cost_rates`

A rate with no start date replaces all of the user's existing rates; one whose start date is before existing rates
replaces those rates.

**Body:** `BillableRateCreationDto` / `CostRateCreationDto`
- `Amount` (decimal, required): The amount of the rate
- `StartDate` (DateOnly?, sent as `YYYY-MM-DD`): The date the rate is effective; cannot be in the future

```csharp
Task<BillableRate> CreateAsync(long userId, [Body] BillableRateCreationDto creationDto, CancellationToken cancellationToken);
Task<CostRate> CreateAsync(long userId, [Body] CostRateCreationDto creationDto, CancellationToken cancellationToken);
```

## Implementation Status

| Method | Endpoint | Status | Notes |
|--------|----------|--------|-------|
| GET | `/v2/users/{userId}/billable_rates` | ✅ Implemented | Paged list |
| GET | `/v2/users/{userId}/billable_rates/{id}` | ✅ Implemented | Individual rate retrieval |
| POST | `/v2/users/{userId}/billable_rates` | ✅ Implemented | Rate creation |
| GET | `/v2/users/{userId}/cost_rates` | ✅ Implemented | Paged list |
| GET | `/v2/users/{userId}/cost_rates/{id}` | ✅ Implemented | Individual rate retrieval |
| POST | `/v2/users/{userId}/cost_rates` | ✅ Implemented | Rate creation |

Harvest documents no update or delete for rates.

## Usage Examples

```csharp
await harvestClient.BillableRates.CreateAsync(userId, new BillableRateCreationDto
{
    Amount = 150m,
    StartDate = new DateOnly(2026, 1, 1)
}, cancellationToken);

var costRates = await harvestClient.CostRates.ListAsync(userId, null, cancellationToken);
```

## Data Models

### BillableRate / CostRate
```csharp
public class BillableRate // CostRate has the same properties
{
    public long Id { get; set; }
    public decimal? Amount { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; } // calculated by Harvest; null for the current rate
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### BillableRatesContainer / CostRatesContainer
```csharp
public class BillableRatesContainer : ListContainerBase
{
    public List<BillableRate> BillableRates { get; set; } = []; // JSON key "billable_rates"
}

public class CostRatesContainer : ListContainerBase
{
    public List<CostRate> CostRates { get; set; } = []; // JSON key "cost_rates"
}
```
