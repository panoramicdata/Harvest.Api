using Harvest.Test.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Harvest.Test;

public class HarvestTest
{
	protected ILogger Logger { get; }

	protected HarvestClient HarvestClient { get; }

	protected Configuration Configuration { get; }

	protected static CancellationToken CancellationToken => TestContext.Current.CancellationToken;

	protected HarvestTest(ITestOutputHelper iTestOutputHelper)
	{
		Logger = new LoggerFactory()
			//.AddDebug(LogLevel.Trace)
			.AddXunit(iTestOutputHelper, LogLevel.Trace)
			.CreateLogger<HarvestTest>();

		Configuration = LoadConfiguration("appsettings.json");
		HarvestClient = new HarvestClient(Configuration.AccountId, Configuration.AccessToken);
	}

	private static Configuration LoadConfiguration(string jsonFilePath)
	{
		var location = typeof(HarvestTest).GetTypeInfo().Assembly.Location;
		var dirPath = Path.Combine(Path.GetDirectoryName(location), "../../..");

		Configuration configuration;
		var configurationRoot = new ConfigurationBuilder()
			.SetBasePath(dirPath)
			.AddJsonFile(jsonFilePath, false, false)
			.Build();
		var services = new ServiceCollection();
		_ = services
			.AddOptions()
			.Configure<Configuration>(configurationRoot);
		using (var sp = services.BuildServiceProvider())
		{
			var options = sp.GetService<IOptions<Configuration>>();
			configuration = options.Value;
		}

		return configuration;
	}
}