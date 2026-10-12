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

	/// <summary>Invoice item categories.</summary>
	IInvoiceItemCategoryApi InvoiceItemCategories { get; }

	/// <summary>Invoices.</summary>
	IInvoiceApi Invoices { get; }

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