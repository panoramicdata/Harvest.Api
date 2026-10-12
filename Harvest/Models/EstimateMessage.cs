namespace Harvest.Models;

/// <summary>An estimate message (<see href="https://help.getharvest.com/api-v2/estimates-api/estimates/estimate-messages/"/>).</summary>
public class EstimateMessage
{
	/// <summary>Unique ID for the message.</summary>
	public long Id { get; set; }

	/// <summary>Name of the user that created the message.</summary>
	public string? SentBy { get; set; }

	/// <summary>Email of the user that created the message.</summary>
	public string? SentByEmail { get; set; }

	/// <summary>Name of the user that the message was sent from.</summary>
	public string? SentFrom { get; set; }

	/// <summary>Email of the user that message was sent from.</summary>
	public string? SentFromEmail { get; set; }

	/// <summary>The message recipients; empty for an event-only message.</summary>
	public List<MessageRecipient>? Recipients { get; set; }

	/// <summary>The message subject.</summary>
	public string? Subject { get; set; }

	/// <summary>The message body.</summary>
	public string? Body { get; set; }

	/// <summary>Whether to email a copy of the message to the current user.</summary>
	public bool SendMeACopy { get; set; }

	/// <summary>
	/// The type of estimate event that occurred with the message: one of the <see cref="EstimateMessageEventType"/> values
	/// (<c>send</c>, <c>accept</c>, <c>decline</c>, <c>re-open</c>, <c>view</c> or <c>invoice</c>), or
	/// <see langword="null"/> for a plain message.
	/// </summary>
	public string? EventType { get; set; }

	/// <summary>Date and time the message was created.</summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>Date and time the message was last updated.</summary>
	public DateTime UpdatedAt { get; set; }
}