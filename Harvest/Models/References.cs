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