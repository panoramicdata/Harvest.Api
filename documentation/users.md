# Users API

The Users API provides access to user management functionality in Harvest.

## Endpoints

### List All Users

**GET** `/v2/users`

Retrieves a list of all users for the authenticated account.

**Parameters:**
- `options` (ListOptions?): Filtering and paging options; pass `null` to send no query parameters
  - `IsActive` (bool?, sent as `is_active`): `true` for active users only, `false` for archived ones
  - `UpdatedSince` (DateTimeOffset?, sent as `updated_since` in ISO 8601 UTC): Only return users that have been updated since the given date
  - `Page` (int?, sent as `page`): The page number to use in pagination (default: 1)
  - `PerPage` (int?, sent as `per_page`): The number of records to return per page (default: 2000, range: 1-2000)

**Response:** `UsersContainer`
```csharp
Task<UsersContainer> ListAsync([Query] ListOptions? options, CancellationToken cancellationToken);
```

### Get a User

**GET** `/v2/users/{id}`

Retrieves a single user by ID.

**Parameters:**
- `id` (long): The ID of the user to retrieve

**Response:** `User`
```csharp
Task<User> GetAsync(long id, CancellationToken cancellationToken);
```

### Get Current User

**GET** `/v2/users/me`

Retrieves information about the currently authenticated user.

**Parameters:** None

**Response:** `User`
```csharp
Task<User> GetMeAsync(CancellationToken cancellationToken);
```

### Create a User

**POST** `/v2/users`

Creates a user. `FirstName`, `LastName` and `Email` are required; optional fields left
`null` take Harvest's defaults.

**Response:** `User`
```csharp
Task<User> CreateAsync([Body] UserCreationDto creationDto, CancellationToken cancellationToken);
```

### Update a User

**PATCH** `/v2/users/{id}`

Updates a user; fields left `null` are unchanged.

**Response:** `User`
```csharp
Task<User> UpdateAsync(long id, [Body] UserPatchDto patchDto, CancellationToken cancellationToken);
```

### Delete a User

**DELETE** `/v2/users/{id}`

Deletes a user. Harvest refuses to delete a user with time entries or expenses; archive them instead (`IsActive = false`).

```csharp
System.Threading.Tasks.Task DeleteAsync(long id, CancellationToken cancellationToken);
```

## Implementation Status

| Method | Endpoint | Status | Notes |
|--------|----------|--------|-------|
| GET | `/v2/users` | ✅ Implemented | Full support with pagination and filtering |
| GET | `/v2/users/{id}` | ✅ Implemented | Individual user retrieval |
| GET | `/v2/users/me` | ✅ Implemented | Current user information |
| POST | `/v2/users` | ✅ Implemented | User creation |
| PATCH | `/v2/users/{id}` | ✅ Implemented | User update |
| DELETE | `/v2/users/{id}` | ✅ Implemented | User deletion |

## Missing Functionality

None: every documented Users operation is implemented. Billable rates, cost rates, teammates and project assignments
have their own pages ([user-rates.md](user-rates.md), [teammates.md](teammates.md),
[user-project-assignments.md](user-project-assignments.md)).

## Usage Examples

```csharp
// List all users
var users = await harvestClient.Users.ListAsync(null, cancellationToken);

// Get current user
var me = await harvestClient.Users.GetMeAsync(cancellationToken);

// Get a specific user
var user = await harvestClient.Users.GetAsync(12345, cancellationToken);
```

## Data Models

### User
```csharp
public class User
{
    public long Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Telephone { get; set; }
    public string? Timezone { get; set; }
    public bool HasAccessToAllFutureProjects { get; set; }
    public bool IsContractor { get; set; }
    public bool IsActive { get; set; }
    public bool? SamlExempt { get; set; }
    public int? WeeklyCapacity { get; set; }
    public decimal? DefaultHourlyRate { get; set; }
    public decimal? CostRate { get; set; }
    public List<string>? Roles { get; set; }
    public List<string>? AccessRoles { get; set; }
    public string? PermissionsProfile { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### UsersContainer
```csharp
public class UsersContainer : ListContainerBase
{
    public List<User> Users { get; set; } = [];
}
