# Company API

The Company API provides access to company-level information and settings.

## Endpoints

### Get Company Information

**GET** `/v2/company`

Retrieves information about the company associated with the authenticated account.

**Parameters:** None

**Response:** `Company`
```csharp
Task<Company> GetAsync(CancellationToken cancellationToken);
```

### Update the Company

**PATCH** `/v2/company`

Updates the company's settings; `null` fields are left unchanged.

**Body:** `CompanyPatchDto` (Harvest documents only these two settings as changeable)
- `WantsTimestampTimers` (bool?): Whether time is tracked via duration (`false`) or start and end times (`true`)
- `WeeklyCapacity` (int?): The weekly capacity in seconds

**Response:** `Company`
```csharp
Task<Company> UpdateAsync([Body] CompanyPatchDto patchDto, CancellationToken cancellationToken);
```

## Implementation Status

| Method | Endpoint | Status | Notes |
|--------|----------|--------|-------|
| GET | `/v2/company` | ✅ Implemented | Full company information retrieval |
| PATCH | `/v2/company` | ✅ Implemented | `wants_timestamp_timers` and `weekly_capacity` |

## Usage Examples

```csharp
// Get company information
var company = await harvestClient.Companies.GetAsync(cancellationToken);
Console.WriteLine($"Company: {company.Name}");

// Switch to start and end times with a 30-hour week
await harvestClient.Companies.UpdateAsync(new CompanyPatchDto { WantsTimestampTimers = true, WeeklyCapacity = 108000 }, cancellationToken);
```

## Data Models

### Company
```csharp
public class Company
{
    public string? BaseUri { get; set; }
    public string? FullDomain { get; set; }
    public string? Name { get; set; }
    public bool IsActive { get; set; }
    public DayOfWeek WeekStartDay { get; set; }
    public bool WantsTimestampTimers { get; set; }
    public string? TimeFormat { get; set; }
    public string? PlanType { get; set; }
    public string? Clock { get; set; }
    public string? DecimalSymbol { get; set; }
    public string? ThousandsSeparator { get; set; }
    public string? ColorScheme { get; set; }
    public bool ExpenseFeature { get; set; }
    public bool InvoiceFeature { get; set; }
    public bool EstimateFeature { get; set; }
    public bool ApprovalFeature { get; set; }
}
```

## Notes

- The Company API returns a single company object, not a collection
- This endpoint provides configuration and feature flags for the account
- Some properties may be null depending on account settings and plan type
