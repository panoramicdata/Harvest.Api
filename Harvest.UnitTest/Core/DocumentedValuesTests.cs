namespace Harvest.UnitTest.Core;

/// <summary>Pins the string values classes to the wire values in Harvest's documentation.</summary>
public class DocumentedValuesTests
{
	[Fact]
	public void InvoiceState_HasTheDocumentedValues()
		=> new[] { InvoiceState.Draft, InvoiceState.Open, InvoiceState.Paid, InvoiceState.Closed }
			.Should().Equal("draft", "open", "paid", "closed");

	[Fact]
	public void InvoicePaymentTerm_HasTheDocumentedValues()
		=> new[]
		{
			InvoicePaymentTerm.UponReceipt, InvoicePaymentTerm.Net15, InvoicePaymentTerm.Net30, InvoicePaymentTerm.Net45,
			InvoicePaymentTerm.Net60, InvoicePaymentTerm.Custom
		}.Should().Equal("upon receipt", "net 15", "net 30", "net 45", "net 60", "custom");

	[Fact]
	public void InvoicePaymentOption_HasTheDocumentedValues()
		=> new[] { InvoicePaymentOption.Ach, InvoicePaymentOption.CreditCard, InvoicePaymentOption.Paypal }
			.Should().Equal("ach", "credit_card", "paypal");

	[Fact]
	public void InvoiceTimeSummaryType_HasTheDocumentedValues()
		=> new[] { InvoiceTimeSummaryType.Project, InvoiceTimeSummaryType.Task, InvoiceTimeSummaryType.People, InvoiceTimeSummaryType.Detailed }
			.Should().Equal("project", "task", "people", "detailed");

	[Fact]
	public void InvoiceExpenseSummaryType_HasTheDocumentedValues()
		=> new[] { InvoiceExpenseSummaryType.Project, InvoiceExpenseSummaryType.Category, InvoiceExpenseSummaryType.People, InvoiceExpenseSummaryType.Detailed }
			.Should().Equal("project", "category", "people", "detailed");

	[Fact]
	public void InvoiceMessageEventType_HasTheDocumentedValues()
		=> new[] { InvoiceMessageEventType.Send, InvoiceMessageEventType.Close, InvoiceMessageEventType.Draft, InvoiceMessageEventType.ReOpen }
			.Should().Equal("send", "close", "draft", "re-open");

	[Fact]
	public void PtoRequestStatus_HasTheDocumentedValues()
		=> new[]
		{
			PtoRequestStatus.Pending, PtoRequestStatus.Approved, PtoRequestStatus.Declined, PtoRequestStatus.ChangesRequested,
			PtoRequestStatus.Cancelled
		}.Should().Equal("pending", "approved", "declined", "changes_requested", "cancelled");

	[Fact]
	public void PtoAccrualType_HasTheDocumentedValues()
		=> new[] { PtoAccrualType.Fixed, PtoAccrualType.Accrued, PtoAccrualType.Unlimited }
			.Should().Equal("fixed", "accrued", "unlimited");

	[Fact]
	public void PtoAccrualSchedule_HasTheDocumentedValues()
		=> new[] { PtoAccrualSchedule.Monthly, PtoAccrualSchedule.Biweekly, PtoAccrualSchedule.Semimonthly, PtoAccrualSchedule.Hourly }
			.Should().Equal("monthly", "biweekly", "semimonthly", "hourly");

	[Fact]
	public void PtoConflictResolution_HasTheDocumentedValues()
		=> new[] { PtoConflictResolution.Skip, PtoConflictResolution.Replace }
			.Should().Equal("skip", "replace");
}