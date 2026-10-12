# Clients API

The Clients API provides access to client management functionality in Harvest.

## Endpoints

### List All Clients

**GET** `/v2/clients`

Retrieves a list of all clients for the authenticated account.

**Parameters:**
- `options` (ListOptions?): Filtering and paging options; pass `null` to send no query parameters
  - `UpdatedSince` (DateTimeOffset?, sent as `updated_since` in ISO 8601 UTC): Only return clients that have been updated since the given date
  - `Page` (int?, sent as `page`): The page number to use in pagination (default: 1)
  - `PerPage` (int?, sent as `per_page`): The number of records to return per page (default: 2000, range: 1-2000)

**Response:** `ClientsContainer`
```csharp
Task<ClientsContainer> ListAsync([Query] ListOptions? options, CancellationToken cancellationToken);
```

### Get a Client

**GET** `/v2/clients/{id}`

Retrieves a single client by ID.

**Parameters:**
- `id` (long): The ID of the client to retrieve

**Response:** `Client`
```csharp
Task<Client> GetAsync(long id, CancellationToken cancellationToken);
```

### Create a Client

**POST** `/v2/clients`

Creates a new client.

**Request Body:** `ClientCreationDto`
```csharp
public class ClientCreationDto
{
    public required string Name { get; set; }
    public bool IsActive { get; set; }
    public string? Address { get; set; }
    public string? Currency { get; set; }
}
```

**Response:** `Client`
```csharp
Task<Client> CreateAsync(ClientCreationDto creationDto, CancellationToken cancellationToken);
```

### Update a Client

**PATCH** `/v2/clients/{id}`

Updates an existing client.

**Parameters:**
- `id` (long): The ID of the client to update

**Request Body:** `ClientPatchDto`
```csharp
public class ClientPatchDto
{
    public string? Name { get; set; }
    public bool? IsActive { get; set; }
    public string? Address { get; set; }
    public string? Currency { get; set; }
}
```

**Response:** `Client`
```csharp
Task<Client> UpdateAsync(
    long id,
    ClientPatchDto patchDto,
    CancellationToken cancellationToken
);
```

### Delete a Client

**DELETE** `/v2/clients/{id}`

Deletes a client.

**Parameters:**
- `id` (long): The ID of the client to delete

**Response:** `Task`
```csharp
Task DeleteAsync(long id, CancellationToken cancellationToken);
```

## Implementation Status

| Method | Endpoint | Status | Notes |
|--------|----------|--------|-------|
| GET | `/v2/clients` | ✅ Implemented | Full support with pagination and filtering |
| GET | `/v2/clients/{id}` | ✅ Implemented | Individual client retrieval |
| POST | `/v2/clients` | ✅ Implemented | Client creation |
| PATCH | `/v2/clients/{id}` | ✅ Implemented | Client update |
| DELETE | `/v2/clients/{id}` | ✅ Implemented | Client deletion |

## Usage Examples

```csharp
// List all clients
var clients = await harvestClient.Clients.ListAsync(null, cancellationToken);

// List the second page of clients, 50 at a time
var page2 = await harvestClient.Clients.ListAsync(new ListOptions { Page = 2, PerPage = 50 }, cancellationToken);

// Get a specific client
var client = await harvestClient.Clients.GetAsync(12345, cancellationToken);

// Create a new client
var newClient = await harvestClient.Clients.CreateAsync(new ClientCreationDto
{
    Name = "New Client",
    IsActive = true
}, cancellationToken);

// Update a client
var updatedClient = await harvestClient.Clients.UpdateAsync(clientId, new ClientPatchDto
{
    Name = "Updated Client Name"
}, cancellationToken);

// Delete a client
await harvestClient.Clients.DeleteAsync(clientId, cancellationToken);
```

## Data Models

### Client
```csharp
public class Client
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public bool IsActive { get; set; }
    public string? Address { get; set; }
    public string? Currency { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### ClientsContainer
```csharp
public class ClientsContainer : ListContainerBase
{
    public List<Client> Clients { get; set; } = [];
}
