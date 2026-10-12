# Accounts API

The Accounts API lists the Harvest and Forecast accounts that the access token can reach, from Harvest ID
([Harvest documentation](https://help.getharvest.com/api-v2/authentication-api/authentication/authentication/), under
"Scopes and account access"). Use it to discover the account IDs a personal access token or OAuth2 token can use.

Exposed as `harvestClient.Accounts` (`IAccountApi`).

Unlike every other API, these requests go to `https://id.getharvest.com` (`HarvestClient.IdBaseAddress`), not
`https://api.harvestapp.com`, and do not send the `Harvest-Account-Id` header. They share the client's authentication,
`User-Agent`, retries, timeout and transport. `HarvestClientOptions.AccountId` is still required to construct a client;
any positive value will do if the only call is to discover accounts.

## Endpoints

### List Accounts

**GET** `https://id.getharvest.com/api/v2/accounts`

**Parameters:** None

**Response:** `AccountsResponse` (not paged)
```csharp
Task<AccountsResponse> ListAsync(CancellationToken cancellationToken);
```

## Implementation Status

| Method | Endpoint | Status | Notes |
|--------|----------|--------|-------|
| GET | `https://id.getharvest.com/api/v2/accounts` | ✅ Implemented | No `Harvest-Account-Id` header |

## Usage Examples

```csharp
var response = await harvestClient.Accounts.ListAsync(cancellationToken);
foreach (var account in response.Accounts.Where(a => a.Product == AccountProduct.Harvest))
{
    Console.WriteLine($"{account.Id}: {account.Name}");
}
```

## Data Models

### AccountsResponse
```csharp
public class AccountsResponse
{
    public AccountUser? User { get; set; }
    public List<Account> Accounts { get; set; } = []; // JSON key "accounts"
}
```

### AccountUser
```csharp
public class AccountUser
{
    public long Id { get; set; } // the Harvest ID user, not a user ID within an account
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
}
```

### Account
```csharp
public class Account
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? Product { get; set; } // AccountProduct.Harvest ("harvest") or AccountProduct.Forecast ("forecast")
}
```
