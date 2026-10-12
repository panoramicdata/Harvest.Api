namespace Harvest.Interfaces;

/// <summary>
/// The Harvest API v2, one property per resource. <see cref="HarvestClient"/> implements it; depend on this interface to
/// substitute a fake in tests.
/// </summary>
public interface IHarvestClient : IDisposable
{
	/// <summary>
	/// The accounts the access token can reach, from Harvest ID (<see cref="HarvestClient.IdBaseAddress"/>); these requests
	/// do not send the <c>Harvest-Account-Id</c> header.
	/// </summary>
	IAccountApi Accounts { get; }

	/// <summary>The billable rates of users.</summary>
	IBillableRateApi BillableRates { get; }

	/// <summary>Clients.</summary>
	IClientApi Clients { get; }

	/// <summary>The company of the authenticated user.</summary>
	ICompanyApi Companies { get; }

	/// <summary>The cost rates of users.</summary>
	ICostRateApi CostRates { get; }

	/// <summary>Projects.</summary>
	IProjectApi Projects { get; }

	/// <summary>The task assignments of projects.</summary>
	IProjectTaskAssignmentApi ProjectTaskAssignments { get; }

	/// <summary>The user assignments of projects.</summary>
	IProjectUserAssignmentApi ProjectUserAssignments { get; }

	/// <summary>Roles.</summary>
	IRoleApi Roles { get; }

	/// <summary>Tasks.</summary>
	ITaskApi Tasks { get; }

	/// <summary>The teammates assigned to managers.</summary>
	ITeammateApi Teammates { get; }

	/// <summary>Time entries.</summary>
	ITimeEntryApi TimeEntries { get; }

	/// <summary>Users.</summary>
	IUserApi Users { get; }

	/// <summary>The project assignments of users.</summary>
	IUserProjectAssignmentApi UserProjectAssignments { get; }
}