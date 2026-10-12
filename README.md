# Harvest.Api

A .NET wrapper for the [Harvest](https://www.getharvest.com/) time tracking API.

[![NuGet](https://img.shields.io/nuget/v/Harvest.svg)](https://www.nuget.org/packages/Harvest/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Installation

Install the package via NuGet:

```bash
dotnet add package Harvest
```

Or via the Package Manager Console:

```powershell
Install-Package Harvest
```

## Getting Started

### Authentication

To use the Harvest API, you'll need:
- An **Account ID** - Found in Harvest under Settings > Choose Integrations > Authorized OAuth2 API Clients
- An **Access Token** - Create a Personal Access Token in the same location

### Basic Usage

```csharp
using Harvest;

// Create a client
using var harvestClient = new HarvestClient(new HarvestClientOptions
{
    AccountId = 123456,
    AccessToken = "your-access-token",
    // Harvest asks every request to name the application and a contact; this is optional and defaults to
    // "Harvest.Api/<version> (+https://github.com/panoramicdata/Harvest.Api)".
    UserAgent = "MyApp (ops@example.com)"
});

// Every method takes a CancellationToken as its last argument
var cancellationToken = CancellationToken.None;

// Get current user
var me = await harvestClient.Users.GetMeAsync(cancellationToken);
Console.WriteLine($"Hello, {me.FirstName}!");

// List clients (null options: Harvest's defaults, first page of up to 2000)
var clientsContainer = await harvestClient.Clients.ListAsync(null, cancellationToken);
foreach (var client in clientsContainer.Clients)
{
    Console.WriteLine($"Client: {client.Name}");
}

// List one client's active projects updated in the last week
var projectsContainer = await harvestClient.Projects.ListAsync(
    new ProjectListOptions { ClientId = 5735776, IsActive = true, UpdatedSince = DateTimeOffset.UtcNow.AddDays(-7) },
    cancellationToken);

// List this week's time entries for one project
var timeEntries = await harvestClient.TimeEntries.ListAsync(
    new TimeEntryListOptions { ProjectId = 12345678, From = new DateOnly(2025, 1, 13), To = new DateOnly(2025, 1, 19) },
    cancellationToken);

// Create a time entry
var timeEntry = await harvestClient.TimeEntries.CreateAsync(new TimeEntryCreationDto
{
    ProjectId = 12345678,
    TaskId = 87654321,
    SpentDate = new DateOnly(2025, 1, 15),
    Hours = 2.5m,
    Notes = "Working on feature X"
}, cancellationToken);
```

### Paging

Each list call returns one page. Harvest prefers cursor paging: when the previous page's `Links.NextCursor` is set, pass
it as `ListOptions.Cursor`; otherwise pass `NextPage` as `ListOptions.Page`. Stop when both are `null`.

```csharp
var all = new List<TimeEntry>();
var options = new TimeEntryListOptions();
while (true)
{
    var page = await harvestClient.TimeEntries.ListAsync(options, cancellationToken);
    all.AddRange(page.TimeEntries);
    var cursor = page.Links?.NextCursor;
    var nextPage = cursor is null ? page.NextPage : null;
    if (cursor is null && nextPage is null)
    {
        break;
    }

    options = new TimeEntryListOptions { Cursor = cursor, Page = nextPage };
}
```

### Errors and retries

A non-success response raises `HarvestApiException`, a Refit `ApiException` whose `Message` and `ErrorMessage` carry
Harvest's own error text (`StatusCode`, `Content` and the response headers are available too). Throttled requests
(HTTP 429, honouring `Retry-After`) and transient server errors are retried first, up to
`HarvestClientOptions.MaxRetries` times (3 by default; set 0 to disable).

### Upgrading from 2.x

Version 3.0 makes breaking changes:
- Every method takes a required `CancellationToken` as its last parameter.
- List methods are named `ListAsync` (`ListMineAsync` for the current user's project assignments) and take a
  `ListOptions` object (`TimeEntryListOptions`, `ProjectListOptions`) in place of the optional `updatedSince`, `page`,
  `perPage` and filter parameters. Pass `null` for none. `TimeEntries.PatchAsync` is now `UpdateAsync`.
- Dates are `DateOnly` (`SpentDate`, `StartsOn`, `From`, ...); `UpdatedSince` is a `DateTimeOffset` sent in ISO 8601.
- Nested objects are reference types (`UserReference`, `ClientReference`, `ProjectReference`, `TaskReference`,
  `InvoiceReference`), and models follow the current Harvest v2 field lists (v1-only fields were removed).
- Optional fields of creation DTOs are nullable and omitted when unset, so Harvest applies its own defaults.
- The undocumented user project assignment `Get`/`Create`/`Update`/`Delete` methods were removed.
- Errors raise `HarvestApiException` (still a Refit `ApiException`).

```csharp
// 2.x
await harvestClient.Clients.ListAllAsync(page: 2, perPage: 50);
// 3.x
await harvestClient.Clients.ListAsync(new ListOptions { Page = 2, PerPage = 50 }, cancellationToken);
```

## Available APIs

The `HarvestClient` (and `IHarvestClient`) provides access to the following API endpoints:

| Property | Description |
|----------|-------------|
| `Clients` | Manage clients |
| `Companies` | Access company information |
| `Contacts` | Manage client contacts |
| `ExpenseCategories` | Manage expense categories |
| `Expenses` | Manage expenses and their receipts |
| `Projects` | Manage projects |
| `ProjectTaskAssignments` | Manage task assignments for projects |
| `Reports` | Time, expense, uninvoiced and project budget reports (100 requests per 15 minutes) |
| `Tasks` | Manage tasks |
| `TimeEntries` | Create, update, and delete time entries |
| `Users` | Manage users |
| `UserProjectAssignments` | List project assignments for users |

Contributors adding resources should follow [docs/IMPLEMENTING.md](docs/IMPLEMENTING.md).

## Proxy Support

If you need to route requests through a proxy:

```csharp
var harvestClient = new HarvestClient(new HarvestClientOptions
{
    AccountId = 123456,
    AccessToken = "your-access-token",
    ProxyUrl = "http://proxy.example.com:8080"
});
```

To customise the transport further, pass your own `HttpMessageHandler` as the second constructor argument.

## API Documentation

For full API documentation, refer to the official [Harvest API v2 Documentation](https://help.getharvest.com/api-v2/).

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License.

## Acknowledgments

This library was forked from projects by:
- Joel Potter
- Data Research Group
- Paul Irwin

Maintained by [Panoramic Data Limited](https://www.intopanoramicdata.com/).
