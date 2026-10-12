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
var harvestClient = new HarvestClient(new HarvestClientOptions
{
    AccountId = 123456,
    AccessToken = "your-access-token"
});

// Every method takes a CancellationToken as its last argument
var cancellationToken = CancellationToken.None;

// Get current user
var me = await harvestClient.Users.GetMeAsync(cancellationToken);
Console.WriteLine($"Hello, {me.FirstName}!");

// List clients (null options: Harvest's defaults, first page of 100)
var clientsContainer = await harvestClient.Clients.ListAllAsync(null, cancellationToken);
foreach (var client in clientsContainer.Clients)
{
    Console.WriteLine($"Client: {client.Name}");
}

// List projects, with paging and filtering through ListOptions
var projectsContainer = await harvestClient.Projects.ListAllAsync(
    new ListOptions { Page = 1, PerPage = 50, UpdatedSince = DateTime.UtcNow.AddDays(-7) },
    cancellationToken);
foreach (var project in projectsContainer.Projects)
{
    Console.WriteLine($"Project: {project.Name}");
}

// List this week's time entries for one project
var timeEntries = await harvestClient.TimeEntries.ListAllAsync(
    new TimeEntryListOptions { ProjectId = 12345678, From = "2025-01-13", To = "2025-01-19" },
    cancellationToken);

// Create a time entry
var timeEntry = await harvestClient.TimeEntries.CreateAsync(new TimeEntryCreationDto
{
    ProjectId = 12345678,
    TaskId = 87654321,
    SpentDate = "2025-01-15",
    Hours = 2.5m,
    Notes = "Working on feature X"
}, cancellationToken);
```

### Upgrading from 2.x

Version 3.0 makes two breaking changes:
- Every method takes a required `CancellationToken` as its last parameter.
- List methods take a `ListOptions` object (`TimeEntryListOptions` for time entries) in place of the optional `updatedSince`, `page`, `perPage` and filter parameters. Pass `null` for none.

```csharp
// 2.x
await harvestClient.Clients.ListAllAsync(page: 2, perPage: 50);
// 3.x
await harvestClient.Clients.ListAllAsync(new ListOptions { Page = 2, PerPage = 50 }, cancellationToken);
```

## Available APIs

The `HarvestClient` provides access to the following API endpoints:

| Property | Description |
|----------|-------------|
| `Clients` | Manage clients |
| `Companies` | Access company information |
| `Projects` | Manage projects |
| `ProjectTaskAssignments` | Manage task assignments for projects |
| `Tasks` | Manage tasks |
| `TimeEntries` | Create, update, and delete time entries |
| `Users` | Manage users |
| `UserProjectAssignments` | Manage project assignments for users |

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
