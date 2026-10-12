namespace Harvest.Models;

/// <summary>
/// The values of an invoice's <c>state</c> (and of the <c>state</c> filter). They are strings rather than an enum so that a
/// value Harvest adds later is still read.
/// </summary>
public static class InvoiceState
{
	/// <summary>A draft invoice, not yet sent.</summary>
	public static string Draft => "draft";

	/// <summary>An open (sent, unpaid) invoice.</summary>
	public static string Open => "open";

	/// <summary>A fully paid invoice.</summary>
	public static string Paid => "paid";

	/// <summary>A closed (written-off) invoice.</summary>
	public static string Closed => "closed";
}

/// <summary>The values of an invoice's <c>payment_term</c>: the timeframe in which the invoice should be paid.</summary>
public static class InvoicePaymentTerm
{
	/// <summary>Due upon receipt.</summary>
	public static string UponReceipt => "upon receipt";

	/// <summary>Due in 15 days.</summary>
	public static string Net15 => "net 15";

	/// <summary>Due in 30 days.</summary>
	public static string Net30 => "net 30";

	/// <summary>Due in 45 days.</summary>
	public static string Net45 => "net 45";

	/// <summary>Due in 60 days.</summary>
	public static string Net60 => "net 60";

	/// <summary>A custom due date (Harvest's default when creating an invoice); required to set <c>due_date</c> yourself.</summary>
	public static string Custom => "custom";
}

/// <summary>The values of an invoice's <c>payment_options</c>: the online payment methods enabled for it.</summary>
public static class InvoicePaymentOption
{
	/// <summary>ACH bank transfer.</summary>
	public static string Ach => "ach";

	/// <summary>Credit card.</summary>
	public static string CreditCard => "credit_card";

	/// <summary>PayPal.</summary>
	public static string Paypal => "paypal";
}

/// <summary>
/// The values of <see cref="InvoiceTimeImportCreationDto.SummaryType"/>: how to summarize imported time entries per line
/// item.
/// </summary>
public static class InvoiceTimeSummaryType
{
	/// <summary>One line item per project.</summary>
	public static string Project => "project";

	/// <summary>One line item per task.</summary>
	public static string Task => "task";

	/// <summary>One line item per person.</summary>
	public static string People => "people";

	/// <summary>One line item per time entry.</summary>
	public static string Detailed => "detailed";
}

/// <summary>
/// The values of <see cref="InvoiceExpenseImportCreationDto.SummaryType"/>: how to summarize imported expenses per line
/// item.
/// </summary>
public static class InvoiceExpenseSummaryType
{
	/// <summary>One line item per project.</summary>
	public static string Project => "project";

	/// <summary>One line item per expense category.</summary>
	public static string Category => "category";

	/// <summary>One line item per person.</summary>
	public static string People => "people";

	/// <summary>One line item per expense.</summary>
	public static string Detailed => "detailed";
}

/// <summary>
/// The values of an invoice message's <c>event_type</c>. A message created without one (<see langword="null"/>) is sent
/// to its recipients; with one, it records the event instead of sending anything.
/// </summary>
public static class InvoiceMessageEventType
{
	/// <summary>Marks a draft invoice as sent (it does not send the message).</summary>
	public static string Send => "send";

	/// <summary>Marks an open invoice as closed (written off).</summary>
	public static string Close => "close";

	/// <summary>Marks an open invoice as a draft.</summary>
	public static string Draft => "draft";

	/// <summary>Re-opens a closed (written-off) invoice.</summary>
	public static string ReOpen => "re-open";
}