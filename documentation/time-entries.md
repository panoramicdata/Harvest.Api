# Time Entries API

The Time Entries API provides comprehensive time tracking functionality in Harvest.

## Endpoints

### List All Time Entries

**GET** `/v2/time_entries`

Retrieves a list of time entries with extensive filtering options.

**Parameters:**
- `user_id` (long?, optional): Only return time entries belonging to the user with the given ID
- `client_id` (long?, optional): Only return time entries belonging to the client with the given ID
- `project_id` (long?, optional): Only return time entries belonging to the project with the given ID
- `is_billed` (bool?, optional): Pass `true` to only return time entries that have been invoiced
- `is_running` (bool?, optional): Pass `true` to only return running time entries
- `updated_since` (DateTime?, optional): Only return time entries that have been updated since the given date
- `from` (string?, optional): Only return time entries with a `spent_date` on or after the given date
- `to` (string?, optional): Only return time entries with a `spent_date` on or before the given date
- `page` (int?, optional): The page number to use in pagination (default: 1)
- `per_page` (int?, optional): The number of records to return per page (default: 100, max: 100)

**Response:** `TimeEntriesContainer`
```csharp
Task<TimeEntriesContainer> ListAllAsync(
    long? userId = null,
    long? clientId = null,
    long? projectId = null,
    bool? isBilled = null,
    bool? isRunning = null,
    DateTime? updatedSince = null,
    string? from = null,
    string? toDate = null,
    int? page = null,
    int? perPage = null,
    CancellationToken cancellationToken = default
);
```

### Get a Time Entry

**GET** `/v2/time_entries/{id}`

Retrieves a single time entry by ID.

**Parameters:**
- `id` (long): The ID of the time entry to retrieve

**Response:** `TimeEntry`
```csharp
Task<TimeEntry> GetAsync(long id, CancellationToken cancellationToken);
```

### Create a Time Entry

**POST** `/v2/time_entries`

Creates a new time entry.

**Request Body:** `TimeEntryCreationDto`

**Response:** `TimeEntry`
```csharp
Task<TimeEntry> CreateAsync(TimeEntryCreationDto creationDto, CancellationToken cancellationToken);
```

### Update a Time Entry

**PATCH** `/v2/time_entries/{id}`

Updates an existing time entry.

**Request Body:** `TimeEntryPatchDto`

**Response:** `TimeEntry`
```csharp
Task<TimeEntry> PatchAsync(long id, TimeEntryPatchDto patchDto, CancellationToken cancellationToken);
```

### Delete a Time Entry

**DELETE** `/v2/time_entries/{id}`

Deletes a time entry.

**Parameters:**
- `id` (long): The ID of the time entry to delete

**Response:** None
```csharp
Task DeleteAsync(long id, CancellationToken cancellationToken);
```

## Implementation Status

| Method | Endpoint | Status | Notes |
|--------|----------|--------|-------|
| GET | `/v2/time_entries` | ✅ Implemented | Full support with comprehensive filtering |
| GET | `/v2/time_entries/{id}` | ✅ Implemented | Individual time entry retrieval |
| POST | `/v2/time_entries` | ✅ Implemented | Time entry creation |
| PATCH | `/v2/time_entries/{id}` | ✅ Implemented | Time entry update |
| DELETE | `/v2/time_entries/{id}` | ✅ Implemented | Time entry deletion |

## Usage Examples

```csharp
// List time entries for a specific user and date range
var entries = await harvestClient.TimeEntries.ListAllAsync(
    userId: 12345,
    from: "2024-01-01",
    toDate: "2024-01-31"
);

// Create a new time entry
var newEntry = await harvestClient.TimeEntries.CreateAsync(new TimeEntryCreationDto
{
    ProjectId = 12345,
    TaskId = 67890,
    SpentDate = "2024-01-15",
    Hours = 4.5m,
    Notes = "Working on feature implementation"
});

// Update a time entry
await harvestClient.TimeEntries.PatchAsync(entryId, new TimeEntryPatchDto
{
    Notes = "Updated notes"
});

// Delete a time entry
await harvestClient.TimeEntries.DeleteAsync(entryId);
```

## Data Models

### TimeEntry
```csharp
public class TimeEntry
{
    public long Id { get; set; }
    public DateTime SpentDate { get; set; }
    public User? User { get; set; }
    public UserAssignment? UserAssignment { get; set; }
    public Client? Client { get; set; }
    public Project? Project { get; set; }
    public Task? Task { get; set; }
    public ProjectTaskAssignment? TaskAssignment { get; set; }
    public ExternalReference? ExternalsReference { get; set; }
    public Invoice? Invoice { get; set; }
    public decimal Hours { get; set; }
    public string? Notes { get; set; }
    public bool IsLocked { get; set; }
    public string? LockedReason { get; set; }
    public bool IsClosed { get; set; }
    public bool IsBilled { get; set; }
    public DateTime? SpentAt { get; set; }
    public string? StartedTime { get; set; }
    public TimeSpan? StartedTimeSpan { get; }
    public string? EndedTime { get; set; }
    public TimeSpan? EndedTimeSpan { get; }
    public bool IsRunning { get; set; }
    public bool Billable { get; set; }
    public bool Budgeted { get; set; }
    public decimal? BillableRate { get; set; }
    public decimal? CostRate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### TimeEntryCreationDto
```csharp
public class TimeEntryCreationDto : CreationDto
{
    public long? UserId { get; set; }
    public long ProjectId { get; set; }
    public long TaskId { get; set; }
    public string? SpentDate { get; set; }
    public string? StartedTime { get; set; }
    public string? EndedTime { get; set; }
    public decimal? Hours { get; set; }
    public string? Notes { get; set; }
    public ExternalReferenceCreationDto? ExternalReference { get; set; }
}
```

### TimeEntryPatchDto
```csharp
public class TimeEntryPatchDto : PatchDto
{
    public long? ProjectId { get; set; }
    public long? TaskId { get; set; }
    public string? SpentDate { get; set; }
    public string? StartedTime { get; set; }
    public string? EndedTime { get; set; }
    public decimal? Hours { get; set; }
    public string? Notes { get; set; }
    public ExternalReference? ExternalReference { get; set; }
}
```

### TimeEntriesContainer
```csharp
public class TimeEntriesContainer : ListContainerBase
{
    public List<TimeEntry> TimeEntries { get; set; } = [];
}
```
