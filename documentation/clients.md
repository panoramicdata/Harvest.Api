# Clients API

The Clients API provides access to client management functionality in Harvest.

## Endpoints

### List All Clients

**GET** `/v2/clients`

Retrieves a list of all clients for the authenticated account.

**Parameters:**
- `updated_since` (DateTime?, optional): Only return clients that have been updated since the given date
- `page` (int?, optional): The page number to use in pagination (default: 1)
- `per_page` (int?, optional): The number of records to return per page (default: 100, max: 100)

**Response:** `ClientsContainer`
```csharp
Task<ClientsContainer> ListAllAsync(
    DateTime? updatedSince = null,
    int? page = null,
    int? perPage = null,
    CancellationToken cancellationToken = default
);
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
Task<Client> CreateAsync(ClientCreationDto clientDto, CancellationToken cancellationToken);
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
    ClientPatchDto clientPatchDto,
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
var clients = await harvestClient.Clients.ListAllAsync();

// Get a specific client
var client = await harvestClient.Clients.GetAsync(12345);

// Create a new client
var newClient = await harvestClient.Clients.CreateAsync(new ClientCreationDto
{
    Name = "New Client",
    IsActive = true
}, CancellationToken);

// Update a client
var updatedClient = await harvestClient.Clients.UpdateAsync(clientId, new ClientPatchDto
{
    Name = "Updated Client Name"
}, CancellationToken);

// Delete a client
await harvestClient.Clients.DeleteAsync(clientId, CancellationToken);
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
