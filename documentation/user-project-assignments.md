# User Project Assignments API

The User Project Assignments API provides access to project assignments for specific users.

## Endpoints

### List Project Assignments for a User

**GET** `/v2/users/{id}/project_assignments`

Retrieves a list of all project assignments for a specific user.

**Parameters:**
- `id` (long): The ID of the user
- `options` (ListOptions?): Filtering and paging options; pass `null` to send no query parameters
  - `UpdatedSince` (DateTime?, sent as `updated_since`): Only return assignments that have been updated since the given date
  - `Page` (int?, sent as `page`): The page number to use in pagination (default: 1)
  - `PerPage` (int?, sent as `per_page`): The number of records to return per page (default: 100, range: 1-2000)

**Response:** `UserProjectAssignmentsContainer`
```csharp
Task<UserProjectAssignmentsContainer> ListAllAsync(
    long id,
    [Query] ListOptions? options,
    CancellationToken cancellationToken
);
```

### List Current User's Project Assignments

**GET** `/v2/users/me/project_assignments`

Retrieves a list of all project assignments for the currently authenticated user.

**Parameters:**
- `options` (ListOptions?): Filtering and paging options; pass `null` to send no query parameters
  - `UpdatedSince` (DateTime?, sent as `updated_since`): Only return assignments that have been updated since the given date
  - `Page` (int?, sent as `page`): The page number to use in pagination (default: 1)
  - `PerPage` (int?, sent as `per_page`): The number of records to return per page (default: 100, range: 1-2000)

**Response:** `UserProjectAssignmentsContainer`
```csharp
Task<UserProjectAssignmentsContainer> ListAllMineAsync([Query] ListOptions? options, CancellationToken cancellationToken);
```

## Implementation Status

| Method | Endpoint | Status | Notes |
|--------|----------|--------|-------|
| GET | `/v2/users/{id}/project_assignments` | ✅ Implemented | List assignments for specific user |
| GET | `/v2/users/me/project_assignments` | ✅ Implemented | List assignments for current user |
| GET | `/v2/users/{userId}/project_assignments/{id}` | ✅ Implemented | Individual assignment retrieval |
| POST | `/v2/users/{userId}/project_assignments` | ✅ Implemented | Assignment creation |
| PATCH | `/v2/users/{userId}/project_assignments/{id}` | ✅ Implemented | Assignment update |
| DELETE | `/v2/users/{userId}/project_assignments/{id}` | ✅ Implemented | Assignment deletion |

## Missing Functionality

All user project assignment operations are now implemented in the Harvest API client.

## Usage Examples

```csharp
// List project assignments for current user
var myAssignments = await harvestClient.UserProjectAssignments.ListAllMineAsync(null, cancellationToken);

// List project assignments for a specific user
var userAssignments = await harvestClient.UserProjectAssignments.ListAllAsync(12345, new ListOptions { Page = 1, PerPage = 50 }, cancellationToken);
```

## Data Models

### UserProjectAssignment
```csharp
public class UserProjectAssignment
{
    public long Id { get; set; }
    public bool IsActive { get; set; }
    public bool IsProjectManager { get; set; }
    public decimal? HourlyRate { get; set; }
    public decimal? Budget { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Project? Project { get; set; }
    public Client? Client { get; set; }
    public List<ProjectTaskAssignment>? TaskAssignments { get; set; }
}
```

### UserProjectAssignmentsContainer
```csharp
public class UserProjectAssignmentsContainer : ListContainerBase
{
    public List<UserProjectAssignment> ProjectAssignments { get; set; } = [];
}
