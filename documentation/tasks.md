# Tasks API

The Tasks API provides access to task management functionality in Harvest.

## Endpoints

### List All Tasks

**GET** `/v2/tasks`

Retrieves a list of all tasks for the authenticated account.

**Parameters:**
- `updated_since` (DateTime?, optional): Only return tasks that have been updated since the given date
- `page` (int?, optional): The page number to use in pagination (default: 1)
- `per_page` (int?, optional): The number of records to return per page (default: 100, max: 100)

**Response:** `TasksContainer`
```csharp
Task<TasksContainer> ListAllAsync(
    DateTime? updatedSince = null,
    int? page = null,
    int? perPage = null,
    CancellationToken cancellationToken = default
);
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
var tasks = await harvestClient.Tasks.ListAllAsync();

// Get a specific task
var task = await harvestClient.Tasks.GetAsync(12345);
```

## Data Models

### Task
```csharp
public class Task
{
    public long Id { get; set; }
    public required string Name { get; set; }
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
