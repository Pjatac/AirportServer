using Airport.DL;
using Airport.Models;
using CommonLibruary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airport.Infra
{
	public interface IDispatcherService
	{
		void CheckForStart(object param);
		void AddNewPlane(Flight flight);
		List<LineDB> GetState();
		List<Flight> GetSchedule();
	}
}
