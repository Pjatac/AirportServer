using Airport.DL;
using Airport.Hubs;
using Airport.Infra;
using Airport.Models;
using CommonLibruary;
using Microsoft.AspNetCore.SignalR;
using NLog;
using System.Collections.Generic;

namespace Airport.Services
{
	public class LinesService: ILinesService
	{
		private readonly IHubContext<AirportHub> _airportHub;
		private readonly IRepo _repo;
		public static List<Line> Lines { get; set; }
		private static Logger logger;
		private static int ArrCount;
		private static int DepCount;
		public LinesService(IHubContext<AirportHub> airportHub, IRepo repo)
		{
			_repo = repo;
			_airportHub = airportHub;
			var config = new NLog.Config.LoggingConfiguration();
			var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "file.txt" };
			var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
			config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
			config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
			LogManager.Configuration = config;
			logger = LogManager.GetCurrentClassLogger();
			Lines = new List<Line>();
			Lines.Add(new Line(1, logger, _airportHub, _repo));
			Lines.Add(new Line(2, logger, _airportHub, _repo));
			Lines.Add(new Line(3, logger, _airportHub, _repo));
			Lines.Add(new Line(4, logger, _airportHub, _repo));
			Lines.Add(new Line(5, logger, _airportHub, _repo));
			Lines.Add(new Line(6, logger, _airportHub, _repo));
			Lines.Add(new Line(7, logger, _airportHub, _repo));
			Lines.Add(new Line(8, logger, _airportHub, _repo));
			//Configuring the ьщмштп logic of lines according to our task
			Lines[0].SetNextArrivalLines(new List<Line> { Lines[1] });
			Lines[1].SetNextArrivalLines(new List<Line> { Lines[2] });
			Lines[2].SetNextArrivalLines(new List<Line> { Lines[3] });
			Lines[3].SetNextArrivalLines(new List<Line> { Lines[4] });
			Lines[4].SetNextArrivalLines(new List<Line> { Lines[5], Lines[6] });
			Lines[5].SetNextDepartureLines(new List<Line> { Lines[7] });
			Lines[6].SetNextDepartureLines(new List<Line> { Lines[7] });
			Lines[7].SetNextDepartureLines(new List<Line> { Lines[3] });
		}
		public void AddArrival(Flight flight)
		{
			lock (Lines[0])
			{
				Lines[0].TakeFlight(new FlightPosition { Craft = flight, Previous = null });
				ArrCount++;
				logger.Info($" ArrCount = {ArrCount}");
			}
		}
		public void AddDepartureOn1(Flight flight)
		{
			lock (Lines[5])
			{
				Lines[5].TakeFlight(new FlightPosition { Craft = flight, Previous = null });
				DepCount++;
				logger.Info($" DepCount (6)= {DepCount}");
			}
		}
		public void AddDepartureOn2(Flight flight)
		{
			lock (Lines[6])
			{
				Lines[6].TakeFlight(new FlightPosition { Craft = flight, Previous = null });
				DepCount++;
				logger.Info($" DepCount (7)= {DepCount}");
			}
		}
		public bool CheckArrival()
		{
			return Lines[0].Busy;
		}
		public bool CheckDepart1()
		{
			return Lines[5].Busy;
		}
		public bool CheckDepart2()
		{
			return Lines[6].Busy;
		}
		public List<LineDB> GetState()
		{
			List<LineDB> lineStates = new List<LineDB>();
			foreach (var l in Lines)
			{
				if (l.Busy)
				{
					lineStates.Add(new LineDB { IsBusy = true, Number = l.Number, Direction = (l.CurrentFlight.Direction == 1 ? "arrive" : "depart")});
				}
				else
				{
					lineStates.Add(new LineDB { IsBusy = false, Number = l.Number, Direction = string.Empty });
				}
			}
			return lineStates;
		}
	}
}
