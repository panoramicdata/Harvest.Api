# User Project Assignments API

The User Project Assignments API lists the active project assignments of a user. Harvest documents only the two list
endpoints below; assignments are created, changed and deleted through a project's user assignments
(`/v2/projects/{projectId}/user_assignments`).

## Endpoints

### List Project Assignments for a User

**GET** `/v2/users/{userId}/project_assignments`

**Parameters:**
- `userId` (long): The ID of the user
- `options` (ListOptions?): Paging and `UpdatedSince`; pass `null` to send no query parameters

```csharp
Task<UserProjectAssignmentsContainer> ListAsync(long userId, [Query] ListOptions? options, CancellationToken cancellationToken);
```

### List the Current User's Project Assignments

**GET** `/v2/users/me/project_assignments`

```csharp
Task<UserProjectAssignmentsContainer> ListMineAsync([Query] ListOptions? options, CancellationToken cancellationToken);
```

## Implementation Status

| Method | Endpoint | Status |
|--------|----------|--------|
| GET | `/v2/users/{userId}/project_assignments` | ✅ Implemented |
| GET | `/v2/users/me/project_assignments` | ✅ Implemented |

Version 3.0 removed `GetAsync`, `CreateAsync`, `UpdateAsync` and `DeleteAsync` (and their DTOs): Harvest does not
document those endpoints.

## Usage Examples

```csharp
var myAssignments = await harvestClient.UserProjectAssignments.ListMineAsync(null, cancellationToken);

var userAssignments = await harvestClient.UserProjectAssignments.ListAsync(12345, new ListOptions { PerPage = 50 }, cancellationToken);
```

## Data Models

`UserProjectAssignment` carries `Id`, `IsActive`, `IsProjectManager`, `UseDefaultRates`, `HourlyRate`, `Budget`,
`CreatedAt`, `UpdatedAt`, `Project` (a `ProjectReference`: id, name, code), `Client` (a `ClientReference`) and
`TaskAssignments`. `UserProjectAssignmentsContainer.ProjectAssignments` holds the page (JSON key `project_assignments`).
