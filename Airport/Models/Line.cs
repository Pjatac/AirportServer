using Airport.BL;
using Airport.DL;
using Airport.Hubs;
using Airport.Infra;
using CommonLibruary;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
//Use this library if you need log
//using NLog;

namespace Airport.Models
{
	public class Line
	{
		private readonly IHubContext<AirportHub> _airportHub;
		private readonly IRepo _repo;
		public int Number { get; set; }
		private Timer Handling { get; set; }
		public bool Busy { get; set; }
		public Queue<FlightPosition> FlightsQueue { get; set; }
		public List<Line> NextForArrival { get; set; }
		public List<Line> NextForDeparture { get; set; }
		public int Downtime { get; set; }
		public Line Previous { get; set; }
		public Flight Current { get; set; }
		public FlightMove CurrentMove { get; set; }
		//Use if you need log
		//private Logger logger;
		//public Line(int number, Logger logger, IHubContext<AirportHub> airportHub, IRepo repo)
		public Line(int number, IHubContext<AirportHub> airportHub, IRepo repo)
		{
			_repo = repo;
			_airportHub = airportHub;
			FlightsQueue = new Queue<FlightPosition>();
			Number = number;
			//this.logger = logger;
		}
		public void SetNextArrivalLines(List<Line> nextForArrival)
		{
			NextForArrival = nextForArrival;
		}
		public void SetNextDepartureLines(List<Line> nextForDeparture)
		{
			NextForDeparture = nextForDeparture;
		}
		public async void TakeFlight(FlightPosition flight)
		{
			//Sending information to client, that the current line take new craft
			await _airportHub.Clients.All.SendAsync("transferdata", new { Number, State = true, Craft = flight.Craft.Number, flight.Craft.Direction });
			lock (this)
			{
				//var data = $"Line {Number} start taking new flight {flight.Craft.Number} with direction {flight.Craft.Direction}";
				//logger.Info(data);
				//Taking craft from previous line in case, that it was on waiting mode
				if (flight.PreviousLine != null)
				{
					//logger.Info($"Start moving flight {flight.Craft.Number} from previos line {flight.PreviousLine.Number}");
					flight.PreviousLine.Move();
				}
				if (Current == null)
				{
					//logger.Info($"Line {Number} start place new flight {flight.Craft.Number} with direction {flight.Craft.Direction} and set servicing time");
					Busy = true;
					Current = flight.Craft;
					Downtime = Helper.GetRandom(Settings.MinServiceTime, Settings.MaxServiceTime);
					TimerCallback timeCB = new TimerCallback(StartLetOutFlight);
					Handling = new Timer(timeCB, null, Downtime, -1);
					//logger.Info($"Line {Number} set {flight.Craft.Number} for service time {Downtime}");
					CurrentMove = new FlightMove { Number = Current.Number, LineNumber = Number, Start = DateTime.Now };
					if (flight.OldPosition == -1)
						//Add snapshot for new arrival or departure
						_repo.AddLinesSnapshot(new LineDB { IsBusy = true, Number = Number, Direction = (Current.Direction == 1 ? "arrive" : "depart") }, -1);
					else
						//Add snapshot for standart move
						_repo.AddLinesSnapshot(new LineDB { IsBusy = true, Number = Number, Direction = (Current.Direction == 1 ? "arrive" : "depart") }, flight.OldPosition);
				}		
			}
		}
		public void CheckQueue()
		{
			lock (FlightsQueue)
			{
				if (FlightsQueue.Count > 0)
				{
					//logger.Info($"Line {Number} start dequeue");
					TakeFlight(FlightsQueue.Dequeue());
				}
				else
				{
					//logger.Info($"Line {Number} queue is empty");
				}
			}
		}
		public void Move()
		{
			lock (this)
			{
				CurrentMove.Finish = DateTime.Now;
				_repo.AddMove(CurrentMove);
				//Sending information to client in case of craft is lefting airport and add snapshot
				if ((Number == 4 && Current.Direction == 2) || ((Number == 6 || Number == 7) && Current.Direction == 1))
				{
					_repo.AddLinesSnapshot(new LineDB { IsBusy = false, Number = Number, Direction = String.Empty }, -1);
					_airportHub.Clients.All.SendAsync("transferdata", new { Number, State = false });
				}
				//var data = $"Line {Number} start move {Current.Number} from themself";
				//logger.Info(data);
				//logger.Info($"Line {Number} start check queue");
				Current = null;
				Busy = false;
			}		
			CheckQueue();
		}
		public void StartLetOutFlight(object param)
		{
			//Case of arrival
			lock(this)
			{
				//logger.Info($"Line {Number} start letout {Current.Number}");
				if (Current.Direction == 1)
				{
					//Check for parking
					if (Number == 6 || Number == 7)
					{
						//logger.Info($"From line {Number} was parking {Current.Number} at {DateTime.Now.ToShortDateString()}");
						Move();
					}
					else
					{
						bool startWaiting = true;
						Dictionary<Line, int> waitingCounts = new Dictionary<Line, int>();
						//logger.Info($"Line {Number} start checking for next line");
						foreach (var line in NextForArrival)
						{
							if (!line.Busy)
							{
								//logger.Info($"Line {Number} find that {line.Number} is free and send {Current.Number}");
								line.TakeFlight(new FlightPosition { Craft = Current, PreviousLine = null, OldPosition = this.Number });
								Move();
								startWaiting = false;
								break;
							}
							else
							{
								waitingCounts.Add(line, line.FlightsQueue.Count);
								//logger.Info($"Line {Number} add count {line.FlightsQueue.Count} on {line.Number}");
							}
						}
						if (startWaiting)
						{
							if (Number != 5)
							{
								//logger.Info($"Line {Number} add to queue of {waitingCounts.FirstOrDefault(pair => pair.Value == waitingCounts.Values.Min()).Key.Number} at {Current.Number}");
								waitingCounts.FirstOrDefault(pair => pair.Value == waitingCounts.Values.Min()).Key.FlightsQueue.Enqueue(new FlightPosition { Craft = Current, PreviousLine = this });
							}
							else
							{
								if (NextForArrival[0].Current.Direction == 1)
								{
									//logger.Info($"Line {Number} add to queue of {NextForArrival[0].Number} at {Current.Number}");
									NextForArrival[0].FlightsQueue.Enqueue(new FlightPosition { Craft = Current, PreviousLine = this });
								}
								else
								{
									//logger.Info($"Line {Number} add to queue of {NextForArrival[1].Number} at {Current.Number}");
									NextForArrival[1].FlightsQueue.Enqueue(new FlightPosition { Craft = Current, PreviousLine = this });
								}
							}
						}
					}
				}
				//Case of departure
				else
				{
					//Check for take wind
					if (Number == 4)
					{
						//logger.Info($"From line {Number} was take wing {Current.Number} at {DateTime.Now.ToShortDateString()}");
						Move();
					}
					else
					{
						bool startWaiting = true;
						Dictionary<Line, int> counts = new Dictionary<Line, int>();
						//logger.Info($"Line {Number} start checking for next line");
						foreach (var line in NextForDeparture)
						{
							if (!line.Busy)
							{
								//logger.Info($"Line {Number} find that {line.Number} is free and send {Current.Number}");
								line.TakeFlight(new FlightPosition { Craft = Current, PreviousLine = null, OldPosition = this.Number });
								Move();
								startWaiting = false;
							}
							else
							{
								//logger.Info($"Line {Number} add count {line.FlightsQueue.Count} on {line.Number}");
								counts.Add(line, line.FlightsQueue.Count);
							}
						}
						if (startWaiting)
						{
							//logger.Info($"Line {Number} add to queue of {counts.FirstOrDefault(pair => pair.Value == counts.Values.Min()).Key.Number} at {Current.Number}");
							counts.FirstOrDefault(pair => pair.Value == counts.Values.Min()).Key.FlightsQueue.Enqueue(new FlightPosition { Craft = Current, PreviousLine = this });
						}
					}
				}
			}
		}
	}
}
