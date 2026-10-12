namespace Harvest.Models;

/// <summary>A page of <c>GET /v2/expenses</c>.</summary>
public class ExpensesContainer : ListContainerBase
{
	/// <summary>The expenses on this page (<c>expenses</c>).</summary>
	public List<Expense> Expenses { get; set; } = [];
}