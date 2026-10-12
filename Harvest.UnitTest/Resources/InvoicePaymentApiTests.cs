using Harvest.UnitTest.Support;

namespace Harvest.UnitTest.Resources;

public class InvoicePaymentApiTests
{
	// "List all payments for an invoice" example from the Harvest docs (e-mail address substituted), with a transaction ID.
	private const string PaymentJson = """
		{"id":10112854,"amount":10700,"paid_at":"2017-02-21T00:00:00Z","paid_date":"2017-02-21","recorded_by":"Alice Doe",
		"recorded_by_email":"alice@example.com","notes":"Paid via check #4321","transaction_id":"TX-1",
		"created_at":"2017-06-27T16:24:57Z","updated_at":"2017-06-27T16:25:03Z",
		"payment_gateway":{"id":1234,"name":"Linkpoint International"}}
		""";

	// "Create an invoice payment" example from the Harvest docs: a hand-recorded payment, with a null gateway.
	private const string RecordedPaymentJson = """
		{"id":10336386,"amount":1575.86,"paid_at":"2017-07-24T13:32:18Z","paid_date":"2017-07-24","recorded_by":"Jane Bar",
		"recorded_by_email":"jane@example.com","notes":"Paid by phone","transaction_id":null,"created_at":"2017-07-28T14:42:44Z",
		"updated_at":"2017-07-28T14:42:44Z","payment_gateway":{"id":null,"name":null}}
		""";

	[Fact]
	public async Task ListAsync_NoOptions_SendsNoQuery()
	{
		var call = await TestClient.SendAsync((c, ct) => c.InvoicePayments.ListAsync(13150378, null, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/invoices/13150378/payments");
	}

	[Fact]
	public async Task ListAsync_AllOptions_SendsEveryParameter()
	{
		var call = await TestClient.SendAsync((c, ct) => c.InvoicePayments.ListAsync(13150378, new ListOptions
		{
			UpdatedSince = new DateTimeOffset(2017, 6, 27, 0, 0, 0, TimeSpan.Zero),
			Cursor = "x",
			Page = 1,
			PerPage = 2000
		}, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/invoices/13150378/payments",
			"?updated_since=2017-06-27T00%3A00%3A00Z&cursor=x&page=1&per_page=2000");
	}

	[Fact]
	public async Task ListAsync_MapsThePaymentsAndPaging()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.InvoicePayments.ListAsync(13150378, null, ct), $$$"""
			{"invoice_payments":[{{{PaymentJson}}},{{{RecordedPaymentJson}}}],"per_page":2000,"total_pages":1,"total_entries":2,
			"next_page":null,"previous_page":null,"page":1,"links":{"first":"https://api.harvestapp.com/v2/invoices/13150378/payments?page=1&per_page=2000",
			"next":null,"previous":null,"last":"https://api.harvestapp.com/v2/invoices/13150378/payments?page=1&per_page=2000"}}
			""");

		page.InvoicePayments.Should().HaveCount(2);
		ShouldBeThePayment(page.InvoicePayments[0]);
		ShouldBeTheRecordedPayment(page.InvoicePayments[1]);
		page.TotalEntries.Should().Be(2);
	}

	[Fact]
	public async Task CreateAsync_DocsExample_SendsThePaymentAndMapsIt()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.Created, RecordedPaymentJson);
		using var client = TestClient.Create(stub);

		var payment = await client.InvoicePayments.CreateAsync(13150378, new InvoicePaymentCreationDto
		{
			Amount = 1575.86m,
			PaidAt = new DateTime(2017, 7, 24, 13, 32, 18, DateTimeKind.Utc),
			Notes = "Paid by phone"
		}, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Subject.ShouldBe(HttpMethod.Post, "/v2/invoices/13150378/payments",
			body: """{"amount":1575.86,"paid_at":"2017-07-24T13:32:18Z","notes":"Paid by phone"}""");
		ShouldBeTheRecordedPayment(payment);
	}

	[Fact]
	public async Task CreateAsync_RequiredOnly_SendsAmount()
	{
		var call = await TestClient.SendAsync((c, ct) => c.InvoicePayments.CreateAsync(1, new InvoicePaymentCreationDto { Amount = 10m }, ct), PaymentJson);

		call.ShouldBe(HttpMethod.Post, "/v2/invoices/1/payments", body: """{"amount":10}""");
	}

	[Fact]
	public async Task CreateAsync_PaidDateAndThankYou_SendsThem()
	{
		var call = await TestClient.SendAsync((c, ct) => c.InvoicePayments.CreateAsync(1, new InvoicePaymentCreationDto
		{
			Amount = 10.5m,
			PaidDate = new DateOnly(2025, 6, 7),
			Notes = "n",
			SendThankYou = false
		}, ct), PaymentJson);

		call.ShouldBe(HttpMethod.Post, "/v2/invoices/1/payments",
			body: """{"amount":10.5,"paid_date":"2025-06-07","notes":"n","send_thank_you":false}""");
	}

	[Fact]
	public async Task DeleteAsync_SendsDelete()
	{
		var call = await TestClient.SendAsync((c, ct) => c.InvoicePayments.DeleteAsync(13150378, 10336386, ct), "");

		call.ShouldBe(HttpMethod.Delete, "/v2/invoices/13150378/payments/10336386");
	}

	private static void ShouldBeThePayment(InvoicePayment payment)
	{
		payment.Id.Should().Be(10112854);
		payment.Amount.Should().Be(10700m);
		payment.PaidAt.Should().Be(new DateTime(2017, 2, 21, 0, 0, 0, DateTimeKind.Utc));
		payment.PaidDate.Should().Be(new DateOnly(2017, 2, 21));
		payment.RecordedBy.Should().Be("Alice Doe");
		payment.RecordedByEmail.Should().Be("alice@example.com");
		payment.Notes.Should().Be("Paid via check #4321");
		payment.TransactionId.Should().Be("TX-1");
		payment.CreatedAt.Should().Be(new DateTime(2017, 6, 27, 16, 24, 57, DateTimeKind.Utc));
		payment.UpdatedAt.Should().Be(new DateTime(2017, 6, 27, 16, 25, 3, DateTimeKind.Utc));
		payment.PaymentGateway!.Id.Should().Be(1234);
		payment.PaymentGateway.Name.Should().Be("Linkpoint International");
	}

	private static void ShouldBeTheRecordedPayment(InvoicePayment payment)
	{
		payment.Id.Should().Be(10336386);
		payment.Amount.Should().Be(1575.86m);
		payment.PaidAt.Should().Be(new DateTime(2017, 7, 24, 13, 32, 18, DateTimeKind.Utc));
		payment.PaidDate.Should().Be(new DateOnly(2017, 7, 24));
		payment.RecordedBy.Should().Be("Jane Bar");
		payment.Notes.Should().Be("Paid by phone");
		payment.TransactionId.Should().BeNull();
		payment.PaymentGateway!.Id.Should().BeNull();
		payment.PaymentGateway.Name.Should().BeNull();
	}
}