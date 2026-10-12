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
Task<TimeEntry> CreateAsync(TimeEntryCreationDto creationDto, CancellationToken cancellationToken);
```

### Update a Time Entry

**PATCH** `/v2/time_entries/{id}`

```csharp
Task<TimeEntry> UpdateAsync(long id, TimeEntryPatchDto patchDto, CancellationToken cancellationToken);
```

### Delete a Time Entry

**DELETE** `/v2/time_entries/{id}`

```csharp
Task DeleteAsync(long id, CancellationToken cancellationToken);
```

## Implementation Status

| Method | Endpoint | Status |
|--------|----------|--------|
| GET | `/v2/time_entries` | ✅ Implemented |
| GET | `/v2/time_entries/{id}` | ✅ Implemented |
| POST | `/v2/time_entries` | ✅ Implemented |
| PATCH | `/v2/time_entries/{id}` | ✅ Implemented |
| DELETE | `/v2/time_entries/{id}` | ✅ Implemented |

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
