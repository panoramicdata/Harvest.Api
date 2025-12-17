namespace Harvest.Interfaces;

	public interface IHarvestClient
	{
		IClientApi Clients { get; }

		ICompanyApi Companies { get; }
	}
