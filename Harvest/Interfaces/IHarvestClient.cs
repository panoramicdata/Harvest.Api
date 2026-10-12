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

	/// <summary>Client contacts.</summary>
	IContactApi Contacts { get; }

	/// <summary>The cost rates of users.</summary>
	ICostRateApi CostRates { get; }

	/// <summary>Estimate item categories.</summary>
	IEstimateItemCategoryApi EstimateItemCategories { get; }

	/// <summary>The messages of estimates, which also mark estimates sent, accepted, declined or re-opened.</summary>
	IEstimateMessageApi EstimateMessages { get; }

	/// <summary>Estimates.</summary>
	IEstimateApi Estimates { get; }

	/// <summary>Expense categories.</summary>
	IExpenseCategoryApi ExpenseCategories { get; }

	/// <summary>Expenses, with their receipts.</summary>
	IExpenseApi Expenses { get; }

	/// <summary>Invoice item categories.</summary>
	IInvoiceItemCategoryApi InvoiceItemCategories { get; }

	/// <summary>The messages of invoices.</summary>
	IInvoiceMessageApi InvoiceMessages { get; }

	/// <summary>The payments of invoices.</summary>
	IInvoicePaymentApi InvoicePayments { get; }

	/// <summary>Invoices.</summary>
	IInvoiceApi Invoices { get; }

	/// <summary>The task assignments of projects.</summary>
	IProjectTaskAssignmentApi ProjectTaskAssignments { get; }

	/// <summary>The user assignments of projects.</summary>
	IProjectUserAssignmentApi ProjectUserAssignments { get; }

	/// <summary>Projects.</summary>
	IProjectApi Projects { get; }

	/// <summary>PTO user allocations (403 for accounts without the gradually rolled-out PTO API).</summary>
	IPtoAllocationApi PtoAllocations { get; }

	/// <summary>PTO user assignments of holiday calendars and work schedules (403 for accounts without the gradually rolled-out PTO API).</summary>
	IPtoAssignmentApi PtoAssignments { get; }

	/// <summary>PTO time off balances (403 for accounts without the gradually rolled-out PTO API).</summary>
	IPtoBalanceApi PtoBalances { get; }

	/// <summary>PTO holiday calendars and their entries (403 for accounts without the gradually rolled-out PTO API).</summary>
	IPtoHolidayCalendarApi PtoHolidayCalendars { get; }

	/// <summary>PTO time off requests (403 for accounts without the gradually rolled-out PTO API).</summary>
	IPtoRequestApi PtoRequests { get; }

	/// <summary>PTO time off policies (PTO types; 403 for accounts without the gradually rolled-out PTO API).</summary>
	IPtoTypeApi PtoTypes { get; }

	/// <summary>PTO work schedules (403 for accounts without the gradually rolled-out PTO API).</summary>
	IPtoWorkScheduleApi PtoWorkSchedules { get; }

	/// <summary>Time, expense, uninvoiced and project budget reports (rate limited to 100 requests per 15 minutes).</summary>
	IReportApi Reports { get; }

	/// <summary>Roles.</summary>
	IRoleApi Roles { get; }

	/// <summary>Tasks.</summary>
	ITaskApi Tasks { get; }

	/// <summary>The teammates assigned to managers.</summary>
	ITeammateApi Teammates { get; }

	/// <summary>Time entries.</summary>
	ITimeEntryApi TimeEntries { get; }

	/// <summary>The project assignments of users.</summary>
	IUserProjectAssignmentApi UserProjectAssignments { get; }

	/// <summary>Users.</summary>
	IUserApi Users { get; }

}