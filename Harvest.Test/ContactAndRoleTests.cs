namespace Harvest.Test;

public class ContactAndRoleTests(ITestOutputHelper testOutputHelper) : HarvestTest(testOutputHelper)
{
	[Fact]
	public async System.Threading.Tasks.Task ListContactsAndRoles()
	{
		var contacts = await HarvestClient.Contacts.ListAsync(new() { Page = 1, PerPage = 10 }, CancellationToken);
		contacts.Contacts.Should().NotBeNull();

		var roles = await HarvestClient.Roles.ListAsync(new() { Page = 1, PerPage = 10 }, CancellationToken);
		roles.Roles.Should().NotBeNull();
	}

	[Fact]
	public System.Threading.Tasks.Task CreateReadUpdateDeleteContact()
		=> RunStrictScenarioAsync(async () =>
		{
			var client = await CreateTestClientAsync("Contact", address: null);
			var firstName = TestName("CRUD", "Contact");

			var created = await HarvestClient.Contacts.CreateAsync(new ContactCreationDto
			{
				ClientId = client.Id,
				FirstName = firstName,
				LastName = "Tester",
				Title = "Dr",
				Email = "test-contact@example.invalid",
				PhoneOffice = "01234 567890"
			}, CancellationToken);
			Track("contact", created.Id, created.FirstName, id => HarvestClient.Contacts.DeleteAsync(id, CancellationToken));
			created.FirstName.Should().Be(firstName);
			created.LastName.Should().Be("Tester");
			created.Client!.Id.Should().Be(client.Id);

			var retrieved = await HarvestClient.Contacts.GetAsync(created.Id, CancellationToken);
			retrieved.Id.Should().Be(created.Id);

			var listed = await HarvestClient.Contacts.ListAsync(new() { ClientId = client.Id }, CancellationToken);
			listed.Contacts.Should().Contain(contact => contact.Id == created.Id);

			var updated = await HarvestClient.Contacts.UpdateAsync(created.Id, new ContactPatchDto { LastName = "Updated" }, CancellationToken);
			updated.LastName.Should().Be("Updated");

			await DeleteTrackedAsync("contact", created.Id);
		});

	[Fact]
	public System.Threading.Tasks.Task CreateReadUpdateDeleteRole()
		=> RunStrictScenarioAsync(async () =>
		{
			var name = TestName("CRUD", "Role");
			var created = await HarvestClient.Roles.CreateAsync(new RoleCreationDto { Name = name }, CancellationToken);
			Track("role", created.Id, created.Name, id => HarvestClient.Roles.DeleteAsync(id, CancellationToken));
			created.Name.Should().Be(name);

			var retrieved = await HarvestClient.Roles.GetAsync(created.Id, CancellationToken);
			retrieved.Id.Should().Be(created.Id);

			var me = await HarvestClient.Users.GetMeAsync(CancellationToken);
			var updated = await HarvestClient.Roles.UpdateAsync(created.Id, new RolePatchDto { Name = name + " Updated", UserIds = [me.Id] }, CancellationToken);
			updated.Name.Should().Be(name + " Updated");
			updated.UserIds.Should().Contain(me.Id);

			var cleared = await HarvestClient.Roles.UpdateAsync(created.Id, new RolePatchDto { UserIds = [] }, CancellationToken);
			cleared.UserIds.Should().BeNullOrEmpty();

			await DeleteTrackedAsync("role", created.Id);
		});
}