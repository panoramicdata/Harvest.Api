# Projects API

The Projects API provides access to project management functionality in Harvest.

## Endpoints

### List All Projects

**GET** `/v2/projects`

Retrieves a list of all projects for the authenticated account.

**Parameters:**
- `updated_since` (DateTime?, optional): Only return projects that have been updated since the given date
- `page` (int?, optional): The page number to use in pagination (default: 1)
- `per_page` (int?, optional): The number of records to return per page (default: 100, max: 100)

**Response:** `ProjectsContainer`
```csharp
Task<ProjectsContainer> ListAllAsync(
    DateTime? updatedSince = null,
    int? page = null,
    int? perPage = null
);
```

### Get a Project

**GET** `/v2/projects/{id}`

Retrieves a single project by ID.

**Parameters:**
- `id` (long): The ID of the project to retrieve

**Response:** `Project`
```csharp
Task<Project> GetAsync(long id);
```

## Implementation Status

| Method | Endpoint | Status | Notes |
|--------|----------|--------|-------|
| GET | `/v2/projects` | ✅ Implemented | Full support with pagination and filtering |
| GET | `/v2/projects/{id}` | ✅ Implemented | Individual project retrieval |
| POST | `/v2/projects` | ✅ Implemented | Project creation |
| PATCH | `/v2/projects/{id}` | ✅ Implemented | Project update |
| DELETE | `/v2/projects/{id}` | ✅ Implemented | Project deletion |

## Missing Functionality

All project management operations are now implemented in the Harvest API client.

## Usage Examples

```csharp
// List all projects
var projects = await harvestClient.Projects.ListAllAsync();

// Get a specific project
var project = await harvestClient.Projects.GetAsync(12345);
```

## Data Models

### Project
```csharp
public class Project
{
    public long Id { get; set; }
    public Client? Client { get; set; }
    public required string Name { get; set; }
    public string? Code { get; set; }
    public bool IsActive { get; set; }
    public bool IsBillable { get; set; }
    public bool IsFixedFee { get; set; }
    public required string BillBy { get; set; }
    public decimal? HourlyRate { get; set; }
    public decimal? Budget { get; set; }
    public required string BudgetBy { get; set; }
    public bool NotifyWhenOverBudget { get; set; }
    public decimal OverBudgetNotificationPercentage { get; set; }
    public DateTime? OverBudgetNotificationDate { get; set; }
    public bool ShowBudgetToAll { get; set; }
    public decimal? CostBudget { get; set; }
    public bool? CostBudgetIncludeExpenses { get; set; }
    public decimal? Fee { get; set; }
    public string? Notes { get; set; }
    public DateTime? StartsOn { get; set; }
    public DateTime? EndsOn { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### ProjectsContainer
```csharp
public class ProjectsContainer : ListContainerBase
{
    public List<Project> Projects { get; set; } = [];
}
