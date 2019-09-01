using Airport.DL;
using Airport.Infra;
using CommonLibruary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Airport.Services
{
	public class DispatcherService: IDispatcherService
	{
		public static ILinesService _linesService;
		public readonly IRepo _repo;
		public static SortedDictionary<DateTime, Flight> WaitingArrivals { get; set; }
		public static SortedDictionary<DateTime, Flight> WaitingDepartures { get; set; }
		private static Timer Handling { get; set; }
		private static int switcher = 1;
		public DispatcherService(ILinesService linesService, IRepo repo)
		{
			_repo = repo;
			_linesService = linesService;
			WaitingArrivals = new SortedDictionary<DateTime, Flight>();
			WaitingDepartures = new SortedDictionary<DateTime, Flight>();
			TimerCallback timeCB = new TimerCallback(CheckForStart);
			Handling = new Timer(timeCB, null, 0, Settings.CheckSchedulePeriod);
		}
		public void CheckForStart(object param)
		{
			KeyValuePair<DateTime, Flight> arrival;
			KeyValuePair<DateTime, Flight> departure;

			lock (WaitingArrivals)
			{
				if (WaitingArrivals.Count > 0)
				{
					arrival = WaitingArrivals.First();
					//Check schedule time for arrival
					if (DateTime.Compare(arrival.Key, DateTime.Now) <= 0)
					{
						if (!_linesService.CheckArrival())
						{
							_linesService.AddArrival(arrival.Value);
							WaitingArrivals.Remove(arrival.Key);
						}
					}
				}
					
			}

			lock (WaitingDepartures)
			{
				if (WaitingDepartures.Count > 0)
				{
					departure = WaitingDepartures.First();
					//Check schedule time for departure
					if (DateTime.Compare(departure.Key, DateTime.Now) <= 0)
					{
						//We can take departure only if both of 6 & 7 lines is empty - in another case we can catch bung
						//Simple switcher use only for beauty
						if (!_linesService.CheckDepart1() && !_linesService.CheckDepart2() && switcher % 2 == 0)
						{
							_linesService.AddDepartureOn1(departure.Value);
							WaitingDepartures.Remove(departure.Key);
							switcher++;
						}
						else if (!_linesService.CheckDepart1() && !_linesService.CheckDepart2())
						{
							_linesService.AddDepartureOn2(departure.Value);
							WaitingDepartures.Remove(departure.Key);
							switcher++;
						}
					}
				}
			}
		}
		public void AddNewPlane(Flight flight)
		{
			if (flight.Direction == 1)
			{
				lock (WaitingArrivals)
				{
					WaitingArrivals.Add(flight.QueryDate, flight);
					_repo.AddArival(new Arrival { Number = flight.Number, Time = flight.QueryDate });
				}
			}
			else
			{
				lock (WaitingDepartures)
				{
					WaitingDepartures.Add(flight.QueryDate, flight);
					_repo.AddDeparture(new Departure { Number = flight.Number, Time = flight.QueryDate });
				}
			}
		}
		public List<LineDB> GetState()
		{
			return _linesService.GetState();
		}
		public List<Flight> GetSchedule()
		{
			List<Flight> schedule = new List<Flight>();
			foreach (var f in WaitingArrivals)
			{
				schedule.Add( new Flight{time=f.Key.ToShortTimeString(), Number = f.Value.Number, Direction=f.Value.Direction});
			}
			foreach (var f in WaitingDepartures)
			{
				schedule.Add(new Flight { time = f.Key.ToShortTimeString(), Number = f.Value.Number, Direction = f.Value.Direction });
			}
			return schedule;
		}
	}
}
