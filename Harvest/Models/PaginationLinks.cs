using System.Text.Json.Serialization;
using System.Web;

namespace Harvest.Models;

/// <summary>The <c>links</c> object of a Harvest list response.</summary>
public class PaginationLinks
{
	/// <summary>The URL of the first page.</summary>
	public string? First { get; set; }

	/// <summary>The URL of the next page, or <see langword="null"/> on the last page.</summary>
	public string? Next { get; set; }

	/// <summary>The URL of the previous page, or <see langword="null"/> on the first page.</summary>
	public string? Previous { get; set; }

	/// <summary>The URL of the last page.</summary>
	public string? Last { get; set; }

	/// <summary>
	/// The <c>cursor</c> query value of <see cref="Next"/>, to pass as <see cref="ListOptions.Cursor"/> for the next page;
	/// <see langword="null"/> on the last page or when the next link is page-numbered rather than cursor-based.
	/// </summary>
	[JsonIgnore]
	public string? NextCursor
		=> Uri.TryCreate(Next, UriKind.Absolute, out var next) ? HttpUtility.ParseQueryString(next.Query)["cursor"] : null;
}