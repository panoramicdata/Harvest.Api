using Harvest.Dtos;

namespace Harvest.Models;

/// <summary>The body of <c>POST /v2/expense_categories</c>.</summary>
public class ExpenseCategoryCreationDto : CreationDto
{
	/// <summary>The name of the expense category.</summary>
	public required string Name { get; set; }

	/// <summary>The unit name of the expense category.</summary>
	public string? UnitName { get; set; }

	/// <summary>The unit price of the expense category.</summary>
	public decimal? UnitPrice { get; set; }

	/// <summary>Whether the expense category is active or archived; Harvest's default is <see langword="true"/>.</summary>
	public bool? IsActive { get; set; }
}

/// <summary>The body of <c>PATCH /v2/expense_categories/{id}</c>.</summary>
public class ExpenseCategoryPatchDto : PatchDto
{
	/// <summary>The name of the expense category.</summary>
	public string? Name { get; set; }

	/// <summary>The unit name of the expense category.</summary>
	public string? UnitName { get; set; }

	/// <summary>The unit price of the expense category.</summary>
	public decimal? UnitPrice { get; set; }

	/// <summary>Whether the expense category is active or archived.</summary>
	public bool? IsActive { get; set; }
}