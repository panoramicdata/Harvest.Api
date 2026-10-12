namespace Harvest.Interfaces;

/// <summary>
/// Expense categories (<see href="https://help.getharvest.com/api-v2/expenses-api/expenses/expense-categories/"/>).
/// Admin permissions required.
/// </summary>
public interface IExpenseCategoryApi
{
	/// <summary>Lists one page of expense categories, most recently created first.</summary>
	/// <param name="options">Filtering (<c>is_active</c>, <c>updated_since</c>) and paging, or <see langword="null"/> for none.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The page of expense categories.</returns>
	[Get("/v2/expense_categories")]
	Task<ExpenseCategoriesContainer> ListAsync([Query] ListOptions? options, CancellationToken cancellationToken);

	/// <summary>Gets an expense category.</summary>
	/// <param name="id">The expense category ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The expense category.</returns>
	[Get("/v2/expense_categories/{id}")]
	Task<ExpenseCategory> GetAsync(long id, CancellationToken cancellationToken);

	/// <summary>Creates an expense category.</summary>
	/// <param name="creationDto">The new expense category.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The created expense category.</returns>
	[Post("/v2/expense_categories")]
	Task<ExpenseCategory> CreateAsync([Body] ExpenseCategoryCreationDto creationDto, CancellationToken cancellationToken);

	/// <summary>Updates an expense category; fields left <see langword="null"/> are unchanged.</summary>
	/// <param name="id">The expense category ID.</param>
	/// <param name="patchDto">The fields to change.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The updated expense category.</returns>
	[Patch("/v2/expense_categories/{id}")]
	Task<ExpenseCategory> UpdateAsync(long id, [Body] ExpenseCategoryPatchDto patchDto, CancellationToken cancellationToken);

	/// <summary>Deletes an expense category.</summary>
	/// <param name="id">The expense category ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A task that completes when the expense category is deleted.</returns>
	[Delete("/v2/expense_categories/{id}")]
	System.Threading.Tasks.Task DeleteAsync(long id, CancellationToken cancellationToken);
}