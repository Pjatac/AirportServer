using Airport.DL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airport.Infra
{
	public interface IRepo
	{
		void AddMove(FlightMove move);
		void AddLinesSnapshot(LineDB changedLine);
		void AddArival(Arrival arrival);
		void AddDeparture(Departure departure);
	}
}
