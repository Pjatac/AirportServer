using System;
using System.ComponentModel.DataAnnotations;

namespace Airport.DL
{
	public class Arrival
	{
		[Key]
		public int ArrivalId { get; set; }
		public string Number { get; set; }
		public DateTime Time { get; set; }
	}
}
