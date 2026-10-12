namespace Harvest.Models;

/// <summary>A page of <c>GET /v2/projects</c>.</summary>
public class ProjectsContainer : ListContainerBase
{
	/// <summary>The projects on this page (<c>projects</c>).</summary>
	public List<Project> Projects { get; set; } = [];
}