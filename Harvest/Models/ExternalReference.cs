namespace Harvest.Models;

	public class ExternalReference
	{
		public long Id { get; set; }

		public long GroupId { get; set; }

		public long Service { get; set; }

		public long Permalink { get; set; }

		public long ServiceIconUrl { get; set; }
	}