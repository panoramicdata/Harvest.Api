# Project User Assignments API

The Project User Assignments API manages which users are assigned to which projects
([Harvest documentation](https://help.getharvest.com/api-v2/projects-api/projects/user-assignments/)). It needs an
Administrator, or a Manager with assigned teammates or managed projects; anyone else gets `403 Forbidden`.

Exposed as `harvestClient.ProjectUserAssignments` (`IProjectUserAssignmentApi`). For the read-only view of one user's
active assignments, see [User Project Assignments](user-project-assignments.md).

## Endpoints

### List All User Assignments

**GET** `/v2/user_assignments`

Retrieves one page of the user assignments of every project, active and archived, most recently created first.

**Parameters:**
- `options` (UserAssignmentListOptions?): Filtering and paging options; pass `null` to send no query parameters
  - `UserId` (long?, sent as `user_id`): Only return user assignments belonging to this user
  - `IsActive` (bool?, sent as `is_active`): `true` for active assignments only, `false` for inactive ones
  - `UpdatedSince` (DateTimeOffset?, sent as `updated_since` in ISO 8601 UTC): Only return assignments updated since then
  - `Cursor` (string?, sent as `cursor`): The cursor from the previous page's `Links.NextCursor`
  - `Page` (int?, sent as `page`): The page number to use in pagination (default: 1, deprecated by Harvest)
  - `PerPage` (int?, sent as `per_page`): The number of records to return per page (default: 2000, range: 1-2000)

```csharp
Task<UserAssignmentsContainer> ListAsync([Query] UserAssignmentListOptions? options, CancellationToken cancellationToken);
```

### List a Project's User Assignments

**GET** `/v2/projects/{projectId}/user_assignments`

Takes the same options as the list above.

```csharp
Task<UserAssignmentsContainer> ListAsync(long projectId, [Query] UserAssignmentListOptions? options, CancellationToken cancellationToken);
```

### Get a User Assignment

**GET** `/v2/projects/{projectId}/user_assignments/{id}`

```csharp
Task<UserAssignment> GetAsync(long projectId, long id, CancellationToken cancellationToken);
```

### Create a User Assignment

**POST** `/v2/projects/{projectId}/user_assignments`

**Body:** `UserAssignmentCreationDto`
- `UserId` (long, required): The ID of the user to associate with the project
- `IsActive` (bool?): Whether the assignment is active or archived (Harvest default: `true`)
- `IsProjectManager` (bool?): Whether the user has Project Manager permissions for the project (Harvest default: `false`
  for Regular Users, `true` for Project Managers and Administrators)
- `UseDefaultRates` (bool?): Use the user's default billable rates (`true`) or `HourlyRate` (`false`) when the project
  bills by People (Harvest default: `true`)
- `HourlyRate` (decimal?): Custom rate used when bill_by is People and use_default_rates is false (Harvest default: 0)
- `Budget` (decimal?): Budget used when the project's budget_by is person

```csharp
Task<UserAssignment> CreateAsync(long projectId, [Body] UserAssignmentCreationDto creationDto, CancellationToken cancellationToken);
```

### Update a User Assignment

**PATCH** `/v2/projects/{projectId}/user_assignments/{id}`

**Body:** `UserAssignmentPatchDto`: the same fields as creation except `UserId`, all optional; `null` fields are left
unchanged.

```csharp
Task<UserAssignment> UpdateAsync(long projectId, long id, [Body] UserAssignmentPatchDto patchDto, CancellationToken cancellationToken);
```

### Delete a User Assignment

**DELETE** `/v2/projects/{projectId}/user_assignments/{id}`

Only possible while the assignment has no time entries or expenses.

```csharp
System.Threading.Tasks.Task DeleteAsync(long projectId, long id, CancellationToken cancellationToken);
```

## Implementation Status

| Method | Endpoint | Status | Notes |
|--------|----------|--------|-------|
| GET | `/v2/user_assignments` | ✅ Implemented | All projects, with `user_id`, `is_active`, `updated_since` filters |
| GET | `/v2/projects/{projectId}/user_assignments` | ✅ Implemented | One project, same filters |
| GET | `/v2/projects/{projectId}/user_assignments/{id}` | ✅ Implemented | Individual assignment retrieval |
| POST | `/v2/projects/{projectId}/user_assignments` | ✅ Implemented | Assignment creation |
| PATCH | `/v2/projects/{projectId}/user_assignments/{id}` | ✅ Implemented | Assignment update |
| DELETE | `/v2/projects/{projectId}/user_assignments/{id}` | ✅ Implemented | Assignment deletion |

## Usage Examples

```csharp
// Every active assignment of one user, across all projects
var page = await harvestClient.ProjectUserAssignments.ListAsync(
    new UserAssignmentListOptions { UserId = 1782959, IsActive = true }, cancellationToken);

// Assign a user to a project at a custom rate
var assignment = await harvestClient.ProjectUserAssignments.CreateAsync(14308069, new UserAssignmentCreationDto
{
    UserId = 1782974,
    UseDefaultRates = false,
    HourlyRate = 75.50m
}, cancellationToken);
```

## Data Models

### UserAssignment
```csharp
public class UserAssignment
{
    public long Id { get; set; }
    public ProjectReference? Project { get; set; }
    public UserReference? User { get; set; }
    public bool IsActive { get; set; }
    public bool IsProjectManager { get; set; }
    public bool? UseDefaultRates { get; set; }
    public decimal? HourlyRate { get; set; }
    public decimal? Budget { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### UserAssignmentsContainer
```csharp
public class UserAssignmentsContainer : ListContainerBase
{
    public List<UserAssignment> UserAssignments { get; set; } = []; // JSON key "user_assignments"
}
```
