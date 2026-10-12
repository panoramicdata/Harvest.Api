namespace Harvest.Interfaces;

/// <summary>
/// Invoices (<see href="https://help.getharvest.com/api-v2/invoices-api/invoices/invoices/"/>). Requires an Administrator,
/// or a Manager with permission to create and edit invoices.
/// </summary>
public interface IInvoiceApi
{
	/// <summary>Lists one page of invoices, most recently issued first.</summary>
	/// <param name="options">Filtering and paging, or <see langword="null"/> for none.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of invoices.</returns>
	[Get("/v2/invoices")]
	Task<InvoicesContainer> ListAsync([Query] InvoiceListOptions? options, CancellationToken cancellationToken);

	/// <summary>Gets an invoice.</summary>
	/// <param name="id">The invoice ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The invoice.</returns>
	[Get("/v2/invoices/{id}")]
	Task<Invoice> GetAsync(long id, CancellationToken cancellationToken);

	/// <summary>
	/// Creates an invoice: a free-form invoice from <see cref="BillingDocumentDto{TLineItem}.LineItems"/>, or one based on tracked time
	/// and expenses from <see cref="InvoiceCreationDto.LineItemsImport"/>.
	/// </summary>
	/// <param name="creationDto">The new invoice.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The created invoice.</returns>
	[Post("/v2/invoices")]
	Task<Invoice> CreateAsync([Body] InvoiceCreationDto creationDto, CancellationToken cancellationToken);

	/// <summary>
	/// Updates an invoice; fields left <see langword="null"/> are unchanged. Also adds, updates and deletes line items through
	/// <see cref="BillingDocumentDto{TLineItem}.LineItems"/>.
	/// </summary>
	/// <param name="id">The invoice ID.</param>
	/// <param name="patchDto">The fields to change.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The updated invoice.</returns>
	[Patch("/v2/invoices/{id}")]
	Task<Invoice> UpdateAsync(long id, [Body] InvoicePatchDto patchDto, CancellationToken cancellationToken);

	/// <summary>Deletes an invoice.</summary>
	/// <param name="id">The invoice ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A task that completes when the invoice is deleted.</returns>
	[Delete("/v2/invoices/{id}")]
	System.Threading.Tasks.Task DeleteAsync(long id, CancellationToken cancellationToken);
}