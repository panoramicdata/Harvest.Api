using Harvest.Dtos;

namespace Harvest.Models;

	public class ExternalReferenceCreationDto : CreationDto
	{
		/// <summary>
		/// The ID of the external reference.
		/// </summary>
		public long Id { get; set; }

		/// <summary>
		/// The ID of the external reference.
		/// </summary>
		public long GroupId { get; set; }

		/// <summary>
		/// The permalink of the external reference.
		/// </summary>
		public long Permalink { get; set; }
	}