using Airport.DL;
using Airport.Models;
using CommonLibruary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airport.Infra
{
	public interface ILinesService
	{
		List<LineDB> GetState();
		bool CheckDepart2();
		bool CheckDepart1();
		bool CheckArrival();
		void AddDepartureOn2(Flight flight);
		void AddDepartureOn1(Flight flight);
		void AddArrival(Flight flight);
	}
}
