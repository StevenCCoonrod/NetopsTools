using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObjects
{
	/// <summary>
	/// CREATOR: Steve C
	/// Created: 2022/04/20
	/// Data Object class for an Mtr Report.
	/// </summary>
	public class MtrReport
    {
		public int MtrReportID { get; set; }
		public string? SyncboxID { get; set; }
		public DateTime UTCStartTime { get; set; }
		public List<MtrHop> Hops { get; set; } = new List<MtrHop>();
		
		
		override
		public  string ToString()
		{
			var sb = new StringBuilder();

			sb.AppendLine("Mtr Database ID: " + MtrReportID);
			sb.AppendLine("Host: " + SyncboxID);
			sb.AppendLine("Start: " + UTCStartTime);
			Hops.ToList().ForEach(l => sb.AppendLine(l.ToString()));

			return sb.ToString();
		}
	}
}
