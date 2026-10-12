using Harvest.Serialization;
using System.ComponentModel;

namespace Harvest.Interfaces;

/// <summary>Expenses (<see href="https://help.getharvest.com/api-v2/expenses-api/expenses/expenses/"/>).</summary>
/// <remarks>
/// Creating or updating an expense with a receipt file needs a <c>multipart/form-data</c> request: use the overloads of
/// <see cref="CreateAsync(ExpenseCreationDto, StreamPart, CancellationToken)"/> and
/// <see cref="UpdateAsync(long, ExpensePatchDto, StreamPart, CancellationToken)"/> that take a <see cref="StreamPart"/>.
/// To remove a receipt, update with <see cref="ExpensePatchDto.DeleteReceipt"/> set to <see langword="true"/>.
/// </remarks>
public interface IExpenseApi
{
	/// <summary>
	/// Lists one page of expenses, most recently spent first. Administrators see every expense in the account; Managers see
	/// those of their assigned teammates and managed projects.
	/// </summary>
	/// <param name="options">Filtering and paging, or <see langword="null"/> for none.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of expenses.</returns>
	[Get("/v2/expenses")]
	Task<ExpensesContainer> ListAsync([Query] ExpenseListOptions? options, CancellationToken cancellationToken);

	/// <summary>Gets an expense.</summary>
	/// <param name="id">The expense ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The expense.</returns>
	[Get("/v2/expenses/{id}")]
	Task<Expense> GetAsync(long id, CancellationToken cancellationToken);

	/// <summary>Creates an expense without a receipt (a JSON request).</summary>
	/// <param name="creationDto">The new expense.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The created expense.</returns>
	[Post("/v2/expenses")]
	Task<Expense> CreateAsync([Body] ExpenseCreationDto creationDto, CancellationToken cancellationToken);

	/// <summary>Creates an expense with a receipt file attached (a <c>multipart/form-data</c> request).</summary>
	/// <param name="creationDto">The new expense.</param>
	/// <param name="receipt">The receipt file, for example <c>new StreamPart(stream, "receipt.pdf", "application/pdf")</c>.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The created expense.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="creationDto"/> or <paramref name="receipt"/> is <see langword="null"/>.</exception>
	Task<Expense> CreateAsync(ExpenseCreationDto creationDto, StreamPart receipt, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(creationDto);
		ArgumentNullException.ThrowIfNull(receipt);
		return CreateWithReceiptAsync(
			HarvestFormValue.Of(creationDto.UserId),
			HarvestFormValue.Of(creationDto.ProjectId)!,
			HarvestFormValue.Of(creationDto.ExpenseCategoryId)!,
			HarvestFormValue.Of(creationDto.SpentDate)!,
			HarvestFormValue.Of(creationDto.Units),
			HarvestFormValue.Of(creationDto.TotalCost),
			creationDto.Notes,
			HarvestFormValue.Of(creationDto.Billable),
			receipt,
			cancellationToken);
	}

	/// <summary>
	/// The <c>multipart/form-data</c> request behind <see cref="CreateAsync(ExpenseCreationDto, StreamPart, CancellationToken)"/>,
	/// with each field as Harvest's form text; a <see langword="null"/> field is omitted. Prefer that overload.
	/// </summary>
	/// <param name="userId">The <c>user_id</c> field.</param>
	/// <param name="projectId">The <c>project_id</c> field.</param>
	/// <param name="expenseCategoryId">The <c>expense_category_id</c> field.</param>
	/// <param name="spentDate">The <c>spent_date</c> field, as <c>YYYY-MM-DD</c>.</param>
	/// <param name="units">The <c>units</c> field.</param>
	/// <param name="totalCost">The <c>total_cost</c> field.</param>
	/// <param name="notes">The <c>notes</c> field.</param>
	/// <param name="billable">The <c>billable</c> field, <c>true</c> or <c>false</c>.</param>
	/// <param name="receipt">The <c>receipt</c> file.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The created expense.</returns>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Multipart]
	[Post("/v2/expenses")]
	Task<Expense> CreateWithReceiptAsync(
		[AliasAs("user_id")] string? userId,
		[AliasAs("project_id")] string projectId,
		[AliasAs("expense_category_id")] string expenseCategoryId,
		[AliasAs("spent_date")] string spentDate,
		[AliasAs("units")] string? units,
		[AliasAs("total_cost")] string? totalCost,
		[AliasAs("notes")] string? notes,
		[AliasAs("billable")] string? billable,
		[AliasAs("receipt")] StreamPart receipt,
		CancellationToken cancellationToken);

	/// <summary>Updates an expense (a JSON request); fields left <see langword="null"/> are unchanged.</summary>
	/// <param name="id">The expense ID.</param>
	/// <param name="patchDto">The fields to change.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The updated expense.</returns>
	[Patch("/v2/expenses/{id}")]
	Task<Expense> UpdateAsync(long id, [Body] ExpensePatchDto patchDto, CancellationToken cancellationToken);

	/// <summary>
	/// Updates an expense and attaches a receipt file (a <c>multipart/form-data</c> request); fields left
	/// <see langword="null"/> are unchanged.
	/// </summary>
	/// <param name="id">The expense ID.</param>
	/// <param name="patchDto">The fields to change.</param>
	/// <param name="receipt">The receipt file, for example <c>new StreamPart(stream, "receipt.pdf", "application/pdf")</c>.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The updated expense.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="patchDto"/> or <paramref name="receipt"/> is <see langword="null"/>.</exception>
	Task<Expense> UpdateAsync(long id, ExpensePatchDto patchDto, StreamPart receipt, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(patchDto);
		ArgumentNullException.ThrowIfNull(receipt);
		return UpdateWithReceiptAsync(
			id,
			HarvestFormValue.Of(patchDto.ProjectId),
			HarvestFormValue.Of(patchDto.ExpenseCategoryId),
			HarvestFormValue.Of(patchDto.SpentDate),
			HarvestFormValue.Of(patchDto.Units),
			HarvestFormValue.Of(patchDto.TotalCost),
			patchDto.Notes,
			HarvestFormValue.Of(patchDto.Billable),
			HarvestFormValue.Of(patchDto.DeleteReceipt),
			receipt,
			cancellationToken);
	}

	/// <summary>
	/// The <c>multipart/form-data</c> request behind <see cref="UpdateAsync(long, ExpensePatchDto, StreamPart, CancellationToken)"/>,
	/// with each field as Harvest's form text; a <see langword="null"/> field is omitted. Prefer that overload.
	/// </summary>
	/// <param name="id">The expense ID.</param>
	/// <param name="projectId">The <c>project_id</c> field.</param>
	/// <param name="expenseCategoryId">The <c>expense_category_id</c> field.</param>
	/// <param name="spentDate">The <c>spent_date</c> field, as <c>YYYY-MM-DD</c>.</param>
	/// <param name="units">The <c>units</c> field.</param>
	/// <param name="totalCost">The <c>total_cost</c> field.</param>
	/// <param name="notes">The <c>notes</c> field.</param>
	/// <param name="billable">The <c>billable</c> field, <c>true</c> or <c>false</c>.</param>
	/// <param name="deleteReceipt">The <c>delete_receipt</c> field, <c>true</c> or <c>false</c>.</param>
	/// <param name="receipt">The <c>receipt</c> file.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The updated expense.</returns>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Multipart]
	[Patch("/v2/expenses/{id}")]
	Task<Expense> UpdateWithReceiptAsync(
		long id,
		[AliasAs("project_id")] string? projectId,
		[AliasAs("expense_category_id")] string? expenseCategoryId,
		[AliasAs("spent_date")] string? spentDate,
		[AliasAs("units")] string? units,
		[AliasAs("total_cost")] string? totalCost,
		[AliasAs("notes")] string? notes,
		[AliasAs("billable")] string? billable,
		[AliasAs("delete_receipt")] string? deleteReceipt,
		[AliasAs("receipt")] StreamPart receipt,
		CancellationToken cancellationToken);

	/// <summary>Deletes an expense.</summary>
	/// <param name="id">The expense ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A task that completes when the expense is deleted.</returns>
	[Delete("/v2/expenses/{id}")]
	System.Threading.Tasks.Task DeleteAsync(long id, CancellationToken cancellationToken);
}