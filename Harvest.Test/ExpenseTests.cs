namespace Harvest.Test;

public class ExpenseTests(ITestOutputHelper testOutputHelper) : HarvestTest(testOutputHelper)
{
	/// <summary>A 1x1 transparent GIF, the smallest valid receipt.</summary>
	private static readonly byte[] _receiptBytes = Convert.FromBase64String("R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7");

	[Fact]
	public async System.Threading.Tasks.Task ListExpenses()
	{
		var container = await HarvestClient.Expenses.ListAsync(new() { Page = 1, PerPage = 10 }, CancellationToken);

		container.Should().NotBeNull();
		container.Expenses.Should().NotBeNull();
	}

	[Fact]
	public async System.Threading.Tasks.Task ListExpenseCategories()
	{
		var container = await HarvestClient.ExpenseCategories.ListAsync(new() { Page = 1, PerPage = 10 }, CancellationToken);

		container.ExpenseCategories.Should().NotBeEmpty();
		container.ExpenseCategories.Should().OnlyContain(category => category.Id > 0 && !string.IsNullOrEmpty(category.Name));
	}

	[Fact]
	public System.Threading.Tasks.Task CreateReadUpdateDeleteExpenseCategory()
		=> RunStrictScenarioAsync(async () =>
		{
			var name = TestName("CRUD", "Expense Category");
			var created = await HarvestClient.ExpenseCategories.CreateAsync(new ExpenseCategoryCreationDto
			{
				Name = name,
				UnitName = "mile",
				UnitPrice = 0.5m,
				IsActive = true
			}, CancellationToken);
			Track("expense category", created.Id, created.Name, id => HarvestClient.ExpenseCategories.DeleteAsync(id, CancellationToken));
			created.Name.Should().Be(name);
			created.UnitName.Should().Be("mile");
			created.UnitPrice.Should().Be(0.5m);

			var retrieved = await HarvestClient.ExpenseCategories.GetAsync(created.Id, CancellationToken);
			retrieved.Id.Should().Be(created.Id);

			var updated = await HarvestClient.ExpenseCategories.UpdateAsync(created.Id, new ExpenseCategoryPatchDto
			{
				Name = name + " Updated",
				UnitPrice = 0.75m
			}, CancellationToken);
			updated.Name.Should().Be(name + " Updated");
			updated.UnitPrice.Should().Be(0.75m);

			await DeleteTrackedAsync("expense category", created.Id);
		});

	[Fact]
	public System.Threading.Tasks.Task CreateReadUpdateDeleteExpense()
		=> RunStrictScenarioAsync(async () =>
		{
			var (project, category) = await CreateExpenseFixtureAsync("CRUD");
			var spentDate = DateOnly.FromDateTime(DateTime.UtcNow);

			var created = await HarvestClient.Expenses.CreateAsync(new ExpenseCreationDto
			{
				ProjectId = project.Id,
				ExpenseCategoryId = category.Id,
				SpentDate = spentDate,
				TotalCost = 12.34m,
				Notes = $"{Prefix}CRUD Test Expense",
				Billable = true
			}, CancellationToken);
			Track("expense", created.Id, created.Notes, id => HarvestClient.Expenses.DeleteAsync(id, CancellationToken));
			created.TotalCost.Should().Be(12.34m);
			created.SpentDate.Should().Be(spentDate);
			created.Billable.Should().BeTrue();
			created.Project!.Id.Should().Be(project.Id);
			created.ExpenseCategory!.Id.Should().Be(category.Id);
			created.Receipt.Should().BeNull();

			var retrieved = await HarvestClient.Expenses.GetAsync(created.Id, CancellationToken);
			retrieved.Id.Should().Be(created.Id);

			var listed = await HarvestClient.Expenses.ListAsync(new() { ProjectId = project.Id }, CancellationToken);
			listed.Expenses.Should().Contain(expense => expense.Id == created.Id);

			var updated = await HarvestClient.Expenses.UpdateAsync(created.Id, new ExpensePatchDto
			{
				TotalCost = 20m,
				Notes = $"{Prefix}Updated CRUD Test Expense",
				Billable = false
			}, CancellationToken);
			updated.TotalCost.Should().Be(20m);
			updated.Billable.Should().BeFalse();
			updated.Notes.Should().Be($"{Prefix}Updated CRUD Test Expense");

			await DeleteTrackedAsync("expense", created.Id);
		});

	[Fact]
	public System.Threading.Tasks.Task ExpenseWithReceiptUpload()
		=> RunStrictScenarioAsync(async () =>
		{
			var (project, category) = await CreateExpenseFixtureAsync("Receipt");

			using var receiptStream = new MemoryStream(_receiptBytes);
			var created = await HarvestClient.Expenses.CreateAsync(new ExpenseCreationDto
			{
				ProjectId = project.Id,
				ExpenseCategoryId = category.Id,
				SpentDate = DateOnly.FromDateTime(DateTime.UtcNow),
				TotalCost = 5m,
				Notes = $"{Prefix}Receipt Test Expense"
			}, new StreamPart(receiptStream, "receipt.gif", "image/gif"), CancellationToken);
			Track("expense", created.Id, created.Notes, id => HarvestClient.Expenses.DeleteAsync(id, CancellationToken));
			created.Receipt.Should().NotBeNull();
			created.Receipt!.FileName.Should().Be("receipt.gif");
			created.Receipt.ContentType.Should().Be("image/gif");
			created.Receipt.FileSize.Should().Be(_receiptBytes.Length);

			// Replace it through the multipart update, then remove it with delete_receipt.
			using var secondReceiptStream = new MemoryStream(_receiptBytes);
			var replaced = await HarvestClient.Expenses.UpdateAsync(created.Id, new ExpensePatchDto { Notes = $"{Prefix}Receipt Test Expense 2" },
				new StreamPart(secondReceiptStream, "receipt2.gif", "image/gif"), CancellationToken);
			replaced.Receipt!.FileName.Should().Be("receipt2.gif");
			replaced.Notes.Should().Be($"{Prefix}Receipt Test Expense 2");

			var removed = await HarvestClient.Expenses.UpdateAsync(created.Id, new ExpensePatchDto { DeleteReceipt = true }, CancellationToken);
			removed.Receipt.Should().BeNull();

			await DeleteTrackedAsync("expense", created.Id);
		});

	/// <summary>A prefixed client, project (with the current user assigned) and expense category, all tracked.</summary>
	private async Task<(Project Project, ExpenseCategory Category)> CreateExpenseFixtureAsync(string purpose)
	{
		var client = await CreateTestClientAsync(purpose, address: null);
		var project = await CreateTestProjectAsync(client.Id, purpose, notes: null);
		var category = await HarvestClient.ExpenseCategories.CreateAsync(new ExpenseCategoryCreationDto { Name = TestName(purpose, "Expense Category") }, CancellationToken);
		Track("expense category", category.Id, category.Name, id => HarvestClient.ExpenseCategories.DeleteAsync(id, CancellationToken));
		return (project, category);
	}
}