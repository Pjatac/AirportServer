using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Airport.DL
{
	public class LineSnapshot
	{
		[Key]
		public int LineSnapshotId { get; set; }
		public DateTime Time { get; set; }
		public List<LineDB> Lines { get; set; }
	}
}
