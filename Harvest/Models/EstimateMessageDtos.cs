using Harvest.Dtos;

namespace Harvest.Models;

/// <summary>
/// The body of <c>POST /v2/estimates/{estimate_id}/messages</c>. Harvest documents <see cref="Recipients"/> as required
/// for a message, but a message that only runs an event (<see cref="ForEvent"/>) sends nothing but
/// <see cref="EventType"/>, so neither is a C# <c>required</c> member.
/// </summary>
public class EstimateMessageCreationDto : CreationDto
{
	/// <summary>The recipients of the message; required unless the message only runs an <see cref="EventType"/>.</summary>
	public List<MessageRecipientCreationDto>? Recipients { get; set; }

	/// <summary>The message subject.</summary>
	public string? Subject { get; set; }

	/// <summary>The message body.</summary>
	public string? Body { get; set; }

	/// <summary>
	/// If set to <see langword="true"/>, a copy of the message email will be sent to the current user; Harvest's default
	/// is <see langword="false"/>.
	/// </summary>
	public bool? SendMeACopy { get; set; }

	/// <summary>
	/// If provided, runs an event against the estimate: <see cref="EstimateMessageEventType.Accept"/>,
	/// <see cref="EstimateMessageEventType.Decline"/>, <see cref="EstimateMessageEventType.ReOpen"/> or
	/// <see cref="EstimateMessageEventType.Send"/>.
	/// </summary>
	public string? EventType { get; set; }

	/// <summary>A message that only runs <paramref name="eventType"/> against the estimate: <c>{"event_type":…}</c>.</summary>
	/// <param name="eventType">
	/// The event: <see cref="EstimateMessageEventType.Send"/> (mark a draft as sent),
	/// <see cref="EstimateMessageEventType.Accept"/> or <see cref="EstimateMessageEventType.Decline"/> (mark an open
	/// estimate accepted or declined), or <see cref="EstimateMessageEventType.ReOpen"/> (re-open a closed estimate).
	/// </param>
	/// <returns>The message.</returns>
	public static EstimateMessageCreationDto ForEvent(string eventType) => new() { EventType = eventType };
}