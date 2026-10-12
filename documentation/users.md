# Users API

The Users API provides access to user management functionality in Harvest.

## Endpoints

### List All Users

**GET** `/v2/users`

Retrieves a list of all users for the authenticated account.

**Parameters:**
- `options` (ListOptions?): Filtering and paging options; pass `null` to send no query parameters
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

## Implementation Status

| Method | Endpoint | Status | Notes |
|--------|----------|--------|-------|
| GET | `/v2/users` | ✅ Implemented | Full support with pagination and filtering |
| GET | `/v2/users/{id}` | ✅ Implemented | Individual user retrieval |
| GET | `/v2/users/me` | ✅ Implemented | Current user information |
| POST | `/v2/users` | ⚠️ Implemented | User creation (may not be available in sandbox) |
| PATCH | `/v2/users/{id}` | ✅ Implemented | User update |
| DELETE | `/v2/users/{id}` | ⚠️ Implemented | User deletion (may not be available in sandbox) |

## Missing Functionality

User creation and deletion operations are implemented but may not be available in sandbox environments due to permission restrictions.

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
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Telephone { get; set; }
    public string Timezone { get; set; }
    public bool HasAccessToAllFutureProjects { get; set; }
    public bool IsContractor { get; set; }
    public bool IsActive { get; set; }
    public int WeeklyCapacity { get; set; }
    public decimal DefaultHourlyRate { get; set; }
    public decimal CostRate { get; set; }
    public List<string> Roles { get; set; }
    public string AvatarUrl { get; set; }
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
