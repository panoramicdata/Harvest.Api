namespace Harvest.Interfaces;

/// <summary>
/// Client contacts (<see href="https://help.getharvest.com/api-v2/clients-api/clients/contacts/"/>). Requires an
/// Administrator, or a Manager with permission to create and edit clients; otherwise Harvest answers 403 Forbidden.
/// </summary>
public interface IContactApi
{
	/// <summary>Lists one page of contacts, most recently created first.</summary>
	/// <param name="options">Filtering (<c>client_id</c>, <c>updated_since</c>) and paging, or <see langword="null"/> for none.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of contacts.</returns>
	[Get("/v2/contacts")]
	Task<ContactsContainer> ListAsync([Query] ContactListOptions? options, CancellationToken cancellationToken);

	/// <summary>Gets a contact.</summary>
	/// <param name="id">The contact ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The contact.</returns>
	[Get("/v2/contacts/{id}")]
	Task<Contact> GetAsync(long id, CancellationToken cancellationToken);

	/// <summary>Creates a contact.</summary>
	/// <param name="creationDto">The new contact.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The created contact.</returns>
	[Post("/v2/contacts")]
	Task<Contact> CreateAsync([Body] ContactCreationDto creationDto, CancellationToken cancellationToken);

	/// <summary>Updates a contact; fields left <see langword="null"/> are unchanged.</summary>
	/// <param name="id">The contact ID.</param>
	/// <param name="patchDto">The fields to change.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The updated contact.</returns>
	[Patch("/v2/contacts/{id}")]
	Task<Contact> UpdateAsync(long id, [Body] ContactPatchDto patchDto, CancellationToken cancellationToken);

	/// <summary>Deletes a contact.</summary>
	/// <param name="id">The contact ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A task that completes when the contact is deleted.</returns>
	[Delete("/v2/contacts/{id}")]
	System.Threading.Tasks.Task DeleteAsync(long id, CancellationToken cancellationToken);
}