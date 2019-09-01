using Airport.DL;
using Airport.Hubs;
using Airport.Infra;
using Airport.Models;
using CommonLibruary;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
//Use this libraries if you need log
//using System.IO;
//using NLog;

namespace Airport.Services
{
	public class LinesService: ILinesService
	{
		private readonly IHubContext<AirportHub> _airportHub;
		private readonly IRepo _repo;
		public static List<Line> Lines { get; set; }
		//Use this counter and loger, if you need log
		//private static Logger logger;
		//private static int ArrCount;
		//private static int DepCount;
		public LinesService(IHubContext<AirportHub> airportHub, IRepo repo)
		{
			_repo = repo;
			_airportHub = airportHub;
			//Initialize logger
			//var config = new NLog.Config.LoggingConfiguration();
			//File.Create("file.txt");
			//var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "file.txt" };
			//var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
			//config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
			//config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
			//LogManager.Configuration = config;
			//logger = LogManager.GetCurrentClassLogger();
			//Lines = new List<Line>
			//{
			//	new Line(1, logger, _airportHub, _repo),
			//	new Line(2, logger, _airportHub, _repo),
			//	new Line(3, logger, _airportHub, _repo),
			//	new Line(4, logger, _airportHub, _repo),
			//	new Line(5, logger, _airportHub, _repo),
			//	new Line(6, logger, _airportHub, _repo),
			//	new Line(7, logger, _airportHub, _repo),
			//	new Line(8, logger, _airportHub, _repo)
			//};
			Lines = new List<Line>
			{
				new Line(1, _airportHub, _repo),
				new Line(2, _airportHub, _repo),
				new Line(3, _airportHub, _repo),
				new Line(4, _airportHub, _repo),
				new Line(5, _airportHub, _repo),
				new Line(6, _airportHub, _repo),
				new Line(7, _airportHub, _repo),
				new Line(8, _airportHub, _repo)
			};
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
				Lines[0].TakeFlight(new FlightPosition { Craft = flight, PreviousLine = null });
				//ArrCount++;
				//logger.Info($" ArrCount = {ArrCount}");
			}
		}
		public void AddDepartureOn1(Flight flight)
		{
			lock (Lines[5])
			{
				Lines[5].TakeFlight(new FlightPosition { Craft = flight, PreviousLine = null });
				//DepCount++;
				//logger.Info($" DepCount (6)= {DepCount}");
			}
		}
		public void AddDepartureOn2(Flight flight)
		{
			lock (Lines[6])
			{
				Lines[6].TakeFlight(new FlightPosition { Craft = flight, PreviousLine = null });
				//DepCount++;
				//logger.Info($" DepCount (7)= {DepCount}");
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
					lineStates.Add(new LineDB { IsBusy = true, Number = l.Number, Direction = (l.Current.Direction == 1 ? "arrive" : "depart")});
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
