namespace Harvest.Test;

public class Configuration
{
	/// <summary>
	/// The account Id
	/// </summary>
	public int AccountId { get; set; }

	/// <summary>
	/// The account key
	/// </summary>
	public string AccessToken { get; set; } = string.Empty;

	/// <summary>
	/// Test configuration settings
	/// </summary>
	public TestSettings TestSettings { get; set; } = new();
}

public class TestSettings
{
	/// <summary>
	/// Prefix for all test data to make it easily identifiable
	/// </summary>
	public string TestDataPrefix { get; set; } = "TEST_";

	/// <summary>
	/// Whether to create test data when needed
	/// </summary>
	public bool CreateTestData { get; set; } = true;

	/// <summary>
	/// Whether to cleanup test data after tests
	/// </summary>
	public bool CleanupTestData { get; set; } = true;
}