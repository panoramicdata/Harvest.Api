namespace Harvest.Interfaces;

/// <summary>
/// The payments of an invoice (<see href="https://help.getharvest.com/api-v2/invoices-api/invoices/invoice-payments/"/>).
/// Requires an Administrator, or a Manager with permission to create and edit invoices.
/// </summary>
public interface IInvoicePaymentApi
{
	/// <summary>Lists one page of an invoice's payments, most recently created first.</summary>
	/// <param name="invoiceId">The invoice ID.</param>
	/// <param name="options">Filtering and paging (<c>updated_since</c>, <c>cursor</c>, <c>page</c>, <c>per_page</c>), or <see langword="null"/> for none.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of invoice payments.</returns>
	[Get("/v2/invoices/{invoiceId}/payments")]
	Task<InvoicePaymentsContainer> ListAsync(long invoiceId, [Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>Records a payment on an invoice.</summary>
	/// <param name="invoiceId">The invoice ID.</param>
	/// <param name="creationDto">The new payment.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The created invoice payment.</returns>
	[Post("/v2/invoices/{invoiceId}/payments")]
	Task<InvoicePayment> CreateAsync(long invoiceId, [Body] InvoicePaymentCreationDto creationDto, CancellationToken cancellationToken);

	/// <summary>Deletes an invoice payment.</summary>
	/// <param name="invoiceId">The invoice ID.</param>
	/// <param name="id">The payment ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A task that completes when the payment is deleted.</returns>
	[Delete("/v2/invoices/{invoiceId}/payments/{id}")]
	System.Threading.Tasks.Task DeleteAsync(long invoiceId, long id, CancellationToken cancellationToken);
}