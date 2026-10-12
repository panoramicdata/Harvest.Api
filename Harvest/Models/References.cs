namespace Harvest.Models;

/// <summary>The <c>{id, name}</c> summary of a user that Harvest nests in other objects (time entries, user assignments).</summary>
public class UserReference
{
	/// <summary>Unique ID for the user.</summary>
	public long Id { get; set; }

	/// <summary>The user's full name.</summary>
	public string? Name { get; set; }
}

/// <summary>The <c>{id, name, currency}</c> summary of a client that Harvest nests in other objects.</summary>
public class ClientReference
{
	/// <summary>Unique ID for the client.</summary>
	public long Id { get; set; }

	/// <summary>The client's name.</summary>
	public string? Name { get; set; }

	/// <summary>The client's currency code, where Harvest includes it (projects).</summary>
	public string? Currency { get; set; }
}

/// <summary>The <c>{id, name, code}</c> summary of a project that Harvest nests in other objects.</summary>
public class ProjectReference
{
	/// <summary>Unique ID for the project.</summary>
	public long Id { get; set; }

	/// <summary>The project's name.</summary>
	public string? Name { get; set; }

	/// <summary>The project's code, where Harvest includes it.</summary>
	public string? Code { get; set; }
}

/// <summary>The <c>{id, name}</c> summary of a task that Harvest nests in other objects.</summary>
public class TaskReference
{
	/// <summary>Unique ID for the task.</summary>
	public long Id { get; set; }

	/// <summary>The task's name.</summary>
	public string? Name { get; set; }
}

/// <summary>The <c>{id, number}</c> summary of an invoice that Harvest nests in invoiced time entries.</summary>
public class InvoiceReference
{
	/// <summary>Unique ID for the invoice.</summary>
	public long Id { get; set; }

	/// <summary>The invoice number.</summary>
	public string? Number { get; set; }
}

/// <summary>The <c>{id, name, unit_price, unit_name}</c> summary of an expense category that Harvest nests in expenses.</summary>
public class ExpenseCategoryReference
{
	/// <summary>Unique ID for the expense category.</summary>
	public long Id { get; set; }

	/// <summary>The expense category's name.</summary>
	public string? Name { get; set; }

	/// <summary>The unit price of the expense category, or <see langword="null"/> if it is not unit-based.</summary>
	public decimal? UnitPrice { get; set; }

	/// <summary>The unit name of the expense category, or <see langword="null"/> if it is not unit-based.</summary>
	public string? UnitName { get; set; }
/// <summary>The <c>{id}</c> summary of an estimate that Harvest nests in invoices.</summary>
public class EstimateReference
{
	/// <summary>Unique ID for the estimate.</summary>
	public long Id { get; set; }
}

/// <summary>The <c>{id}</c> summary of a retainer that Harvest nests in invoices.</summary>
public class RetainerReference
{
	/// <summary>Unique ID for the retainer.</summary>
	public long Id { get; set; }
}

/// <summary>
/// The <c>{id, name}</c> summary of the payment gateway that processed an invoice payment. Both are <see langword="null"/>
/// for a payment recorded by hand.
/// </summary>
public class PaymentGatewayReference
{
	/// <summary>Unique ID for the payment gateway, or <see langword="null"/>.</summary>
	public long? Id { get; set; }

	/// <summary>The payment gateway's name, or <see langword="null"/>.</summary>
	public string? Name { get; set; }
}