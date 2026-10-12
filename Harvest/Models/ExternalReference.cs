using Harvest.Serialization;
using System.Text.Json.Serialization;

namespace Harvest.Models;

/// <summary>
/// A link from a time entry to an item in another service (an issue tracker, for example), as Harvest returns it. Harvest
/// documents every field as a string; a numeric value is read as its text.
/// </summary>
public class ExternalReference
{
	/// <summary>The ID of the item in the external service.</summary>
	[JsonConverter(typeof(FlexibleStringConverter))]
	public string? Id { get; set; }

	/// <summary>The ID of the group the item belongs to (a project or repository, for example).</summary>
	[JsonConverter(typeof(FlexibleStringConverter))]
	public string? GroupId { get; set; }

	/// <summary>The ID of the account in the external service.</summary>
	[JsonConverter(typeof(FlexibleStringConverter))]
	public string? AccountId { get; set; }

	/// <summary>The URL of the item.</summary>
	public string? Permalink { get; set; }

	/// <summary>The external service, for example <c>github.com</c>.</summary>
	public string? Service { get; set; }

	/// <summary>The URL of the service's icon.</summary>
	public string? ServiceIconUrl { get; set; }
}