# Projects API

The Projects API provides access to project management functionality in Harvest.

## Endpoints

### List All Projects

**GET** `/v2/projects`

Retrieves a list of all projects for the authenticated account.

**Parameters:**
- `options` (ProjectListOptions?): Filtering and paging options (adds `ClientId`, sent as `client_id`); pass `null` to send no query parameters
  - `UpdatedSince` (DateTimeOffset?, sent as `updated_since` in ISO 8601 UTC): Only return projects that have been updated since the given date
  - `Page` (int?, sent as `page`): The page number to use in pagination (default: 1)
  - `PerPage` (int?, sent as `per_page`): The number of records to return per page (default: 2000, range: 1-2000)

**Response:** `ProjectsContainer`
```csharp
Task<ProjectsContainer> ListAsync([Query] ProjectListOptions? options, CancellationToken cancellationToken);
```

### Get a Project

**GET** `/v2/projects/{id}`

Retrieves a single project by ID.

**Parameters:**
- `id` (long): The ID of the project to retrieve

**Response:** `Project`
```csharp
Task<Project> GetAsync(long id, CancellationToken cancellationToken);
```

### Create a Project

**POST** `/v2/projects`

Creates a project. `ClientId`, `Name`, `IsBillable`, `BillBy` and `BudgetBy` are required; optional fields left `null`
take Harvest's defaults.

**Response:** `Project`
```csharp
Task<Project> CreateAsync([Body] ProjectCreationDto creationDto, CancellationToken cancellationToken);
```

### Update a Project

**PATCH** `/v2/projects/{id}`

Updates a project; fields left `null` are unchanged.

**Response:** `Project`
```csharp
Task<Project> UpdateAsync(long id, [Body] ProjectPatchDto patchDto, CancellationToken cancellationToken);
```

### Delete a Project

**DELETE** `/v2/projects/{id}`

Deletes a project and any time entries or expenses tracked to it (its invoices are kept). Archive it instead
(`IsActive = false`) to keep them.

```csharp
System.Threading.Tasks.Task DeleteAsync(long id, CancellationToken cancellationToken);
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
var projects = await harvestClient.Projects.ListAsync(null, cancellationToken);

// List projects updated in the last week
var recentProjects = await harvestClient.Projects.ListAsync(new ProjectListOptions { UpdatedSince = DateTime.UtcNow.AddDays(-7) }, cancellationToken);

// Get a specific project
var project = await harvestClient.Projects.GetAsync(12345, cancellationToken);
```

## Data Models

### Project
```csharp
public class Project
{
    public long Id { get; set; }
    public ClientReference? Client { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public bool IsActive { get; set; }
    public bool IsBillable { get; set; }
    public bool IsFixedFee { get; set; }
    public string? BillBy { get; set; }
    public decimal? HourlyRate { get; set; }
    public string? BudgetBy { get; set; }
    public bool BudgetIsMonthly { get; set; }
    public decimal? Budget { get; set; }
    public decimal? CostBudget { get; set; }
    public bool? CostBudgetIncludeExpenses { get; set; }
    public bool NotifyWhenOverBudget { get; set; }
    public decimal? OverBudgetNotificationPercentage { get; set; }
    public DateOnly? OverBudgetNotificationDate { get; set; }
    public bool ShowBudgetToAll { get; set; }
    public decimal? Fee { get; set; }
    public string? Notes { get; set; }
    public DateOnly? StartsOn { get; set; }
    public DateOnly? EndsOn { get; set; }
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
