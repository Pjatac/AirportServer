using System;
using System.ComponentModel.DataAnnotations;

namespace Airport.DL
{
	public class Departure
	{
		[Key]
		public int DepartureId { get; set; }
		public string Number { get; set; }
		public DateTime Time { get; set; }
	}
}
