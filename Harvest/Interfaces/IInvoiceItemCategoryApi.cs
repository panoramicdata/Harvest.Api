namespace Harvest.Interfaces;

/// <summary>
/// Invoice item categories (<see href="https://help.getharvest.com/api-v2/invoices-api/invoices/invoice-item-categories/"/>).
/// Admin or Project Manager permissions are required.
/// </summary>
public interface IInvoiceItemCategoryApi
{
	/// <summary>Lists one page of invoice item categories, most recently created first.</summary>
	/// <param name="options">Filtering and paging (<c>updated_since</c>, <c>cursor</c>, <c>page</c>, <c>per_page</c>), or <see langword="null"/> for none.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of invoice item categories.</returns>
	[Get("/v2/invoice_item_categories")]
	Task<InvoiceItemCategoriesContainer> ListAsync([Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>Gets an invoice item category.</summary>
	/// <param name="id">The invoice item category ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The invoice item category.</returns>
	[Get("/v2/invoice_item_categories/{id}")]
	Task<InvoiceItemCategory> GetAsync(long id, CancellationToken cancellationToken);

	/// <summary>Creates an invoice item category.</summary>
	/// <param name="creationDto">The new invoice item category.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The created invoice item category.</returns>
	[Post("/v2/invoice_item_categories")]
	Task<InvoiceItemCategory> CreateAsync([Body] InvoiceItemCategoryCreationDto creationDto, CancellationToken cancellationToken);

	/// <summary>Updates an invoice item category; fields left <see langword="null"/> are unchanged.</summary>
	/// <param name="id">The invoice item category ID.</param>
	/// <param name="patchDto">The fields to change.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The updated invoice item category.</returns>
	[Patch("/v2/invoice_item_categories/{id}")]
	Task<InvoiceItemCategory> UpdateAsync(long id, [Body] InvoiceItemCategoryPatchDto patchDto, CancellationToken cancellationToken);

	/// <summary>
	/// Deletes an invoice item category. Harvest only allows this when <see cref="InvoiceItemCategory.UseAsService"/> and
	/// <see cref="InvoiceItemCategory.UseAsExpense"/> are both <see langword="false"/>.
	/// </summary>
	/// <param name="id">The invoice item category ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A task that completes when the invoice item category is deleted.</returns>
	[Delete("/v2/invoice_item_categories/{id}")]
	System.Threading.Tasks.Task DeleteAsync(long id, CancellationToken cancellationToken);
}