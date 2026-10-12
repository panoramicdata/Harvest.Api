# Project Task Assignments API

The Project Task Assignments API provides access to task assignments for specific projects.

## Endpoints

### List Task Assignments for a Project

**GET** `/v2/projects/{id}/task_assignments`

Retrieves a list of all task assignments for a specific project.

**Parameters:**
- `id` (long): The ID of the project
- `options` (ListOptions?): Filtering and paging options; pass `null` to send no query parameters
  - `UpdatedSince` (DateTime?, sent as `updated_since`): Only return assignments that have been updated since the given date
  - `Page` (int?, sent as `page`): The page number to use in pagination (default: 1)
  - `PerPage` (int?, sent as `per_page`): The number of records to return per page (default: 100, range: 1-2000)

**Response:** `ProjectTaskAssignmentsContainer`
```csharp
Task<ProjectTaskAssignmentsContainer> ListAllAsync(
    long id,
    [Query] ListOptions? options,
    CancellationToken cancellationToken
);
```

## Implementation Status

| Method | Endpoint | Status | Notes |
|--------|----------|--------|-------|
| GET | `/v2/projects/{id}/task_assignments` | ✅ Implemented | List all task assignments for a project |
| GET | `/v2/projects/{projectId}/task_assignments/{id}` | ✅ Implemented | Individual task assignment retrieval |
| POST | `/v2/projects/{projectId}/task_assignments` | ✅ Implemented | Task assignment creation |
| PATCH | `/v2/projects/{projectId}/task_assignments/{id}` | ✅ Implemented | Task assignment update |
| DELETE | `/v2/projects/{projectId}/task_assignments/{id}` | ✅ Implemented | Task assignment deletion |

## Missing Functionality

All task assignment operations are now implemented in the Harvest API client.

## Usage Examples

```csharp
// List all task assignments for a project
var assignments = await harvestClient.ProjectTaskAssignments.ListAllAsync(12345, null, cancellationToken);
```

## Data Models

### ProjectTaskAssignment
```csharp
public class ProjectTaskAssignment
{
    public long Id { get; set; }
    public Task? Task { get; set; }
    public bool IsActive { get; set; }
    public bool Billable { get; set; }
    public decimal? HourlyRate { get; set; }
    public decimal? Budget { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### ProjectTaskAssignmentsContainer
```csharp
public class ProjectTaskAssignmentsContainer : ListContainerBase
{
    public List<ProjectTaskAssignment> ProjectTaskAssignments { get; set; } = [];
}
