namespace Harvest.Test;

public class UserTests(ITestOutputHelper testOutputHelper) : HarvestTest(testOutputHelper)
{
	[Fact]
	public async System.Threading.Tasks.Task GetMe()
	{
		var user = await HarvestClient.Users.GetMeAsync(CancellationToken);
		user.Should().NotBeNull();
		user.Id.Should().BePositive();
		user.FirstName.Should().NotBeNullOrEmpty();
		user.LastName.Should().NotBeNullOrEmpty();
	}

	[Fact]
	public async System.Threading.Tasks.Task ListAllUsers()
	{
		var usersContainer = await HarvestClient.Users.ListAllAsync(
			page: 1,
			perPage: 10,
			cancellationToken: CancellationToken
		);

		usersContainer.Should().NotBeNull();
		usersContainer.Users.Should().NotBeNull();
	}

	[Fact]
	public async System.Threading.Tasks.Task GetUser()
	{
		// Get current user first
		var currentUser = await HarvestClient.Users.GetMeAsync(CancellationToken);
		currentUser.Should().NotBeNull();

		// Get the same user by ID
		var user = await HarvestClient.Users.GetAsync(currentUser.Id, CancellationToken);

		user.Should().NotBeNull();
		user.Id.Should().Be(currentUser.Id);
		user.FirstName.Should().Be(currentUser.FirstName);
		user.LastName.Should().Be(currentUser.LastName);
	}

	[Fact]
	public async System.Threading.Tasks.Task UpdateCurrentUser()
	{
		// Get current user
		var currentUser = await HarvestClient.Users.GetMeAsync(CancellationToken);
		currentUser.Should().NotBeNull();

		var originalTimezone = currentUser.Timezone;

		try
		{
			// Update the user's timezone (safe update that won't break anything)
			var updateDto = new UserPatchDto
			{
				Timezone = "Eastern Time (US & Canada)"
			};

			var updatedUser = await HarvestClient.Users.UpdateAsync(currentUser.Id, updateDto, CancellationToken);
			updatedUser.Should().NotBeNull();
			updatedUser.Id.Should().Be(currentUser.Id);
			updatedUser.Timezone.Should().Be(updateDto.Timezone);
		}
		finally
		{
			// Restore original timezone
			if (originalTimezone != null)
			{
				var restoreDto = new UserPatchDto
				{
					Timezone = originalTimezone
				};
				await HarvestClient.Users.UpdateAsync(currentUser.Id, restoreDto, CancellationToken);
			}
		}
	}
}
