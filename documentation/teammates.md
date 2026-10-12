# Teammates API

The Teammates API reads and replaces the teammates assigned to a manager
([Harvest documentation](https://help.getharvest.com/api-v2/users-api/users/teammates/)). Only Administrators may use it
(anyone else gets `403 Forbidden`), and the user must be a Manager (otherwise `422 Unprocessable Entity`).

Exposed as `harvestClient.Teammates` (`ITeammateApi`).

## Endpoints

### List a Manager's Teammates

**GET** `/v2/users/{userId}/teammates`

**Parameters:**
- `userId` (long): The ID of the manager
- `options` (ListOptions?): Paging options; pass `null` to send no query parameters. Harvest documents no filters.
  - `Cursor` (string?, sent as `cursor`): The cursor from the previous page's `Links.NextCursor`
  - `Page` (int?, sent as `page`): The page number to use in pagination (default: 1, deprecated by Harvest)
  - `PerPage` (int?, sent as `per_page`): The number of records to return per page (default: 2000, range: 1-2000)

**Response:** `TeammatesContainer`
```csharp
Task<TeammatesContainer> ListAsync(long userId, [Query] ListOptions? options, CancellationToken cancellationToken);
```

### Update a Manager's Teammates

**PATCH** `/v2/users/{userId}/teammates`

Replaces the manager's teammates: any teammate not listed is unassigned. Adding teammates for the first time gives the
manager the `people_manager` access role; an empty list unassigns everyone and removes that role.

**Body:** `TeammatePatchDto`
- `TeammateIds` (List&lt;long&gt;, required): Full list of user IDs to be assigned to the manager

**Response:** `TeammatesContainer` (the teammates now assigned; Harvest sends no paging fields)
```csharp
Task<TeammatesContainer> UpdateAsync(long userId, [Body] TeammatePatchDto patchDto, CancellationToken cancellationToken);
```

## Implementation Status

| Method | Endpoint | Status | Notes |
|--------|----------|--------|-------|
| GET | `/v2/users/{userId}/teammates` | ✅ Implemented | Paged list |
| PATCH | `/v2/users/{userId}/teammates` | ✅ Implemented | Replaces the full list |

## Usage Examples

```csharp
var teammates = await harvestClient.Teammates.ListAsync(managerId, null, cancellationToken);

await harvestClient.Teammates.UpdateAsync(managerId, new TeammatePatchDto { TeammateIds = [3230547, 3230575] }, cancellationToken);
```

## Data Models

### Teammate
```csharp
public class Teammate
{
    public long Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
}
```

### TeammatesContainer
```csharp
public class TeammatesContainer : ListContainerBase
{
    public List<Teammate> Teammates { get; set; } = []; // JSON key "teammates"
}
```
