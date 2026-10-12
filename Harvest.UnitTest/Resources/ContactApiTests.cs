using Harvest.UnitTest.Support;

namespace Harvest.UnitTest.Resources;

public class ContactApiTests
{
	private const string JaneJson = """
		{"id":4706479,"title":"Owner","first_name":"Jane","last_name":"Doe","email":"janedoe@example.com",
		"phone_office":"(203) 697-8885","phone_mobile":"(203) 697-8886","fax":"(203) 697-8887",
		"created_at":"2017-06-26T21:20:07Z","updated_at":"2017-06-26T21:27:07Z","client":{"id":5735774,"name":"ABC Corp"}}
		""";

	private const string GeorgeJson = """
		{"id":4706510,"title":null,"first_name":"George","last_name":"Frank","email":"georgefrank@example.com",
		"phone_office":"","phone_mobile":"","fax":"","invoice_recipient_status":"cc","created_at":"2019-06-26T21:44:57Z",
		"updated_at":"2019-06-26T21:44:57Z","client":{"id":5735776,"name":"123 Industries"}}
		""";

	[Fact]
	public async Task ListAsync_NoOptions_SendsNoQuery()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Contacts.ListAsync(null, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/contacts");
	}

	[Fact]
	public async Task ListAsync_AllOptions_SendsEveryParameter()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Contacts.ListAsync(new ContactListOptions
		{
			ClientId = 5735776,
			UpdatedSince = new DateTimeOffset(2017, 6, 26, 21, 52, 18, TimeSpan.Zero),
			Cursor = "abc",
			Page = 2,
			PerPage = 100
		}, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/contacts",
			"?client_id=5735776&updated_since=2017-06-26T21%3A52%3A18Z&cursor=abc&page=2&per_page=100");
	}

	[Fact]
	public async Task ListAsync_MapsTheContacts()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.Contacts.ListAsync(null, ct), $$$"""
			{"contacts":[{{{JaneJson}}},{{{GeorgeJson}}}],"per_page":2000,"total_pages":1,"total_entries":2,"next_page":null,
			"previous_page":null,"page":1,"links":{"first":"https://api.harvestapp.com/v2/contacts?page=1&per_page=2000","next":null,
			"previous":null,"last":"https://api.harvestapp.com/v2/contacts?page=1&per_page=2000"}}
			""");

		page.Contacts.Should().HaveCount(2);
		page.TotalEntries.Should().Be(2);
		ShouldBeJane(page.Contacts[0]);
		ShouldBeGeorge(page.Contacts[1]);
	}

	[Fact]
	public async Task GetAsync_SendsGetAndMapsTheContact()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.OK, JaneJson);
		using var client = TestClient.Create(stub);

		var contact = await client.Contacts.GetAsync(4706479, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Subject.ShouldBe(HttpMethod.Get, "/v2/contacts/4706479");
		ShouldBeJane(contact);
	}

	[Fact]
	public async Task CreateAsync_RequiredOnly_SendsRequiredFields()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Contacts.CreateAsync(new ContactCreationDto
		{
			ClientId = 5735776,
			FirstName = "George"
		}, ct), GeorgeJson);

		call.ShouldBe(HttpMethod.Post, "/v2/contacts", body: """{"client_id":5735776,"first_name":"George"}""");
	}

	[Fact]
	public async Task CreateAsync_AllFields_SendsEveryFieldAndMapsTheContact()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.Created, GeorgeJson);
		using var client = TestClient.Create(stub);

		var contact = await client.Contacts.CreateAsync(new ContactCreationDto
		{
			ClientId = 5735776,
			Title = "CFO",
			FirstName = "George",
			LastName = "Frank",
			Email = "georgefrank@example.com",
			PhoneOffice = "1",
			PhoneMobile = "2",
			Fax = "3",
			InvoiceRecipientStatus = InvoiceRecipientStatus.Cc
		}, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Subject.ShouldBe(HttpMethod.Post, "/v2/contacts", body: """
			{"client_id":5735776,"title":"CFO","first_name":"George","last_name":"Frank","email":"georgefrank@example.com","phone_office":"1","phone_mobile":"2","fax":"3","invoice_recipient_status":"cc"}
			""");
		ShouldBeGeorge(contact);
	}

	[Fact]
	public async Task UpdateAsync_AllFields_SendsPatch()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Contacts.UpdateAsync(4706510, new ContactPatchDto
		{
			ClientId = 1,
			Title = "Owner",
			FirstName = "G",
			LastName = "F",
			Email = "g@example.com",
			PhoneOffice = "4",
			PhoneMobile = "5",
			Fax = "6",
			InvoiceRecipientStatus = InvoiceRecipientStatus.Bcc
		}, ct), GeorgeJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/contacts/4706510", body: """
			{"client_id":1,"title":"Owner","first_name":"G","last_name":"F","email":"g@example.com","phone_office":"4","phone_mobile":"5","fax":"6","invoice_recipient_status":"bcc"}
			""");
	}

	[Fact]
	public async Task UpdateAsync_EmptyPatch_SendsEmptyObject()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Contacts.UpdateAsync(4706510, new ContactPatchDto(), ct), GeorgeJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/contacts/4706510", body: "{}");
	}

	[Fact]
	public async Task DeleteAsync_SendsDelete()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Contacts.DeleteAsync(4706510, ct), "");

		call.ShouldBe(HttpMethod.Delete, "/v2/contacts/4706510");
	}

	[Fact]
	public void InvoiceRecipientStatus_HasTheDocumentedValues()
	{
		new[] { InvoiceRecipientStatus.None, InvoiceRecipientStatus.Recipient, InvoiceRecipientStatus.Cc, InvoiceRecipientStatus.Bcc }
			.Should().Equal("none", "recipient", "cc", "bcc");
	}

	private static void ShouldBeJane(Contact contact)
	{
		contact.Id.Should().Be(4706479);
		contact.Title.Should().Be("Owner");
		contact.FirstName.Should().Be("Jane");
		contact.LastName.Should().Be("Doe");
		contact.Email.Should().Be("janedoe@example.com");
		contact.PhoneOffice.Should().Be("(203) 697-8885");
		contact.PhoneMobile.Should().Be("(203) 697-8886");
		contact.Fax.Should().Be("(203) 697-8887");
		contact.InvoiceRecipientStatus.Should().BeNull();
		contact.CreatedAt.Should().Be(new DateTime(2017, 6, 26, 21, 20, 7, DateTimeKind.Utc));
		contact.UpdatedAt.Should().Be(new DateTime(2017, 6, 26, 21, 27, 7, DateTimeKind.Utc));
		contact.Client!.Id.Should().Be(5735774);
		contact.Client.Name.Should().Be("ABC Corp");
		contact.Client.Currency.Should().BeNull();
	}

	private static void ShouldBeGeorge(Contact contact)
	{
		contact.Id.Should().Be(4706510);
		contact.Title.Should().BeNull();
		contact.FirstName.Should().Be("George");
		contact.LastName.Should().Be("Frank");
		contact.Email.Should().Be("georgefrank@example.com");
		contact.PhoneOffice.Should().BeEmpty();
		contact.PhoneMobile.Should().BeEmpty();
		contact.Fax.Should().BeEmpty();
		contact.InvoiceRecipientStatus.Should().Be(InvoiceRecipientStatus.Cc);
		contact.CreatedAt.Should().Be(new DateTime(2019, 6, 26, 21, 44, 57, DateTimeKind.Utc));
		contact.UpdatedAt.Should().Be(new DateTime(2019, 6, 26, 21, 44, 57, DateTimeKind.Utc));
		contact.Client!.Id.Should().Be(5735776);
		contact.Client.Name.Should().Be("123 Industries");
	}
}