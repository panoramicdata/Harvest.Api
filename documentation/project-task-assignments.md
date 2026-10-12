# Project Task Assignments API

The Project Task Assignments API manages which tasks are assigned to which projects
([Harvest documentation](https://help.getharvest.com/api-v2/projects-api/projects/task-assignments/)).

Exposed as `harvestClient.ProjectTaskAssignments` (`IProjectTaskAssignmentApi`).

## Endpoints

### List All Task Assignments

**GET** `/v2/task_assignments`

Retrieves one page of the task assignments of every project, most recently created first.

**Parameters:**
- `options` (ListOptions?): Filtering and paging options; pass `null` to send no query parameters
  - `IsActive` (bool?, sent as `is_active`): `true` for active assignments only, `false` for inactive ones
  - `UpdatedSince` (DateTimeOffset?, sent as `updated_since` in ISO 8601 UTC): Only return assignments updated since then
  - `Cursor` (string?, sent as `cursor`): The cursor from the previous page's `Links.NextCursor`
  - `Page` (int?, sent as `page`): The page number to use in pagination (default: 1, deprecated by Harvest)
  - `PerPage` (int?, sent as `per_page`): The number of records to return per page (default: 2000, range: 1-2000)

**Response:** `ProjectTaskAssignmentsContainer`
```csharp
Task<ProjectTaskAssignmentsContainer> ListAsync([Query] ListOptions? options, CancellationToken cancellationToken);
```

### List Task Assignments for a Project

**GET** `/v2/projects/{projectId}/task_assignments`

Retrieves one page of a project's task assignments. Takes the same options as the list above.

**Response:** `ProjectTaskAssignmentsContainer`
```csharp
Task<ProjectTaskAssignmentsContainer> ListAsync(
    long projectId,
    [Query] ListOptions? options,
    CancellationToken cancellationToken
);
```

## Implementation Status

| Method | Endpoint | Status | Notes |
|--------|----------|--------|-------|
| GET | `/v2/task_assignments` | ✅ Implemented | All projects, with `is_active` and `updated_since` filters |
| GET | `/v2/projects/{projectId}/task_assignments` | ✅ Implemented | List all task assignments for a project |
| GET | `/v2/projects/{projectId}/task_assignments/{id}` | ✅ Implemented | Individual task assignment retrieval |
| POST | `/v2/projects/{projectId}/task_assignments` | ✅ Implemented | Task assignment creation |
| PATCH | `/v2/projects/{projectId}/task_assignments/{id}` | ✅ Implemented | Task assignment update |
| DELETE | `/v2/projects/{projectId}/task_assignments/{id}` | ✅ Implemented | Task assignment deletion |

## Missing Functionality

All task assignment operations are now implemented in the Harvest API client.

## Usage Examples

```csharp
// List all task assignments for a project
var assignments = await harvestClient.ProjectTaskAssignments.ListAsync(12345, null, cancellationToken);

// Active task assignments across every project
var active = await harvestClient.ProjectTaskAssignments.ListAsync(new ListOptions { IsActive = true }, cancellationToken);
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
    public List<ProjectTaskAssignment> TaskAssignments { get; set; } = []; // JSON key "task_assignments"
}
