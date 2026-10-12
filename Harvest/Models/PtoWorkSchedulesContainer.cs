namespace Harvest.Models;

/// <summary>A page of <c>GET /v2/pto/work_schedules</c>.</summary>
public class PtoWorkSchedulesContainer : ListContainerBase
{
	/// <summary>The work schedules on this page (<c>pto_work_schedules</c>).</summary>
	public List<PtoWorkSchedule> PtoWorkSchedules { get; set; } = [];
}