using Microsoft.Extensions.Logging.Abstractions;

namespace Harvest.UnitTest.Core;

public class HarvestClientOptionsTests
{
	[Fact]
	public void Defaults_AreDocumentedValues()
	{
		var options = new HarvestClientOptions { AccountId = 1, AccessToken = "t" };

		options.UserAgent.Should().BeNull();
		options.ProxyUrl.Should().BeNull();
		options.MaxRetries.Should().Be(HarvestClientOptions.DefaultMaxRetries).And.Be(3);
		options.RetryBaseDelay.Should().Be(TimeSpan.FromSeconds(1));
		options.MaxRetryDelay.Should().Be(TimeSpan.FromSeconds(60));
		options.Timeout.Should().Be(TimeSpan.FromMinutes(5));
		options.Logger.Should().BeSameAs(NullLogger.Instance);
		options.Invoking(o => o.Validate()).Should().NotThrow();
	}

	private static HarvestClientOptions Invalid(string setting) => setting switch
	{
		"AccessToken" => new() { AccountId = 1, AccessToken = " " },
		"AccountId" => new() { AccountId = 0, AccessToken = "t" },
		"UserAgent" => new() { AccountId = 1, AccessToken = "t", UserAgent = "" },
		"MaxRetries" => new() { AccountId = 1, AccessToken = "t", MaxRetries = -1 },
		"RetryBaseDelay" => new() { AccountId = 1, AccessToken = "t", RetryBaseDelay = TimeSpan.FromSeconds(-1) },
		"MaxRetryDelay" => new() { AccountId = 1, AccessToken = "t", MaxRetryDelay = TimeSpan.FromSeconds(-1) },
		_ => new() { AccountId = 1, AccessToken = "t", Timeout = TimeSpan.Zero }
	};

	[Theory]
	[InlineData("AccessToken", "AccessToken")]
	[InlineData("AccountId", "AccountId")]
	[InlineData("UserAgent", "UserAgent")]
	[InlineData("MaxRetries", "MaxRetries")]
	[InlineData("RetryBaseDelay", "RetryBaseDelay")]
	[InlineData("MaxRetryDelay", "RetryBaseDelay")]
	[InlineData("Timeout", "Timeout")]
	public void Validate_InvalidSetting_ThrowsNamingIt(string setting, string parameterName)
		=> Invalid(setting).Invoking(o => o.Validate()).Should().Throw<ArgumentException>().WithParameterName(parameterName);

	[Fact]
	public void Validate_InfiniteTimeoutAndZeroRetries_AreAllowed()
	{
		var options = new HarvestClientOptions
		{
			AccountId = 1,
			AccessToken = "t",
			UserAgent = "MyApp (me@example.com)",
			MaxRetries = 0,
			RetryBaseDelay = TimeSpan.Zero,
			MaxRetryDelay = TimeSpan.Zero,
			Timeout = Timeout.InfiniteTimeSpan
		};

		options.Invoking(o => o.Validate()).Should().NotThrow();
	}
}