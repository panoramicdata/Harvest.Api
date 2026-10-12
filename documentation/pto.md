# PTO (Time Off) API

The PTO API manages paid time off in Harvest: time off requests, time off policies (PTO types), balances, holiday
calendars, work schedules, and per-user assignments and allocations
(<https://help.getharvest.com/api-v2/pto-api/>).

> **Availability:** Harvest is rolling the PTO API out gradually. Every call from an account without it fails with
> `403 Forbidden` (a `HarvestApiException`). Most endpoints are also administrator-only; a non-administrator gets `403`
> (or `404` for a policy that is not visible to them).

## Design

The PTO API is exposed as seven flat resource interfaces on `HarvestClient` / `IHarvestClient`, one per documentation
page (the user assignments and allocations page is split in two), all prefixed `Pto` so they group together and do not
collide with the core resources (`Users`, `UserProjectAssignments`):

| Property | Interface | Harvest page |
|----------|-----------|--------------|
| `PtoWorkSchedules` | `IPtoWorkScheduleApi` | Work Schedules |
| `PtoTypes` | `IPtoTypeApi` | Time Off Policies |
| `PtoRequests` | `IPtoRequestApi` | Time Off Requests |
| `PtoBalances` | `IPtoBalanceApi` | Time Off Balances |
| `PtoHolidayCalendars` | `IPtoHolidayCalendarApi` | Holiday Calendars (calendars and their entries) |
| `PtoAssignments` | `IPtoAssignmentApi` | User Assignments & Allocations (assignments) |
| `PtoAllocations` | `IPtoAllocationApi` | User Assignments & Allocations (allocations) |

Harvest's documentation calls time off policies "policies", but its paths and JSON call them PTO types
(`/v2/pto/types`, `pto_type`, `pto_type_id`), so the library follows the wire names (`PtoType`, `PtoTypeId`).

`DELETE` endpoints that do not delete are named for what they do: `PtoWorkSchedules.DeactivateAsync`,
`PtoTypes.DeactivateAsync` and `PtoRequests.CancelAsync` return the updated object.

## Work Schedules (`PtoWorkSchedules`)

| Method | Endpoint | C# |
|--------|----------|----|
| GET | `/v2/pto/work_schedules` | `ListAsync(ListOptions? options, ct)` (`is_active`, `page`, `per_page`) |
| GET | `/v2/pto/work_schedules/{id}` | `GetAsync(id, ct)` |
| POST | `/v2/pto/work_schedules` | `CreateAsync(PtoWorkScheduleCreationDto, ct)` |
| PATCH | `/v2/pto/work_schedules/{id}` | `UpdateAsync(id, PtoWorkSchedulePatchDto, ct)` |
| DELETE | `/v2/pto/work_schedules/{id}` | `DeactivateAsync(id, ct)` - returns the deactivated schedule |

## Time Off Policies (`PtoTypes`)

| Method | Endpoint | C# |
|--------|----------|----|
| GET | `/v2/pto/types` | `ListAsync(ListOptions? options, ct)` (`is_active`, `updated_since`, `page`, `per_page`) |
| GET | `/v2/pto/types/{id}` | `GetAsync(id, ct)` |
| POST | `/v2/pto/types` | `CreateAsync(PtoTypeCreationDto, ct)` |
| PATCH | `/v2/pto/types/{id}` | `UpdateAsync(id, PtoTypePatchDto, ct)` - `Deactivated = false` reactivates |
| DELETE | `/v2/pto/types/{id}` | `DeactivateAsync(id, ct)` - returns the deactivated policy (`keep_history` default) |
| DELETE | `/v2/pto/types/{id}?keep_history=false&confirm_delete_history=1` | `DeleteAsync(id, ct)` - permanent, `204 No Content` |

Accrual values are strings with constants in `PtoAccrualType` (`fixed`, `accrued`, `unlimited`) and `PtoAccrualSchedule`
(`monthly`, `biweekly`, `semimonthly`, `hourly`).

## Time Off Requests (`PtoRequests`)

| Method | Endpoint | C# |
|--------|----------|----|
| GET | `/v2/pto/requests` | `ListAsync(PtoRequestListOptions? options, ct)` |
| GET | `/v2/pto/requests/{id}` | `GetAsync(id, ct)` |
| POST | `/v2/pto/requests` | `CreateAsync(PtoRequestCreationDto, ct)` |
| PATCH | `/v2/pto/requests/{id}` | `UpdateAsync(id, PtoRequestPatchDto, ct)` - returns a replacement request with a **new ID** |
| DELETE | `/v2/pto/requests/{id}` | `CancelAsync(id, ct)` - status becomes `cancelled` |
| POST | `/v2/pto/requests/{id}/approve` | `ApproveAsync(id, ct)` |
| POST | `/v2/pto/requests/{id}/decline` | `DeclineAsync(id, PtoRequestDeclineDto, ct)` - `Reason` optional |
| POST | `/v2/pto/requests/{id}/request_changes` | `RequestChangesAsync(id, PtoRequestChangesDto, ct)` - `Reason` required |

`PtoRequestListOptions` adds `UserId` (`user_id`), `Status` (`status`, see `PtoRequestStatus`), `PtoTypeId`
(`pto_type_id`), `From` / `To` (`from` / `to`, `YYYY-MM-DD`) to `UpdatedSince`, `Page` and `PerPage`.
`ConflictResolution` on the request DTOs takes `PtoConflictResolution.Skip` or `.Replace`.

## Time Off Balances (`PtoBalances`)

| Method | Endpoint | C# |
|--------|----------|----|
| GET | `/v2/pto/balances` | `GetAsync(PtoBalanceOptions? options, ct)` (`user_id`, `year`, `as_of`) |

Harvest sends the balance amounts as JSON strings (`"accrued_days": "25.0"`); `PtoBalance` reads them as `decimal?`
whether they arrive as strings or numbers.

## Holiday Calendars (`PtoHolidayCalendars`)

| Method | Endpoint | C# |
|--------|----------|----|
| GET | `/v2/pto/holiday_calendars` | `ListAsync(ListOptions? options, ct)` (`page`, `per_page`) |
| GET | `/v2/pto/holiday_calendars/{id}` | `GetAsync(id, ct)` |
| POST | `/v2/pto/holiday_calendars` | `CreateAsync(PtoHolidayCalendarCreationDto, ct)` - `CountryCode` auto-populates public holidays |
| PATCH | `/v2/pto/holiday_calendars/{id}` | `UpdateAsync(id, PtoHolidayCalendarPatchDto, ct)` - `Name` required |
| DELETE | `/v2/pto/holiday_calendars/{id}` | `DeleteAsync(id, ct)` - returns the deleted calendar |
| GET | `/v2/pto/holiday_calendars/{id}/entries` | `ListEntriesAsync(holidayCalendarId, ListOptions? options, ct)` |
| POST | `/v2/pto/holiday_calendars/{id}/entries` | `CreateEntryAsync(holidayCalendarId, PtoHolidayCalendarEntryCreationDto, ct)` |
| PATCH | `/v2/pto/holiday_calendars/{id}/entries/{entry_id}` | `UpdateEntryAsync(holidayCalendarId, id, PtoHolidayCalendarEntryPatchDto, ct)` |
| DELETE | `/v2/pto/holiday_calendars/{id}/entries/{entry_id}` | `DeleteEntryAsync(holidayCalendarId, id, ct)` - returns the deleted entry |

An entry is created for a single `Date`, or for a range with `DateMode = PtoHolidayCalendarEntryCreationDto.RangeDateMode`
(`"range"`), `StartDate` and `EndDate`.

## User Assignments (`PtoAssignments`)

| Method | Endpoint | C# |
|--------|----------|----|
| GET | `/v2/pto/assignments` | `ListAsync(PtoAssignmentListOptions? options, ct)` (`user_id`, `user_ids[]`) |
| PATCH | `/v2/pto/assignments/{user_id}` | `UpdateAsync(userId, PtoAssignmentPatchDto, ct)` |

Harvest unassigns a holiday calendar or work schedule when it receives an explicit JSON `null`. Because `null`
properties are omitted (leaving the assignment unchanged), `PtoAssignmentPatchDto` has `UnassignHolidayCalendar` and
`UnassignWorkSchedule` flags that send `null`:

```csharp
// Assign a calendar and revert the work schedule to the company default
await harvestClient.PtoAssignments.UpdateAsync(userId, new PtoAssignmentPatchDto
{
	HolidayCalendarId = 12,
	UnassignWorkSchedule = true
}, cancellationToken);
// {"holiday_calendar_id":12,"work_schedule_id":null}
```

## User Allocations (`PtoAllocations`)

| Method | Endpoint | C# |
|--------|----------|----|
| GET | `/v2/pto/allocations` | `ListAsync(PtoAllocationListOptions? options, ct)` (`year`, `user_ids[]`, `pto_type_ids[]`, `page`, `per_page`) |
| PATCH | `/v2/pto/allocations` | `UpdateAsync(PtoAllocationPatchDto, ct)` - `UserId` and `PtoTypeId` required |

The update answers either with the saved allocation or, when it resets the allocation to the policy defaults
(`ResetToDefault = true`, or values equal to the defaults), with a confirmation. Both read into
`PtoAllocationUpdateResult`; check `IsReset`:

```csharp
var result = await harvestClient.PtoAllocations.UpdateAsync(new PtoAllocationPatchDto
{
	UserId = userId,
	PtoTypeId = 101,
	Year = 2026,
	ResetToDefault = true
}, cancellationToken);
if (result.IsReset)
{
	Console.WriteLine(result.Message); // "Allocation reset to type defaults"
}
```

## Notes and Documentation Ambiguities

- **Array query parameters** (`user_ids`, `pto_type_ids`): Harvest documents them as "array of integers" without a wire
  format. They are sent in the Rails convention, as repeated `user_ids[]=1&user_ids[]=2` parameters.
- **Holiday calendar update**: Harvest marks `name` as required on `PATCH`, so `PtoHolidayCalendarPatchDto.Name` is
  `required` (unlike most patch bodies).
- **Paging**: the assignments list documents no paging, so `PtoAssignmentsContainer` has only `Assignments`. Other lists
  document only `page` / `per_page`; the inherited `Cursor` is sent if set and is otherwise harmless.
- **Allocation reset**: the reset confirmation has no `id` or timestamps, so `Id` is `0` and `CreatedAt` / `UpdatedAt`
  are `null` on it.

## Implementation Status

All 32 documented operations are implemented.

## Usage Examples

```csharp
// Book two days of vacation
var request = await harvestClient.PtoRequests.CreateAsync(new PtoRequestCreationDto
{
	PtoTypeId = 101,
	StartDate = new DateOnly(2026, 11, 5),
	EndDate = new DateOnly(2026, 11, 6),
	Notes = "Doctor appointment"
}, cancellationToken);

// List pending requests in October
var pending = await harvestClient.PtoRequests.ListAsync(new PtoRequestListOptions
{
	Status = PtoRequestStatus.Pending,
	From = new DateOnly(2026, 10, 1),
	To = new DateOnly(2026, 10, 31)
}, cancellationToken);

// Approve one
await harvestClient.PtoRequests.ApproveAsync(request.Id, cancellationToken);

// A user's balances this year
var balances = await harvestClient.PtoBalances.GetAsync(new PtoBalanceOptions { UserId = userId }, cancellationToken);
```
