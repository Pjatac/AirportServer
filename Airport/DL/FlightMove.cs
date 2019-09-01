using System;
using System.ComponentModel.DataAnnotations;

namespace Airport.DL
{
	public class FlightMove
	{
		[Key]
		public int FlightMoveId { get; set; }
		public string Number { get; set; }
		public int LineNumber { get; set; }
		public DateTime Start { get; set; }
		public DateTime Finish { get; set; }
	}
}
