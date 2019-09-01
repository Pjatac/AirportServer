using System.ComponentModel.DataAnnotations;

namespace Airport.DL
{
	public class LineDB
	{
		[Key]
		public int LineId { get; set; }
		public LineSnapshot LineSnapshot { get; set; }
		public int LineSnapshotId { get; set; }
		public bool IsBusy { get; set; }
		public int Number { get; set; }
		public string Direction { get; set; }
	}
}
