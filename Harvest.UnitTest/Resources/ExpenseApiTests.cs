using Harvest.UnitTest.Support;
using Refit;
using System.Globalization;
using System.Text;

namespace Harvest.UnitTest.Resources;

public class ExpenseApiTests
{
	private const string LunchJson = """
		{"id":15296442,"notes":"Lunch with client","total_cost":33.35,"units":1.0,"is_closed":false,"approval_status":"unsubmitted",
		"is_locked":true,"is_billed":true,"locked_reason":"Expense is invoiced.","spent_date":"2017-03-03",
		"created_at":"2017-06-27T15:09:54Z","updated_at":"2017-06-27T16:47:14Z","billable":true,
		"receipt":{"url":"https://{ACCOUNT_SUBDOMAIN}.harvestapp.com/expenses/15296442/receipt","file_name":"lunch_receipt.gif",
		"file_size":39410,"content_type":"image/gif"},"user":{"id":1782959,"name":"Kim Allen"},
		"user_assignment":{"id":125068553,"is_project_manager":true,"is_active":true,"budget":null,"created_at":"2017-06-26T22:32:52Z",
		"updated_at":"2017-06-26T22:32:52Z","hourly_rate":100.0},"project":{"id":14307913,"name":"Marketing Website","code":"MW"},
		"expense_category":{"id":4195926,"name":"Meals","unit_price":null,"unit_name":null},
		"client":{"id":5735774,"name":"ABC Corp","currency":"USD"},"invoice":{"id":13150403,"number":"1001"}}
		""";

	private const string HotelJson = """
		{"id":15296423,"notes":"Hotel stay for meeting","total_cost":100.0,"units":1.0,"is_closed":true,"approval_status":"approved",
		"is_locked":true,"is_billed":false,"locked_reason":"The project is locked for this time period.","spent_date":"2017-03-01",
		"created_at":"2017-06-27T15:09:17Z","updated_at":"2017-06-27T16:47:14Z","billable":true,"receipt":null,
		"user":{"id":1782959,"name":"Kim Allen"},"user_assignment":{"id":125068554,"is_project_manager":true,"is_active":true,
		"budget":null,"created_at":"2017-06-26T22:32:52Z","updated_at":"2017-06-26T22:32:52Z","hourly_rate":100.0},
		"project":{"id":14308069,"name":"Online Store - Phase 1","code":"OS1"},
		"expense_category":{"id":4197501,"name":"Lodging","unit_price":null,"unit_name":null},
		"client":{"id":5735776,"name":"123 Industries","currency":"EUR"},"invoice":null}
		""";

	private const string MileageJson = """
		{"id":15297032,"notes":null,"total_cost":5.35,"units":10.0,"is_closed":false,"approval_status":"unsubmitted","is_locked":false,
		"is_billed":false,"locked_reason":null,"spent_date":"2017-03-01","created_at":"2017-06-27T15:42:27Z",
		"updated_at":"2017-06-27T15:42:27Z","billable":false,"receipt":null,"user":{"id":1782959,"name":"Kim Allen"},
		"project":{"id":14308069,"name":"Online Store - Phase 1","code":"OS1"},
		"expense_category":{"id":4195930,"name":"Mileage","unit_price":0.535,"unit_name":"mile"},
		"client":{"id":5735776,"name":"123 Industries","currency":"EUR"},"invoice":null}
		""";

	// Refit's default multipart boundary; the delimiter line is "--" followed by it.
	private const string Boundary = "----MyGreatBoundary";

	private static readonly byte[] ReceiptBytes = Encoding.ASCII.GetBytes("GIF89a");

	[Fact]
	public async Task ListAsync_NoOptions_SendsNoQuery()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Expenses.ListAsync(null, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/expenses");
	}

	[Fact]
	public async Task ListAsync_AllOptions_SendsEveryParameter()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Expenses.ListAsync(new ExpenseListOptions
		{
			UserId = 1782959,
			ClientId = 5735776,
			ProjectId = 14308069,
			IsBilled = false,
			ApprovalStatus = ApprovalStatus.Submitted,
			From = new DateOnly(2017, 3, 1),
			To = new DateOnly(2017, 3, 31),
			UpdatedSince = new DateTimeOffset(2017, 6, 26, 21, 52, 18, TimeSpan.Zero),
			Cursor = "c1",
			Page = 2,
			PerPage = 100
		}, ct));

		call.ShouldBe(HttpMethod.Get, "/v2/expenses",
			"?user_id=1782959&client_id=5735776&project_id=14308069&is_billed=false&approval_status=submitted&from=2017-03-01&to=2017-03-31&updated_since=2017-06-26T21%3A52%3A18Z&cursor=c1&page=2&per_page=100");
	}

	[Fact]
	public async Task ListAsync_MapsTheExpenses()
	{
		var page = await TestClient.ReadAsync((c, ct) => c.Expenses.ListAsync(null, ct), $$$"""
			{"expenses":[{{{LunchJson}}},{{{HotelJson}}}],"per_page":2000,"total_pages":1,"total_entries":2,"next_page":null,
			"previous_page":null,"page":1,"links":{"first":"https://api.harvestapp.com/v2/expenses?page=1&per_page=2000","next":null,
			"previous":null,"last":"https://api.harvestapp.com/v2/expenses?page=1&per_page=2000"}}
			""");

		page.Expenses.Should().HaveCount(2);
		page.TotalEntries.Should().Be(2);
		ShouldBeTheLunch(page.Expenses[0]);
		var hotel = page.Expenses[1];
		hotel.Id.Should().Be(15296423);
		hotel.TotalCost.Should().Be(100m);
		hotel.IsClosed.Should().BeTrue();
		hotel.ApprovalStatus.Should().Be(ApprovalStatus.Approved);
		hotel.IsBilled.Should().BeFalse();
		hotel.LockedReason.Should().Be("The project is locked for this time period.");
		hotel.Receipt.Should().BeNull();
		hotel.Invoice.Should().BeNull();
		hotel.Client!.Currency.Should().Be("EUR");
		hotel.ExpenseCategory!.Name.Should().Be("Lodging");
	}

	[Fact]
	public async Task GetAsync_SendsGetAndMapsTheExpense()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.OK, LunchJson);
		using var client = TestClient.Create(stub);

		var expense = await client.Expenses.GetAsync(15296442, TestClient.Ct);

		stub.Calls.Should().ContainSingle().Subject.ShouldBe(HttpMethod.Get, "/v2/expenses/15296442");
		ShouldBeTheLunch(expense);
	}

	[Fact]
	public async Task GetAsync_UnitBasedExpenseWithoutUserAssignment_MapsIt()
	{
		var expense = await TestClient.ReadAsync((c, ct) => c.Expenses.GetAsync(15297032, ct), MileageJson);

		expense.Notes.Should().BeNull();
		expense.Units.Should().Be(10m);
		expense.TotalCost.Should().Be(5.35m);
		expense.Billable.Should().BeFalse();
		expense.IsLocked.Should().BeFalse();
		expense.LockedReason.Should().BeNull();
		expense.UserAssignment.Should().BeNull();
		expense.ExpenseCategory!.Id.Should().Be(4195930);
		expense.ExpenseCategory.UnitPrice.Should().Be(0.535m);
		expense.ExpenseCategory.UnitName.Should().Be("mile");
	}

	[Fact]
	public async Task CreateAsync_RequiredOnly_SendsRequiredFields()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Expenses.CreateAsync(new ExpenseCreationDto
		{
			ProjectId = 14308069,
			ExpenseCategoryId = 4195926,
			SpentDate = new DateOnly(2017, 3, 1)
		}, ct), MileageJson);

		call.ShouldBe(HttpMethod.Post, "/v2/expenses",
			body: """{"project_id":14308069,"expense_category_id":4195926,"spent_date":"2017-03-01"}""");
	}

	[Fact]
	public async Task CreateAsync_AllFields_SendsEveryField()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Expenses.CreateAsync(new ExpenseCreationDto
		{
			UserId = 1782959,
			ProjectId = 14308069,
			ExpenseCategoryId = 4195926,
			SpentDate = new DateOnly(2017, 3, 1),
			Units = 2m,
			TotalCost = 13.59m,
			Notes = "Lunch",
			Billable = false
		}, ct), MileageJson);

		call.ShouldBe(HttpMethod.Post, "/v2/expenses", body: """
			{"user_id":1782959,"project_id":14308069,"expense_category_id":4195926,"spent_date":"2017-03-01","units":2,"total_cost":13.59,"notes":"Lunch","billable":false}
			""");
	}

	[Fact]
	public async Task CreateAsync_WithReceipt_RequiredOnly_SendsMultipartForm()
	{
		var stub = new StubHandler().Enqueue(HttpStatusCode.Created, LunchJson);
		using var client = TestClient.Create(stub);

		var expense = await client.Expenses.CreateAsync(new ExpenseCreationDto
		{
			ProjectId = 14308069,
			ExpenseCategoryId = 4195926,
			SpentDate = new DateOnly(2017, 3, 1)
		}, Receipt(), TestClient.Ct);

		var call = stub.Calls.Should().ContainSingle().Subject;
		call.Method.Should().Be(HttpMethod.Post);
		call.Uri.AbsolutePath.Should().Be("/v2/expenses");
		call.Uri.Query.Should().BeEmpty();
		call.ContentType.Should().Be("multipart/form-data");
		call.Body.Should().Be(Form(("project_id", "14308069"), ("expense_category_id", "4195926"), ("spent_date", "2017-03-01")));
		ShouldBeTheLunch(expense);
	}

	[Fact]
	public async Task CreateAsync_WithReceipt_AllFields_SendsEveryFieldAsFormText()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Expenses.CreateAsync(new ExpenseCreationDto
		{
			UserId = 1782959,
			ProjectId = 14308069,
			ExpenseCategoryId = 4195930,
			SpentDate = new DateOnly(2017, 3, 1),
			Units = 10.5m,
			TotalCost = 5.62m,
			Notes = "Client visit",
			Billable = true
		}, Receipt(), ct), MileageJson);

		call.Method.Should().Be(HttpMethod.Post);
		call.Uri.AbsolutePath.Should().Be("/v2/expenses");
		call.ContentType.Should().Be("multipart/form-data");
		call.Body.Should().Be(Form(
			("user_id", "1782959"),
			("project_id", "14308069"),
			("expense_category_id", "4195930"),
			("spent_date", "2017-03-01"),
			("units", "10.5"),
			("total_cost", "5.62"),
			("notes", "Client visit"),
			("billable", "true")));
	}

	[Fact]
	public async Task CreateAsync_WithReceipt_NullArguments_Throw()
	{
		using var client = TestClient.Create(new StubHandler());
		var dto = new ExpenseCreationDto { ProjectId = 1, ExpenseCategoryId = 2, SpentDate = new DateOnly(2017, 3, 1) };

		await FluentActions.Awaiting(() => client.Expenses.CreateAsync(null!, Receipt(), TestClient.Ct))
			.Should().ThrowAsync<ArgumentNullException>().WithParameterName("creationDto");
		await FluentActions.Awaiting(() => client.Expenses.CreateAsync(dto, (StreamPart)null!, TestClient.Ct))
			.Should().ThrowAsync<ArgumentNullException>().WithParameterName("receipt");
	}

	[Fact]
	public async Task UpdateAsync_AllFields_SendsPatch()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Expenses.UpdateAsync(15297032, new ExpensePatchDto
		{
			ProjectId = 14308069,
			ExpenseCategoryId = 4195926,
			SpentDate = new DateOnly(2017, 3, 2),
			Units = 3m,
			TotalCost = 20.5m,
			Notes = "Dinner",
			Billable = true,
			DeleteReceipt = true
		}, ct), MileageJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/expenses/15297032", body: """
			{"project_id":14308069,"expense_category_id":4195926,"spent_date":"2017-03-02","units":3,"total_cost":20.5,"notes":"Dinner","billable":true,"delete_receipt":true}
			""");
	}

	[Fact]
	public async Task UpdateAsync_EmptyPatch_SendsEmptyObject()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Expenses.UpdateAsync(15297032, new ExpensePatchDto(), ct), MileageJson);

		call.ShouldBe(HttpMethod.Patch, "/v2/expenses/15297032", body: "{}");
	}

	[Fact]
	public async Task UpdateAsync_WithReceipt_NoFields_SendsOnlyTheReceipt()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Expenses.UpdateAsync(15297032, new ExpensePatchDto(), Receipt(), ct), LunchJson);

		call.Method.Should().Be(HttpMethod.Patch);
		call.Uri.AbsolutePath.Should().Be("/v2/expenses/15297032");
		call.Uri.Query.Should().BeEmpty();
		call.ContentType.Should().Be("multipart/form-data");
		call.Body.Should().Be(Form());
	}

	[Fact]
	public async Task UpdateAsync_WithReceipt_AllFields_SendsEveryFieldAsFormText()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Expenses.UpdateAsync(15297032, new ExpensePatchDto
		{
			ProjectId = 14308069,
			ExpenseCategoryId = 4195926,
			SpentDate = new DateOnly(2017, 3, 2),
			Units = 1m,
			TotalCost = 13.59m,
			Notes = "Dinner",
			Billable = false,
			DeleteReceipt = false
		}, Receipt(), ct), LunchJson);

		call.Method.Should().Be(HttpMethod.Patch);
		call.Uri.AbsolutePath.Should().Be("/v2/expenses/15297032");
		call.ContentType.Should().Be("multipart/form-data");
		call.Body.Should().Be(Form(
			("project_id", "14308069"),
			("expense_category_id", "4195926"),
			("spent_date", "2017-03-02"),
			("units", "1"),
			("total_cost", "13.59"),
			("notes", "Dinner"),
			("billable", "false"),
			("delete_receipt", "false")));
	}

	[Fact]
	public async Task UpdateAsync_WithReceipt_NullArguments_Throw()
	{
		using var client = TestClient.Create(new StubHandler());

		await FluentActions.Awaiting(() => client.Expenses.UpdateAsync(1, null!, Receipt(), TestClient.Ct))
			.Should().ThrowAsync<ArgumentNullException>().WithParameterName("patchDto");
		await FluentActions.Awaiting(() => client.Expenses.UpdateAsync(1, new ExpensePatchDto(), (StreamPart)null!, TestClient.Ct))
			.Should().ThrowAsync<ArgumentNullException>().WithParameterName("receipt");
	}

	[Fact]
	public async Task DeleteAsync_SendsDelete()
	{
		var call = await TestClient.SendAsync((c, ct) => c.Expenses.DeleteAsync(15297032, ct), "");

		call.ShouldBe(HttpMethod.Delete, "/v2/expenses/15297032");
	}

	private static StreamPart Receipt() => new(new MemoryStream(ReceiptBytes), "lunch_receipt.gif", "image/gif");

	/// <summary>The exact multipart body Refit sends: each text field in order, then the receipt file.</summary>
	private static string Form(params (string Name, string Value)[] fields)
	{
		var body = new StringBuilder();
		foreach (var (name, value) in fields)
		{
			body.Append(CultureInfo.InvariantCulture, $"--{Boundary}\r\n")
				.Append("Content-Type: text/plain; charset=utf-8\r\n")
				.Append(CultureInfo.InvariantCulture, $"Content-Disposition: form-data; name={name}\r\n\r\n")
				.Append(CultureInfo.InvariantCulture, $"{value}\r\n");
		}

		return body
			.Append(CultureInfo.InvariantCulture, $"--{Boundary}\r\n")
			.Append("Content-Type: image/gif\r\n")
			.Append("Content-Disposition: form-data; name=receipt; filename=lunch_receipt.gif; filename*=utf-8''lunch_receipt.gif\r\n\r\n")
			.Append("GIF89a\r\n")
			.Append(CultureInfo.InvariantCulture, $"--{Boundary}--\r\n")
			.ToString();
	}

	private static void ShouldBeTheLunch(Expense expense)
	{
		expense.Id.Should().Be(15296442);
		expense.Notes.Should().Be("Lunch with client");
		expense.TotalCost.Should().Be(33.35m);
		expense.Units.Should().Be(1m);
		expense.IsClosed.Should().BeFalse();
		expense.ApprovalStatus.Should().Be(ApprovalStatus.Unsubmitted);
		expense.IsLocked.Should().BeTrue();
		expense.IsBilled.Should().BeTrue();
		expense.LockedReason.Should().Be("Expense is invoiced.");
		expense.SpentDate.Should().Be(new DateOnly(2017, 3, 3));
		expense.CreatedAt.Should().Be(new DateTime(2017, 6, 27, 15, 9, 54, DateTimeKind.Utc));
		expense.UpdatedAt.Should().Be(new DateTime(2017, 6, 27, 16, 47, 14, DateTimeKind.Utc));
		expense.Billable.Should().BeTrue();
		expense.Receipt!.Url.Should().Be("https://{ACCOUNT_SUBDOMAIN}.harvestapp.com/expenses/15296442/receipt");
		expense.Receipt.FileName.Should().Be("lunch_receipt.gif");
		expense.Receipt.FileSize.Should().Be(39410);
		expense.Receipt.ContentType.Should().Be("image/gif");
		expense.User!.Id.Should().Be(1782959);
		expense.User.Name.Should().Be("Kim Allen");
		expense.UserAssignment!.Id.Should().Be(125068553);
		expense.UserAssignment.IsProjectManager.Should().BeTrue();
		expense.UserAssignment.IsActive.Should().BeTrue();
		expense.UserAssignment.Budget.Should().BeNull();
		expense.UserAssignment.HourlyRate.Should().Be(100m);
		expense.UserAssignment.CreatedAt.Should().Be(new DateTime(2017, 6, 26, 22, 32, 52, DateTimeKind.Utc));
		expense.UserAssignment.UpdatedAt.Should().Be(new DateTime(2017, 6, 26, 22, 32, 52, DateTimeKind.Utc));
		expense.Project!.Id.Should().Be(14307913);
		expense.Project.Name.Should().Be("Marketing Website");
		expense.Project.Code.Should().Be("MW");
		expense.ExpenseCategory!.Id.Should().Be(4195926);
		expense.ExpenseCategory.Name.Should().Be("Meals");
		expense.ExpenseCategory.UnitPrice.Should().BeNull();
		expense.ExpenseCategory.UnitName.Should().BeNull();
		expense.Client!.Id.Should().Be(5735774);
		expense.Client.Name.Should().Be("ABC Corp");
		expense.Client.Currency.Should().Be("USD");
		expense.Invoice!.Id.Should().Be(13150403);
		expense.Invoice.Number.Should().Be("1001");
	}
}