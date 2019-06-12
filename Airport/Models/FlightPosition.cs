using CommonLibruary;

namespace Airport.Models
{
	public class FlightPosition
	{
		public Flight Craft { get; set; }
		public Line Previous { get; set; }
	}
}
