namespace Harvest.Models;

/// <summary>A page of <c>GET /v2/expense_categories</c>.</summary>
public class ExpenseCategoriesContainer : ListContainerBase
{
	/// <summary>The expense categories on this page (<c>expense_categories</c>).</summary>
	public List<ExpenseCategory> ExpenseCategories { get; set; } = [];
}