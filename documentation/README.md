# Harvest API v2 Documentation

This folder documents the Harvest API v2 endpoints implemented by this .NET client library: for each resource, the
routes, the C# method signatures, the options and DTOs, and usage examples.

## Overview

The Harvest API v2 provides RESTful access to Harvest's time tracking, project, invoicing and time off features. Every
resource is a property of `HarvestClient` / `IHarvestClient`; see [Harvest's own documentation](https://help.getharvest.com/api-v2/)
for the full field descriptions.

## Implemented APIs

| API | Client properties | Description | Status |
|-----|-------------------|-------------|--------|
| [Accounts](accounts.md) | `Accounts` | Accounts reachable by the token (Harvest ID) | ✅ **Complete** |
| [Clients](clients.md) | `Clients` | Client management | ✅ **Complete** |
| [Company](company.md) | `Companies` | Company information and settings | ✅ **Complete** |
| [Contacts](contacts.md) | `Contacts` | Client contact management | ✅ **Complete** |
| [Estimates](estimates.md) | `Estimates`, `EstimateMessages`, `EstimateItemCategories` | Estimates, line items, estimate messages and item categories | ✅ **Complete** |
| [Expense Categories](expense-categories.md) | `ExpenseCategories` | Expense category management | ✅ **Complete** |
| [Expenses](expenses.md) | `Expenses` | Expense management, with receipts | ✅ **Complete** |
| [Invoice Item Categories](invoice-item-categories.md) | `InvoiceItemCategories` | Invoice line item categories | ✅ **Complete** |
| [Invoice Messages](invoice-messages.md) | `InvoiceMessages` | Sending invoices and changing their state | ✅ **Complete** |
| [Invoice Payments](invoice-payments.md) | `InvoicePayments` | Invoice payments | ✅ **Complete** |
| [Invoices](invoices.md) | `Invoices` | Invoices, their line items, and time and expense import | ✅ **Complete** |
| [Project Task Assignments](project-task-assignments.md) | `ProjectTaskAssignments` | Task assignments, across projects and per project | ✅ **Complete** |
| [Project User Assignments](project-user-assignments.md) | `ProjectUserAssignments` | User assignments, across projects and per project | ✅ **Complete** |
| [Projects](projects.md) | `Projects` | Project management | ✅ **Complete** |
| [PTO (Time Off)](pto.md) | `PtoAllocations`, `PtoAssignments`, `PtoBalances`, `PtoHolidayCalendars`, `PtoRequests`, `PtoTypes`, `PtoWorkSchedules` | Time off requests, policies, balances, holiday calendars, work schedules, assignments and allocations | ✅ **Complete** |
| [Reports](reports.md) | `Reports` | Time, expense, uninvoiced and project budget reports | ✅ **Complete** |
| [Roles](roles.md) | `Roles` | Role management | ✅ **Complete** |
| [Tasks](tasks.md) | `Tasks` | Task management | ✅ **Complete** |
| [Teammates](teammates.md) | `Teammates` | Teammates of managers | ✅ **Complete** |
| [Time Entries](time-entries.md) | `TimeEntries` | Time entries, timers and external references | ✅ **Complete** |
| [User Billable and Cost Rates](user-rates.md) | `BillableRates`, `CostRates` | Dated billable and cost rates per user | ✅ **Complete** |
| [User Project Assignments](user-project-assignments.md) | `UserProjectAssignments` | A user's (or your own) project assignments | ✅ **Complete** |
| [Users](users.md) | `Users` | User management, including the current user | ✅ **Complete** |

## Coverage

Every operation documented in the Harvest API v2 reference (including the gradually rolled-out PTO API and the
Harvest ID accounts endpoint) is implemented: 32 resource interfaces on `IHarvestClient` with 149 public methods
between them (plus two hidden `multipart/form-data` helpers behind the expense receipt overloads).

- **Unit tests** (`Harvest.UnitTest`): every method's request (verb, path, query and exact JSON body) is pinned against a
  stub handler, and every response model is read from the sample in Harvest's documentation. Line and branch coverage
  of the library is 100%. These tests never touch the network.
- **Live verification is pending.** `Harvest.Test` runs against a real Harvest account and so far covers only clients,
  company, projects, task assignments, tasks, time entries, user project assignments and users. The newer resources
  (estimates, expenses, invoices, reports, roles, rates, PTO and the rest) have not yet been exercised against the live
  API; field lists follow Harvest's documentation, which may differ in detail from what the API sends.

## Live Test Data Management

The live suite (`Harvest.Test`) includes a `TestDataManager` class that:

- **Configurable Prefixes**: prefixes all test data with a configurable string (default: "TEST_")
- **Dependency Management**: creates required dependencies (clients → projects → tasks → time entries)
- **Graceful Fallbacks**: falls back to existing data when creation fails
- **Cleanup**: tracks and cleans up the data it created
- **Configuration**: is controlled via the `appsettings.json` `TestSettings` section

### Test Settings Configuration

```json
{
  "TestSettings": {
    "TestDataPrefix": "TEST_",
    "CreateTestData": true,
    "CleanupTestData": true
  }
}
```

The live suite changes real data in the configured account: never point it at a production account you care about.

## Contributing

To add or change a resource, follow [docs/IMPLEMENTING.md](../docs/IMPLEMENTING.md), and keep the resource's page here
in step with its interface.
