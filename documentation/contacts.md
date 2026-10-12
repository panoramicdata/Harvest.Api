# Client Contacts API

The Client Contacts API manages the contacts of clients. It requires an Administrator, or a Manager with permission to
create and edit clients; anyone else gets `403 Forbidden`.
See the [Harvest documentation](https://help.getharvest.com/api-v2/clients-api/clients/contacts/).

## Endpoints

### List All Contacts

**GET** `/v2/contacts`

Retrieves one page of contacts, most recently created first.

**Parameters:**
- `options` (ContactListOptions?): Filtering and paging options; pass `null` to send no query parameters
  - `ClientId` (long?, sent as `client_id`): Only return contacts belonging to the given client
  - `UpdatedSince` (DateTimeOffset?, sent as `updated_since` in ISO 8601 UTC): Only return contacts updated since the given date and time
  - `Cursor` (string?, sent as `cursor`): The pagination cursor from the previous response
  - `Page` (int?, sent as `page`): The page number (deprecated by Harvest in favour of `Cursor`; default: 1)
  - `PerPage` (int?, sent as `per_page`): The number of records to return per page (default: 2000, range: 1-2000)

**Response:** `ContactsContainer`
```csharp
Task<ContactsContainer> ListAsync([Query] ContactListOptions? options, CancellationToken cancellationToken);
```

### Get a Contact

**GET** `/v2/contacts/{id}`

**Response:** `Contact`
```csharp
Task<Contact> GetAsync(long id, CancellationToken cancellationToken);
```

### Create a Contact

**POST** `/v2/contacts`

**Request Body:** `ContactCreationDto`
```csharp
public class ContactCreationDto : CreationDto
{
    public required long ClientId { get; set; }
    public string? Title { get; set; }
    public required string FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneOffice { get; set; }
    public string? PhoneMobile { get; set; }
    public string? Fax { get; set; }
    public string? InvoiceRecipientStatus { get; set; }   // none (Harvest's default), recipient, cc or bcc
}
```

**Response:** `Contact`
```csharp
Task<Contact> CreateAsync([Body] ContactCreationDto creationDto, CancellationToken cancellationToken);
```

### Update a Contact

**PATCH** `/v2/contacts/{id}`

Fields left `null` are not sent and stay unchanged. `ContactPatchDto` has the same fields as `ContactCreationDto`, all
optional.

**Response:** `Contact`
```csharp
Task<Contact> UpdateAsync(long id, [Body] ContactPatchDto patchDto, CancellationToken cancellationToken);
```

### Delete a Contact

**DELETE** `/v2/contacts/{id}`

```csharp
System.Threading.Tasks.Task DeleteAsync(long id, CancellationToken cancellationToken);
```

## Implementation Status

| Method | Endpoint | Status | Notes |
|--------|----------|--------|-------|
| GET | `/v2/contacts` | ✅ Implemented | Client filter and cursor/page pagination |
| GET | `/v2/contacts/{id}` | ✅ Implemented | |
| POST | `/v2/contacts` | ✅ Implemented | |
| PATCH | `/v2/contacts/{id}` | ✅ Implemented | |
| DELETE | `/v2/contacts/{id}` | ✅ Implemented | |

## Usage Examples

```csharp
// List a client's contacts
var contacts = await harvestClient.Contacts.ListAsync(new ContactListOptions { ClientId = clientId }, cancellationToken);

// Create a contact who is copied on the client's invoices
var contact = await harvestClient.Contacts.CreateAsync(new ContactCreationDto
{
    ClientId = clientId,
    FirstName = "George",
    LastName = "Frank",
    Email = "george@example.com",
    InvoiceRecipientStatus = InvoiceRecipientStatus.Cc
}, cancellationToken);

// Give them a title
await harvestClient.Contacts.UpdateAsync(contact.Id, new ContactPatchDto { Title = "Owner" }, cancellationToken);

// Delete them
await harvestClient.Contacts.DeleteAsync(contact.Id, cancellationToken);
```

## Data Models

### Contact
```csharp
public class Contact
{
    public long Id { get; set; }
    public ClientReference? Client { get; set; }   // id and name
    public string? Title { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneOffice { get; set; }
    public string? PhoneMobile { get; set; }
    public string? Fax { get; set; }
    public string? InvoiceRecipientStatus { get; set; }   // see the InvoiceRecipientStatus constants
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### ContactsContainer
```csharp
public class ContactsContainer : ListContainerBase
{
    public List<Contact> Contacts { get; set; } = [];
}
```
