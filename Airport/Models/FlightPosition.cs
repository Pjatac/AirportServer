using CommonLibruary;

namespace Airport.Models
{
	public class FlightPosition
	{
		public Flight Craft { get; set; }
		public Line PreviousLine { get; set; }

		public int OldPosition { get; set; }
		public FlightPosition()
		{
			OldPosition = -1;
		}
	}
}
