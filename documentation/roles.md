# Roles API

The Roles API manages the business roles that people are grouped by
([Harvest documentation](https://help.getharvest.com/api-v2/roles-api/roles/roles/)). Only Administrators may use it;
anyone else gets `403 Forbidden`.

Exposed as `harvestClient.Roles` (`IRoleApi`).

## Endpoints

### List All Roles

**GET** `/v2/roles`

Retrieves one page of roles, most recently created first.

**Parameters:**
- `options` (ListOptions?): Paging options; pass `null` to send no query parameters. Harvest documents no filters for roles.
  - `Cursor` (string?, sent as `cursor`): The cursor from the previous page's `Links.NextCursor`
  - `Page` (int?, sent as `page`): The page number to use in pagination (default: 1, deprecated by Harvest)
  - `PerPage` (int?, sent as `per_page`): The number of records to return per page (default: 2000, range: 1-2000)

**Response:** `RolesContainer`
```csharp
Task<RolesContainer> ListAsync([Query] ListOptions? options, CancellationToken cancellationToken);
```

### Get a Role

**GET** `/v2/roles/{id}`

```csharp
Task<Role> GetAsync(long id, CancellationToken cancellationToken);
```

### Create a Role

**POST** `/v2/roles`

**Body:** `RoleCreationDto`
- `Name` (string, required): The name of the role
- `UserIds` (List&lt;long&gt;?): The IDs of the users assigned to this role

```csharp
Task<Role> CreateAsync([Body] RoleCreationDto creationDto, CancellationToken cancellationToken);
```

### Update a Role

**PATCH** `/v2/roles/{id}`

**Body:** `RolePatchDto` (every field optional; `null` fields are left unchanged)
- `Name` (string?): The name of the role
- `UserIds` (List&lt;long&gt;?): Replaces the role's users entirely: anyone not listed is unassigned; an empty list
  unassigns everyone

```csharp
Task<Role> UpdateAsync(long id, [Body] RolePatchDto patchDto, CancellationToken cancellationToken);
```

### Delete a Role

**DELETE** `/v2/roles/{id}`

Deleting a role unlinks it from any users it was assigned to.

```csharp
System.Threading.Tasks.Task DeleteAsync(long id, CancellationToken cancellationToken);
```

## Implementation Status

| Method | Endpoint | Status | Notes |
|--------|----------|--------|-------|
| GET | `/v2/roles` | ✅ Implemented | Paged list |
| GET | `/v2/roles/{id}` | ✅ Implemented | Individual role retrieval |
| POST | `/v2/roles` | ✅ Implemented | Role creation |
| PATCH | `/v2/roles/{id}` | ✅ Implemented | Role update |
| DELETE | `/v2/roles/{id}` | ✅ Implemented | Role deletion |

## Usage Examples

```csharp
var role = await harvestClient.Roles.CreateAsync(new RoleCreationDto
{
    Name = "Marketing",
    UserIds = [3122374, 3122373]
}, cancellationToken);

// Replace the role's users with just one
await harvestClient.Roles.UpdateAsync(role.Id, new RolePatchDto { UserIds = [3122374] }, cancellationToken);
```

## Data Models

### Role
```csharp
public class Role
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public List<long>? UserIds { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### RolesContainer
```csharp
public class RolesContainer : ListContainerBase
{
    public List<Role> Roles { get; set; } = []; // JSON key "roles"
}
```
