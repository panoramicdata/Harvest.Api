namespace Harvest.Models;

/// <summary>
/// The values of an estimate message's <c>event_type</c>. They are strings rather than an enum so that a value Harvest
/// adds later is still read. <see cref="Send"/>, <see cref="Accept"/>, <see cref="Decline"/> and <see cref="ReOpen"/>
/// may be sent in <see cref="EstimateMessageCreationDto.EventType"/>; <see cref="View"/> and <see cref="Invoice"/> are
/// only read.
/// </summary>
public static class EstimateMessageEventType
{
	/// <summary>Marks a draft estimate as sent.</summary>
	public static string Send => "send";

	/// <summary>Marks an open estimate as accepted.</summary>
	public static string Accept => "accept";

	/// <summary>Marks an open estimate as declined.</summary>
	public static string Decline => "decline";

	/// <summary>Re-opens a closed estimate.</summary>
	public static string ReOpen => "re-open";

	/// <summary>The client viewed the estimate (read only).</summary>
	public static string View => "view";

	/// <summary>The estimate was invoiced (read only).</summary>
	public static string Invoice => "invoice";
}