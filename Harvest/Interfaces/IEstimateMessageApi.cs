namespace Harvest.Interfaces;

/// <summary>
/// The messages of estimates, which also drive their state (<see href="https://help.getharvest.com/api-v2/estimates-api/estimates/estimate-messages/"/>).
/// Requires an Administrator, or a Manager with permission to create and edit estimates.
/// </summary>
public interface IEstimateMessageApi
{
	/// <summary>Lists one page of an estimate's messages, most recently created first.</summary>
	/// <param name="estimateId">The estimate ID.</param>
	/// <param name="options">Filtering (<c>updated_since</c>) and paging, or <see langword="null"/> for none.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of estimate messages.</returns>
	[Get("/v2/estimates/{estimateId}/messages")]
	Task<EstimateMessagesContainer> ListAsync(long estimateId, [Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>
	/// Creates an estimate message: emails it to <see cref="EstimateMessageCreationDto.Recipients"/> and, when
	/// <see cref="EstimateMessageCreationDto.EventType"/> is set, runs that event against the estimate. To change the
	/// estimate's state without emailing anyone, send only an event (<see cref="EstimateMessageCreationDto.ForEvent"/>):
	/// <see cref="EstimateMessageEventType.Send"/> marks a draft as sent, <see cref="EstimateMessageEventType.Accept"/> and
	/// <see cref="EstimateMessageEventType.Decline"/> mark an open estimate accepted or declined, and
	/// <see cref="EstimateMessageEventType.ReOpen"/> re-opens a closed one.
	/// </summary>
	/// <param name="estimateId">The estimate ID.</param>
	/// <param name="creationDto">The new message.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The created estimate message.</returns>
	[Post("/v2/estimates/{estimateId}/messages")]
	Task<EstimateMessage> CreateAsync(long estimateId, [Body] EstimateMessageCreationDto creationDto, CancellationToken cancellationToken);

	/// <summary>Deletes an estimate message.</summary>
	/// <param name="estimateId">The estimate ID.</param>
	/// <param name="id">The estimate message ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A task that completes when the estimate message is deleted.</returns>
	[Delete("/v2/estimates/{estimateId}/messages/{id}")]
	System.Threading.Tasks.Task DeleteAsync(long estimateId, long id, CancellationToken cancellationToken);
}