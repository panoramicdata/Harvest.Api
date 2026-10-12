# Tasks API

The Tasks API provides access to task management functionality in Harvest.

## Endpoints

### List All Tasks

**GET** `/v2/tasks`

Retrieves a list of all tasks for the authenticated account.

**Parameters:**
- `options` (ListOptions?): Filtering and paging options; pass `null` to send no query parameters
  - `UpdatedSince` (DateTimeOffset?, sent as `updated_since` in ISO 8601 UTC): Only return tasks that have been updated since the given date
  - `Page` (int?, sent as `page`): The page number to use in pagination (default: 1)
  - `PerPage` (int?, sent as `per_page`): The number of records to return per page (default: 2000, range: 1-2000)

**Response:** `TasksContainer`
```csharp
Task<TasksContainer> ListAsync([Query] ListOptions? options, CancellationToken cancellationToken);
```

### Get a Task

**GET** `/v2/tasks/{id}`

Retrieves a single task by ID.

**Parameters:**
- `id` (long): The ID of the task to retrieve

**Response:** `Task`
```csharp
Task<Models.Task> GetAsync(long id, CancellationToken cancellationToken);
```

### Create a Task

**POST** `/v2/tasks`

Creates a task. `Name` is required; optional fields left `null` take Harvest's defaults.

**Response:** `Task`
```csharp
Task<Models.Task> CreateAsync([Body] TaskCreationDto creationDto, CancellationToken cancellationToken);
```

### Update a Task

**PATCH** `/v2/tasks/{id}`

Updates a task; fields left `null` are unchanged.

**Response:** `Task`
```csharp
Task<Models.Task> UpdateAsync(long id, [Body] TaskPatchDto patchDto, CancellationToken cancellationToken);
```

### Delete a Task

**DELETE** `/v2/tasks/{id}`

Deletes a task. Harvest refuses to delete a task that has time entries; archive it instead (`IsActive = false`).

```csharp
System.Threading.Tasks.Task DeleteAsync(long id, CancellationToken cancellationToken);
```

## Implementation Status

| Method | Endpoint | Status | Notes |
|--------|----------|--------|-------|
| GET | `/v2/tasks` | ✅ Implemented | Full support with pagination and filtering |
| GET | `/v2/tasks/{id}` | ✅ Implemented | Individual task retrieval |
| POST | `/v2/tasks` | ✅ Implemented | Task creation |
| PATCH | `/v2/tasks/{id}` | ✅ Implemented | Task update |
| DELETE | `/v2/tasks/{id}` | ✅ Implemented | Task deletion |

## Missing Functionality

All task management operations are now implemented in the Harvest API client.

## Usage Examples

```csharp
// List all tasks
var tasks = await harvestClient.Tasks.ListAsync(null, cancellationToken);

// Get a specific task
var task = await harvestClient.Tasks.GetAsync(12345, cancellationToken);
```

## Data Models

### Task
```csharp
public class Task
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public bool BillableByDefault { get; set; }
    public decimal? DefaultHourlyRate { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### TasksContainer
```csharp
public class TasksContainer : ListContainerBase
{
    public List<Task> Tasks { get; set; } = [];
}
