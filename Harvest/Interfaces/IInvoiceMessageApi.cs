namespace Harvest.Interfaces;

/// <summary>
/// The messages of an invoice (<see href="https://help.getharvest.com/api-v2/invoices-api/invoices/invoice-messages/"/>):
/// sending it, and recording that it was sent, closed, re-opened or returned to draft. Requires an Administrator, or a
/// Manager with permission to create and edit invoices.
/// </summary>
public interface IInvoiceMessageApi
{
	/// <summary>Lists one page of an invoice's messages, most recently created first.</summary>
	/// <param name="invoiceId">The invoice ID.</param>
	/// <param name="options">Filtering and paging (<c>updated_since</c>, <c>cursor</c>, <c>page</c>, <c>per_page</c>), or <see langword="null"/> for none.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of invoice messages.</returns>
	[Get("/v2/invoices/{invoiceId}/messages")]
	Task<InvoiceMessagesContainer> ListAsync(long invoiceId, [Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>
	/// Gets the subject and body Harvest has configured for a new message on an invoice, without creating one: a general
	/// message, or a thank-you or reminder message as <paramref name="options"/> selects.
	/// </summary>
	/// <param name="invoiceId">The invoice ID.</param>
	/// <param name="options">The kind of message, or <see langword="null"/> for a general invoice message.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The message subject and body.</returns>
	[Get("/v2/invoices/{invoiceId}/messages/new")]
	Task<InvoiceMessageTemplate> GetTemplateAsync(long invoiceId, [Query] InvoiceMessageTemplateOptions? options, CancellationToken cancellationToken);

	/// <summary>
	/// Creates an invoice message. Without <see cref="InvoiceMessageCreationDto.EventType"/> the message is sent; with one
	/// (<see cref="InvoiceMessageEventType"/>) the invoice is marked as sent, closed, re-opened or returned to draft instead.
	/// </summary>
	/// <param name="invoiceId">The invoice ID.</param>
	/// <param name="creationDto">The new message.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The created invoice message.</returns>
	[Post("/v2/invoices/{invoiceId}/messages")]
	Task<InvoiceMessage> CreateAsync(long invoiceId, [Body] InvoiceMessageCreationDto creationDto, CancellationToken cancellationToken);

	/// <summary>Deletes an invoice message.</summary>
	/// <param name="invoiceId">The invoice ID.</param>
	/// <param name="id">The message ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A task that completes when the message is deleted.</returns>
	[Delete("/v2/invoices/{invoiceId}/messages/{id}")]
	System.Threading.Tasks.Task DeleteAsync(long invoiceId, long id, CancellationToken cancellationToken);
}