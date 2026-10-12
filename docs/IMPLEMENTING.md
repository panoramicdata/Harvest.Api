# Implementing a Harvest resource

This is the checklist for adding a Harvest API v2 resource (or an endpoint on an existing one) to this library. The
core (client, retry, errors, paging, serialization, unit-test harness) is in place; a resource is an interface, its
models and DTOs, its registration, and its unit tests. Follow the existing resources (`IClientApi`, `IProjectApi`,
`ITimeEntryApi`) as worked examples.

Always confirm field lists, parameters and required flags against the resource's page under
<https://help.getharvest.com/api-v2/>; do not copy v1 fields or guess.

## 1. The interface

- One interface per resource in `Harvest/Interfaces`, named `I<Resource>Api` (singular resource: `IInvoiceApi`,
  `IExpenseCategoryApi`, `IProjectUserAssignmentApi`).
- Refit attributes with absolute paths: `[Get("/v2/invoices/{id}")]`.
- Method names:

  | Operation | Name | Signature shape |
  |-----------|------|-----------------|
  | List one page | `ListAsync` | `Task<XsContainer> ListAsync([Query] XListOptions? options, CancellationToken cancellationToken)` |
  | Get one | `GetAsync` | `Task<X> GetAsync(long id, CancellationToken cancellationToken)` |
  | Create | `CreateAsync` | `Task<X> CreateAsync([Body] XCreationDto creationDto, CancellationToken cancellationToken)` |
  | Update (PATCH) | `UpdateAsync` | `Task<X> UpdateAsync(long id, [Body] XPatchDto patchDto, CancellationToken cancellationToken)` |
  | Delete | `DeleteAsync` | `System.Threading.Tasks.Task DeleteAsync(long id, CancellationToken cancellationToken)` |
  | Other actions | verb + `Async` | e.g. `RestartAsync`, `StopAsync`, `ApproveAsync` |

  Decision (3.0): list methods are `ListAsync`, not `ListAllAsync` - they return one page, and the old name implied
  otherwise. `ListAllAsync` and `TimeEntries.PatchAsync` were renamed in this release; do not reintroduce them. A list
  scoped to the current user is `ListMineAsync`.
- Parent IDs come first, named for what they are: `ListAsync(long projectId, ...)`, `GetAsync(long projectId, long id, ...)`.
- A `DELETE` that does not delete is named for what it does and returns the object Harvest sends back
  (`PtoRequests.CancelAsync`, `PtoTypes.DeactivateAsync`). A child collection documented on its parent's page goes on the
  parent's interface with the child in the name (`PtoHolidayCalendars.ListEntriesAsync`, `CreateEntryAsync`).
- A `multipart/form-data` upload (expense receipts) is a public default-interface overload taking the DTO and a
  `StreamPart`, which calls a hidden (`[EditorBrowsable(Never)]`) `[Multipart]` method whose fields are formatted with
  `HarvestFormValue`.
- `CancellationToken cancellationToken` is **required and last** on every method (a unit test enforces this).
- Return `System.Threading.Tasks.Task` for no body; `Harvest.Models.Task` makes plain `Task` ambiguous inside the library.
- Mark bodies `[Body]` and options `[Query]`.

## 2. List options

- Every list method takes an options object, nullable, passed as `[Query]`. `null` sends no query string.
- Use `ListOptions` when the endpoint has only the common parameters (`is_active`, `updated_since`, `cursor`, `page`,
  `per_page`). When it has more filters, add `<Resource>ListOptions : ListOptions` in `Harvest/Models`, as
  `TimeEntryListOptions` and `ProjectListOptions` do.
- Each property: `init`-only, nullable, `[AliasAs("wire_name")]`, and an XML summary naming the wire parameter.
- Types: IDs `long?`; flags `bool?` (sent as lowercase `true`/`false`); dates `DateOnly?` (sent `YYYY-MM-DD`); date-times
  `DateTimeOffset?` (sent ISO 8601 UTC). `HarvestUrlParameterFormatter` does this formatting; do not pre-format strings.
- A filter that takes several values (`user_ids[]`) is an `IReadOnlyCollection<long>?` with `[AliasAs("user_ids[]")]` and
  `[Query(CollectionFormat.Multi)]`, which sends one parameter per value (see `PtoAllocationListOptions`).
- Endpoints that are not paged take a standalone options class that does not derive from `ListOptions`
  (`PtoAssignmentListOptions`, `PtoBalanceOptions`, `InvoiceMessageTemplateOptions`), or none when they have no
  parameters (`Accounts.ListAsync`).
- The reports take their own `ReportOptions` family: `From` and `To` are C# `required` and sent as `YYYYMMDD` with
  `[Query(Format = ...)]`, as the Reports API documents, and paging is by page number only.
- Paging: each list returns a `<Resources>Container : ListContainerBase` (paging fields plus `Links`). Callers page with
  `Links.NextCursor` (as `Cursor`) or `NextPage` (as `Page`); see the README. A response that Harvest does not page keeps
  the `Container` name (or `Response` when it carries more than the list, as `AccountsResponse`) but does not derive from
  `ListContainerBase` (`PtoAssignmentsContainer`).

## 3. Models (responses)

- Namespace `Harvest.Models`, one class per resource, `public class` with `{ get; set; }` properties.
- The serializer maps PascalCase to snake_case (`JsonNamingPolicy.SnakeCaseLower`). Name properties so they snake-case to
  the documented key; use `[JsonPropertyName]` only where that is impossible. The list property of a container is named
  after the JSON key (`task_assignments` -> `TaskAssignments`); getting this wrong silently yields an empty list.
- Be tolerant:
  - never `required` on a response model (a missing field must not throw);
  - `Id` is `long`; booleans and timestamps that Harvest always sends may be non-nullable; everything that can be
    `null` or omitted (strings, decimals, dates, nested objects, lists, anything absent in nested representations) is
    nullable;
  - string-valued enumerations (statuses, display formats) stay `string`, with a static class of constants where useful
    (`ApprovalStatus`), so a new Harvest value does not break deserialization;
  - a field documented as a string that may arrive as a number uses `[JsonConverter(typeof(FlexibleStringConverter))]`.
- Types: money and hours `decimal`; dates `DateOnly`; date-times `DateTime` (UTC); seconds `int`.
- Nested summaries use the shared reference types in `References.cs`: `UserReference`, `ClientReference`,
  `ProjectReference`, `TaskReference`, `InvoiceReference`, `ExpenseCategoryReference`, `EstimateReference`,
  `RetainerReference`, `PaymentGatewayReference`, `PtoTypeReference`, `PtoHolidayCalendarReference` and
  `PtoWorkScheduleReference`. Add a new `<Thing>Reference` there rather than reusing a full model for a `{id, name}`
  object, and reuse an existing type when two resources nest the same shape (estimate and invoice messages share
  `MessageRecipient` and `MessageRecipientCreationDto`).
- Computed helpers must be `[JsonIgnore]` and must never throw (see `TimeEntry.StartedTimeSpan`).

## 4. DTOs (request bodies)

- `<Resource>CreationDto : CreationDto` for POST and `<Resource>PatchDto : PatchDto` for PATCH, in
  `Harvest/Models/<Resource>Dtos.cs` (or one file each when large).
- Never repeat a run of fields. When the creation and patch bodies share optional fields, put them in an abstract
  `<Resource>DtoBase` that both derive from instead (`ProjectDtoBase`, `UserDtoBase`); when a response model shares them
  too, name the base for what the fields are (`ContactDetails`, `PtoAccrualSettings`, `PtoScheduleHours`). Estimates and
  invoices share `BillingDocument<TLineItem>`, `BillingDocumentDto<TLineItem>`, `LineItem` and the line item DTO bases.
  System.Text.Json writes a derived class's own properties before its base's, which the body tests pin.
- The body of another action is `<Resource><Action>Dto` and derives from neither base (`PtoRequestDeclineDto` for
  `PtoRequests.DeclineAsync`).
- Creation DTO: documented-required fields are C# `required` (non-nullable); every optional field is nullable so it is
  omitted and Harvest applies its default. Never give an optional field a C# default (`= true`) - that overrides
  Harvest's default. State Harvest's default in the XML summary.
- Patch DTO: every field nullable, unless Harvest documents the field as required on the PATCH itself (`TeammatePatchDto`
  replaces the whole list, `PtoAllocationPatchDto` names the allocation by user and policy). Where Harvest needs an
  explicit JSON `null`, add a flag rather than relying on a `null` property (`PtoAssignmentPatchDto.UnassignWorkSchedule`).
- No `[AliasAs]` on body properties (it does not affect JSON); naming follows the snake_case policy.
- Nested request objects get their own DTO (`ExternalReferenceCreationDto`), never the response model.

## 5. Register the API

- Add a property to `IHarvestClient` with an XML summary, and the implementation to `HarvestClient` (`/// <inheritdoc />`),
  assigned in the constructor with `For<I<Resource>Api>()`. Keep the property order identical in both. The test
  `HarvestClient_ExposesEveryApiThroughIHarvestClient` fails if the interface misses one; update its expected count.
- Add the property to the README's "Available APIs" table.

## 6. XML documentation

- `GenerateDocumentationFile` is on and warnings are errors, so every public type and member needs XML docs.
- Interfaces: a `<summary>` linking the Harvest page (`<see href="..."/>`); each method `<summary>`, every `<param>` and
  `<returns>`.
- Models and DTOs: a summary per property, in Harvest's words, naming allowed values where documented.

## 7. Unit tests (Harvest.UnitTest)

Never call the live API from unit tests; `Harvest.Test` is the live suite and is not run in CI.

- One test class per resource in `Harvest.UnitTest/Resources/<Resource>ApiTests.cs`.
- Pin **every** method's request with `TestClient.SendAsync` and `ShouldBe(method, path, query, body)`:

  ```csharp
  var call = await TestClient.SendAsync((c, ct) => c.Clients.CreateAsync(new ClientCreationDto { Name = "Acme" }, ct), ClientJson);

  call.ShouldBe(HttpMethod.Post, "/v2/clients", body: """{"name":"Acme"}""");
  ```

  The body is compared as exact JSON text, which also proves snake_case names and omission of `null` fields. For each
  list method assert the empty query (null options) and a query with every option set. For each DTO assert a
  required-only body and an every-field body.
- Map responses with `TestClient.ReadAsync` against a JSON sample taken from the Harvest docs page, asserting every
  property (a private `ShouldBeThe<Resource>` helper keeps it to one place).
- Errors, retries and headers are covered centrally (`Core/`); a resource needs no error tests of its own.
- `StubHandler` (a recording `HttpMessageHandler`) and `TestClient` live in `Harvest.UnitTest/Support`. Inside the test
  project `Task` means `System.Threading.Tasks.Task`; refer to the model as `Models.Task`.
- Coverage must stay at 100% of lines and branches of the library. Measure it with:

  ```powershell
  dotnet build Harvest.UnitTest/Harvest.UnitTest.csproj
  ./Harvest.UnitTest/bin/Debug/net10.0/Harvest.UnitTest --coverage --coverage-settings "$PWD/coverage.config" --coverage-output-format cobertura --coverage-output coverage.cobertura.xml
  ```

  (`--coverage-settings` must be an absolute path.) `dotnet test Harvest.UnitTest/Harvest.UnitTest.csproj` runs the
  tests without coverage. Do not run `dotnet test` on the solution: that includes the live `Harvest.Test` project.

## 8. Platform behaviour you get for free

- Authentication, `Harvest-Account-Id`, `Accept` and `User-Agent` headers (`HarvestClientOptions.UserAgent`, defaulting
  to `Harvest.Api/<version> (+https://github.com/panoramicdata/Harvest.Api)`).
- Retries of 429 (honouring `Retry-After`) and transient 5xx (idempotent verbs; 503 for any verb) with bounded
  exponential back-off, in `Handlers/RetryHandler`. Bodies are buffered so they can be resent.
- Non-success responses raise `HarvestApiException` (a Refit `ApiException`) with Harvest's `message` /
  `error_description` as the message and `error` as `ErrorCode`.

## 9. Formatting and build

- Follow `.editorconfig`: tabs, CRLF. Run `dotnet format whitespace Harvest.slnx` before committing.
- Build the library and `Harvest.UnitTest` with zero warnings; never weaken `TreatWarningsAsErrors`.
