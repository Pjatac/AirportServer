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
		void StartArrival();
		void StartDeparture();
		List<LineDB> GetState();
		List<object> GetSchedule();
	}
}
