# Time Entries API

The Time Entries API provides time tracking in Harvest.

## Endpoints

### List All Time Entries

**GET** `/v2/time_entries`

**Parameters:**
- `options` (TimeEntryListOptions?): Filtering and paging options; pass `null` to send no query parameters
  - `UserId`, `ClientId`, `ProjectId`, `TaskId` (long?, sent as `user_id`, `client_id`, `project_id`, `task_id`)
  - `ExternalReferenceId` (string?, sent as `external_reference_id`)
  - `IsBilled`, `IsRunning` (bool?, sent as `is_billed`, `is_running`)
  - `ApprovalStatus` (string?, sent as `approval_status`): one of the `ApprovalStatus` constants
  - `From`, `To` (DateOnly?, sent as `from`, `to` in `YYYY-MM-DD`)
  - `UpdatedSince` (DateTimeOffset?, sent as `updated_since` in ISO 8601 UTC)
  - `Cursor`, `Page`, `PerPage` (paging; cursor paging is preferred, see `ListOptions`)

```csharp
Task<TimeEntriesContainer> ListAsync([Query] TimeEntryListOptions? options, CancellationToken cancellationToken);
```

### Get a Time Entry

**GET** `/v2/time_entries/{id}`

```csharp
Task<TimeEntry> GetAsync(long id, CancellationToken cancellationToken);
```

### Create a Time Entry

**POST** `/v2/time_entries`

```csharp
Task<TimeEntry> CreateAsync([Body] TimeEntryCreationDto creationDto, CancellationToken cancellationToken);
```

### Update a Time Entry

**PATCH** `/v2/time_entries/{id}`

```csharp
Task<TimeEntry> UpdateAsync(long id, [Body] TimeEntryPatchDto patchDto, CancellationToken cancellationToken);
```

### Delete a Time Entry

**DELETE** `/v2/time_entries/{id}`

```csharp
System.Threading.Tasks.Task DeleteAsync(long id, CancellationToken cancellationToken);
```

### Delete a Time Entry's External Reference

**DELETE** `/v2/time_entries/{id}/external_reference`

Removes the external reference, leaving the time entry in place.

```csharp
System.Threading.Tasks.Task DeleteExternalReferenceAsync(long id, CancellationToken cancellationToken);
```

### Restart a Stopped Time Entry

**PATCH** `/v2/time_entries/{id}/restart` (no body)

Only possible while the time entry is not running. Returns the running time entry.

```csharp
Task<TimeEntry> RestartAsync(long id, CancellationToken cancellationToken);
```

### Stop a Running Time Entry

**PATCH** `/v2/time_entries/{id}/stop` (no body)

Only possible while the time entry is running. Returns the stopped time entry.

```csharp
Task<TimeEntry> StopAsync(long id, CancellationToken cancellationToken);
```

## Implementation Status

| Method | Endpoint | Status |
|--------|----------|--------|
| GET | `/v2/time_entries` | ✅ Implemented |
| GET | `/v2/time_entries/{id}` | ✅ Implemented |
| POST | `/v2/time_entries` | ✅ Implemented |
| PATCH | `/v2/time_entries/{id}` | ✅ Implemented |
| DELETE | `/v2/time_entries/{id}` | ✅ Implemented |
| DELETE | `/v2/time_entries/{id}/external_reference` | ✅ Implemented |
| PATCH | `/v2/time_entries/{id}/restart` | ✅ Implemented |
| PATCH | `/v2/time_entries/{id}/stop` | ✅ Implemented |

## Usage Examples

```csharp
// List time entries for a user in January
var entries = await harvestClient.TimeEntries.ListAsync(new TimeEntryListOptions
{
    UserId = 12345,
    From = new DateOnly(2024, 1, 1),
    To = new DateOnly(2024, 1, 31)
}, cancellationToken);

// Create a time entry by duration
var newEntry = await harvestClient.TimeEntries.CreateAsync(new TimeEntryCreationDto
{
    ProjectId = 12345,
    TaskId = 67890,
    SpentDate = new DateOnly(2024, 1, 15),
    Hours = 4.5m,
    Notes = "Working on feature implementation"
}, cancellationToken);

// Update a time entry
await harvestClient.TimeEntries.UpdateAsync(newEntry.Id, new TimeEntryPatchDto { Notes = "Updated notes" }, cancellationToken);

// Stop a running timer, then restart it
var stopped = await harvestClient.TimeEntries.StopAsync(newEntry.Id, cancellationToken);
var running = await harvestClient.TimeEntries.RestartAsync(newEntry.Id, cancellationToken);

// Delete a time entry
await harvestClient.TimeEntries.DeleteAsync(newEntry.Id, cancellationToken);
```

## Data Models

`TimeEntry` follows the documented attribute list: nested `User`, `Client`, `Project` and `Task` are `{id, name}`
references; `ExternalReference` has string `Id`, `GroupId`, `AccountId`, `Permalink`, `Service` and `ServiceIconUrl`;
`HoursWithoutTimer`, `RoundedHours`, `ApprovalStatus` and `TimerStartedAt` are included. `StartedTime`/`EndedTime` are
the strings Harvest sends (`8:00am`, or `08:00` on a 24-hour clock); `StartedTimeSpan`/`EndedTimeSpan` parse them
tolerantly, returning `null` for anything unrecognised, and are never serialized.

`TimeEntryCreationDto` and `TimeEntryPatchDto` take `SpentDate` as a `DateOnly` and an `ExternalReferenceCreationDto`
(`Id`, `GroupId`, `AccountId`, `Permalink`).
