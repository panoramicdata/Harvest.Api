# Reports API

The Reports API provides Harvest's aggregate reports: time and expense totals by client, project, task or expense
category and team member, the uninvoiced report, and the project budget report. All of them are on one interface,
`IReportApi`, exposed as `harvestClient.Reports`. Each method returns one page of results in a
`ReportResultsContainer<TResult>`.

Harvest documentation: <https://help.getharvest.com/api-v2/reports-api/reports/time-reports/>

## Rate limit

Reports have their own, much stricter rate limit: **100 requests per 15 minutes** (general API requests allow 100 per
15 seconds). A throttled request is answered `429 Too Many Requests` with a `Retry-After` header; the client honours it
when retrying (`HarvestClientOptions.MaxRetries`), so a burst of report requests can stall for minutes. Cache report
results rather than polling them, and prefer a large `PerPage` (up to 2000, the default) to fewer pages.

## Timeframe and date format

Every report except the project budget report requires a timeframe: `From` and `To` are C# `required` properties of the
options type, so the options object itself is required (not nullable). The timeframe cannot exceed one year (365 days).

Dates are sent in the compact `YYYYMMDD` form that Harvest's Reports API examples use (`from=20170101&to=20171231`),
not the `YYYY-MM-DD` form used by the other list endpoints.

## Paging

Reports are paged by page number only (there is no cursor): pass the previous response's `NextPage` as `Page` until it
is `null`.

## Endpoints

### Time Reports

| Method | Endpoint | Options |
|--------|----------|---------|
| `GetClientTimeReportAsync` | **GET** `/v2/reports/time/clients` | `TimeReportOptions` |
| `GetProjectTimeReportAsync` | **GET** `/v2/reports/time/projects` | `TimeReportWithForecastOptions` |
| `GetTaskTimeReportAsync` | **GET** `/v2/reports/time/tasks` | `TimeReportOptions` |
| `GetTeamTimeReportAsync` | **GET** `/v2/reports/time/team` | `TimeReportWithForecastOptions` |

**Parameters:**
- `From` (DateOnly, required, sent as `from` in `YYYYMMDD`): Only report on time entries with a `spent_date` on or after the given date
- `To` (DateOnly, required, sent as `to` in `YYYYMMDD`): Only report on time entries with a `spent_date` on or before the given date
- `IncludeFixedFee` (bool?, sent as `include_fixed_fee`): When `true`, billable amounts will be calculated and included for fixed fee projects
- `IncludeForecast` (bool?, sent as `include_forecast`; projects and team reports only): When `true`, scheduled hours from Forecast are included (`ScheduledHours`). Requires the account to be connected to Forecast
- `Page` (int?, sent as `page`): The page number to use in pagination (default: 1)
- `PerPage` (int?, sent as `per_page`): The number of records to return per page (default: 2000, range: 1-2000)

**Response:** `ReportResultsContainer<TimeReportResult>`
```csharp
Task<ReportResultsContainer<TimeReportResult>> GetClientTimeReportAsync([Query] TimeReportOptions options, CancellationToken cancellationToken);
Task<ReportResultsContainer<TimeReportResult>> GetProjectTimeReportAsync([Query] TimeReportWithForecastOptions options, CancellationToken cancellationToken);
Task<ReportResultsContainer<TimeReportResult>> GetTaskTimeReportAsync([Query] TimeReportOptions options, CancellationToken cancellationToken);
Task<ReportResultsContainer<TimeReportResult>> GetTeamTimeReportAsync([Query] TimeReportWithForecastOptions options, CancellationToken cancellationToken);
```

### Expense Reports

| Method | Endpoint | Options |
|--------|----------|---------|
| `GetClientExpenseReportAsync` | **GET** `/v2/reports/expenses/clients` | `ReportOptions` |
| `GetProjectExpenseReportAsync` | **GET** `/v2/reports/expenses/projects` | `ReportOptions` |
| `GetExpenseCategoryExpenseReportAsync` | **GET** `/v2/reports/expenses/categories` | `ReportOptions` |
| `GetTeamExpenseReportAsync` | **GET** `/v2/reports/expenses/team` | `ReportOptions` |

**Parameters:** `From`, `To` (required), `Page`, `PerPage`, as above.

**Response:** `ReportResultsContainer<ExpenseReportResult>`
```csharp
Task<ReportResultsContainer<ExpenseReportResult>> GetClientExpenseReportAsync([Query] ReportOptions options, CancellationToken cancellationToken);
```

### Uninvoiced Report

**GET** `/v2/reports/uninvoiced`

The uninvoiced hours and expenses for all billable projects in the timeframe. Administrators see all projects; Managers
with permission to create invoices see only the projects they manage.

**Parameters:**
- `From`, `To` (required), `Page`, `PerPage`, as above
- `IncludeFixedFee` (bool?, sent as `include_fixed_fee`): Whether to include fixed-fee projects (Harvest's default: `true`). A fixed-fee project's uninvoiced fee shows as long as the timeframe is on or after the project start date (or creation date); otherwise it is 0

**Response:** `ReportResultsContainer<UninvoicedReportResult>`
```csharp
Task<ReportResultsContainer<UninvoicedReportResult>> GetUninvoicedReportAsync([Query] UninvoicedReportOptions options, CancellationToken cancellationToken);
```

### Project Budget Report

**GET** `/v2/reports/project_budget`

Budget information for projects that have been assigned a budget. It takes no timeframe, so its options are optional.

**Parameters:**
- `options` (ProjectBudgetReportOptions?): pass `null` to send no query parameters
  - `IsActive` (bool?, sent as `is_active`): `true` to only return active projects, `false` for inactive projects
  - `Page` (int?, sent as `page`), `PerPage` (int?, sent as `per_page`), as above

**Response:** `ReportResultsContainer<ProjectBudgetReportResult>`
```csharp
Task<ReportResultsContainer<ProjectBudgetReportResult>> GetProjectBudgetReportAsync([Query] ProjectBudgetReportOptions? options, CancellationToken cancellationToken);
```

## Implementation Status

| Method | Endpoint | Status | Notes |
|--------|----------|--------|-------|
| GET | `/v2/reports/time/clients` | ✅ Implemented | `include_fixed_fee` |
| GET | `/v2/reports/time/projects` | ✅ Implemented | `include_fixed_fee`, `include_forecast` |
| GET | `/v2/reports/time/tasks` | ✅ Implemented | `include_fixed_fee` |
| GET | `/v2/reports/time/team` | ✅ Implemented | `include_fixed_fee`, `include_forecast` |
| GET | `/v2/reports/expenses/clients` | ✅ Implemented | |
| GET | `/v2/reports/expenses/projects` | ✅ Implemented | |
| GET | `/v2/reports/expenses/categories` | ✅ Implemented | |
| GET | `/v2/reports/expenses/team` | ✅ Implemented | |
| GET | `/v2/reports/uninvoiced` | ✅ Implemented | `include_fixed_fee` |
| GET | `/v2/reports/project_budget` | ✅ Implemented | `is_active` |

## Missing Functionality

None: every documented Reports API operation and parameter is implemented.

## Usage Examples

```csharp
// Hours per project for 2017, with Forecast's scheduled hours
var projects = await harvestClient.Reports.GetProjectTimeReportAsync(new TimeReportWithForecastOptions
{
    From = new DateOnly(2017, 1, 1),
    To = new DateOnly(2017, 12, 31),
    IncludeForecast = true
}, cancellationToken);

// Every page of the uninvoiced report (mind the 100 requests per 15 minutes limit)
var results = new List<UninvoicedReportResult>();
int? page = 1;
while (page is not null)
{
    var response = await harvestClient.Reports.GetUninvoicedReportAsync(new UninvoicedReportOptions
    {
        From = new DateOnly(2017, 1, 1),
        To = new DateOnly(2017, 12, 31),
        Page = page
    }, cancellationToken);
    results.AddRange(response.Results);
    page = response.NextPage;
}

// Budgets of active projects
var budgets = await harvestClient.Reports.GetProjectBudgetReportAsync(new ProjectBudgetReportOptions { IsActive = true }, cancellationToken);
```

## Data Models

### ReportResultsContainer&lt;TResult&gt;
```csharp
public class ReportResultsContainer<TResult> : ListContainerBase
{
    public List<TResult> Results { get; set; } = [];
}
```

### TimeReportResult
Subject fields are set according to the report: client and project fields in the clients and projects reports, task
fields in the tasks report, user fields in the team report.
```csharp
public class TimeReportResult
{
    public long? ClientId { get; set; }
    public string? ClientName { get; set; }
    public long? ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public long? TaskId { get; set; }
    public string? TaskName { get; set; }
    public long? UserId { get; set; }
    public string? UserName { get; set; }
    public int? WeeklyCapacity { get; set; }      // seconds
    public string? AvatarUrl { get; set; }
    public bool? IsContractor { get; set; }
    public decimal TotalHours { get; set; }
    public decimal BillableHours { get; set; }
    public string? Currency { get; set; }          // needs the View billable rates and amounts permission
    public decimal? BillableAmount { get; set; }   // needs the View billable rates and amounts permission
    public decimal? ScheduledHours { get; set; }   // include_forecast=true, projects and team reports only
}
```

### ExpenseReportResult
```csharp
public class ExpenseReportResult
{
    public long? ClientId { get; set; }
    public string? ClientName { get; set; }
    public long? ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public long? ExpenseCategoryId { get; set; }
    public string? ExpenseCategoryName { get; set; }
    public long? UserId { get; set; }
    public string? UserName { get; set; }
    public bool? IsContractor { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal BillableAmount { get; set; }
    public string? Currency { get; set; }
}
```

### UninvoicedReportResult
```csharp
public class UninvoicedReportResult
{
    public long ClientId { get; set; }
    public string? ClientName { get; set; }
    public long ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public string? Currency { get; set; }
    public decimal TotalHours { get; set; }
    public decimal UninvoicedHours { get; set; }
    public decimal UninvoicedExpenses { get; set; }
    public decimal UninvoicedAmount { get; set; }
}
```

### ProjectBudgetReportResult
```csharp
public class ProjectBudgetReportResult
{
    public long ClientId { get; set; }
    public string? ClientName { get; set; }
    public long ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public bool BudgetIsMonthly { get; set; }
    public string? BudgetBy { get; set; }          // project, project_cost, task, task_fees, person, none
    public bool IsActive { get; set; }
    public decimal? Budget { get; set; }
    public decimal? BudgetSpent { get; set; }
    public decimal? BudgetRemaining { get; set; }
}
```
