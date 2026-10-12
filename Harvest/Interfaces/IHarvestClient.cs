namespace Harvest.Interfaces;

/// <summary>
/// The Harvest API v2, one property per resource. <see cref="HarvestClient"/> implements it; depend on this interface to
/// substitute a fake in tests.
/// </summary>
public interface IHarvestClient : IDisposable
{
	/// <summary>Clients.</summary>
	IClientApi Clients { get; }

	/// <summary>The company of the authenticated user.</summary>
	ICompanyApi Companies { get; }

	/// <summary>Estimate item categories.</summary>
	IEstimateItemCategoryApi EstimateItemCategories { get; }

	/// <summary>The messages of estimates, which also mark estimates sent, accepted, declined or re-opened.</summary>
	IEstimateMessageApi EstimateMessages { get; }

	/// <summary>Estimates.</summary>
	IEstimateApi Estimates { get; }

	/// <summary>Projects.</summary>
	IProjectApi Projects { get; }

	/// <summary>The task assignments of projects.</summary>
	IProjectTaskAssignmentApi ProjectTaskAssignments { get; }

	/// <summary>Tasks.</summary>
	ITaskApi Tasks { get; }

	/// <summary>Time entries.</summary>
	ITimeEntryApi TimeEntries { get; }

	/// <summary>Users.</summary>
	IUserApi Users { get; }

	/// <summary>The project assignments of users.</summary>
	IUserProjectAssignmentApi UserProjectAssignments { get; }
}