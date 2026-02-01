# Project Task Assignments API

The Project Task Assignments API provides access to task assignments for specific projects.

## Endpoints

### List Task Assignments for a Project

**GET** `/v2/projects/{id}/task_assignments`

Retrieves a list of all task assignments for a specific project.

**Parameters:**
- `id` (long): The ID of the project
- `updated_since` (DateTime?, optional): Only return assignments that have been updated since the given date
- `page` (int?, optional): The page number to use in pagination (default: 1)
- `per_page` (int?, optional): The number of records to return per page (default: 100, max: 100)

**Response:** `ProjectTaskAssignmentsContainer`
```csharp
Task<ProjectTaskAssignmentsContainer> ListAllAsync(
    long id,
    DateTime? updatedSince = null,
    int? page = null,
    int? perPage = null
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
var assignments = await harvestClient.ProjectTaskAssignments.ListAllAsync(12345);
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
