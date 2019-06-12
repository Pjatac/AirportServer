using Airport.BL;
using Airport.DL;
using Airport.Hubs;
using Airport.Infra;
using CommonLibruary;
using Microsoft.AspNetCore.SignalR;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Airport.Models
{
	public class Line
	{
		private readonly IHubContext<AirportHub> _airportHub;
		private readonly IRepo _repo;
		private Logger logger;
		public int Number { get; set; }
		private Timer Handling { get; set; }
		public bool Busy { get; set; }
		public Queue<FlightPosition> Flights { get; set; }
		public List<Line> NextForArrival { get; set; }
		public List<Line> NextForDeparture { get; set; }
		public int Downtime { get; set; }
		public Line Previous { get; set; }
		public Flight CurrentFlight { get; set; }
		public FlightMove CurrentMove { get; set; }
		public Line(int number, Logger logger, IHubContext<AirportHub> airportHub, IRepo repo)
		{
			_repo = repo;
			_airportHub = airportHub;
			Flights = new Queue<FlightPosition>();
			Number = number;
			this.logger = logger;
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
			await _airportHub.Clients.All.SendAsync("transferdata", new { Number = this.Number, State = true, Craft = flight.Craft.Number, Direction = flight.Craft.Direction });
			lock (this)
			{
				var data = $"Line {Number} start taking new flight {flight.Craft.Number} with direction {flight.Craft.Direction}";
				logger.Info(data);	
				if (flight.Previous != null)
				{
					logger.Info($"Start moving flight {flight.Craft} from previos line {flight.Previous.Number}");
					flight.Previous.ReleaseLine();
				}
				if (CurrentFlight == null)
				{
				
					logger.Info($"Line {Number} start place new flight {flight.Craft.Number} with direction {flight.Craft.Direction} and set servicing time");
					Busy = true;
					CurrentFlight = flight.Craft;
					Downtime = Helper.GetRandom(Settings.MinServiceTime, Settings.MaxServiceTime);
					TimerCallback timeCB = new TimerCallback(StartLetOutFlight);
					Handling = new Timer(timeCB, null, Downtime, -1);
					logger.Info($"Line {Number} set {flight.Craft.Number} for service time {Downtime}");
				}
				CurrentMove = new FlightMove { FlightMoveId = 0, Number = CurrentFlight.Number, LineNumber = Number, Start = DateTime.Now};
				_repo.AddLinesSnapshot(new LineDB { IsBusy = true, Number = Number, Direction = (CurrentFlight.Direction == 1 ? "arrive" : "depart") });
			}
		}
		public string GetState()
		{
			lock (this)
			{
				if (Busy)
					return $"Busy with{CurrentFlight.Number}, which {CurrentFlight.Direction}";
				else
					return $"Free";
			}
		}
		public void CheckQueue()
		{
			lock (this)
			{
				if (Flights.Count > 0)
				{
					logger.Info($"Line {Number} start dequeue");
					TakeFlight(Flights.Dequeue());
				}
				else
				{
					logger.Info($"Line {Number} queue is empty");
				}
			}
		}
		public void ReleaseLine()
		{
			lock (this)
			{
				var data = $"Line {Number} start move {CurrentFlight.Number} from themself";
				_airportHub.Clients.All.SendAsync("transferdata", new { Number = this.Number, State = false });
				logger.Info(data);
				CurrentFlight = null;
				Busy = false;
				logger.Info($"Line {Number} start check queue");
				CurrentMove.Finish = DateTime.Now;
				_repo.AddLinesSnapshot(new LineDB { IsBusy = false, Number = Number, Direction = string.Empty });
				_repo.AddMove(CurrentMove);
				CheckQueue();
			}		
		}
		public void StartLetOutFlight(object param)
		{
			lock (this)
			{
				logger.Info($"Line {Number} start letout {CurrentFlight.Number}");
				if (CurrentFlight.Direction == 1)//Arrival logic
				{
					if (Number == 6 || Number == 7)//Arrival parking
					{
						logger.Info($"From line {Number} was parking {CurrentFlight.Number} at {DateTime.Now.ToShortDateString()}");
						ReleaseLine();
					}
					else
					{
						bool startWaiting = true;
						Dictionary<Line, int> counts = new Dictionary<Line, int>();
						logger.Info($"Line {Number} start checking for next line");
						foreach (var line in NextForArrival)//Checking next line in path
						{
							if (!line.Busy)//Relocation to next line
							{
								logger.Info($"Line {Number} find that {line.Number} is free and send {CurrentFlight.Number}");
								line.TakeFlight(new FlightPosition { Craft = CurrentFlight, Previous = null });
								ReleaseLine();
								startWaiting = false;
								break;
							}
							else//Form list of counts of next lines queues 
							{
								counts.Add(line, line.Flights.Count);
								logger.Info($"Line {Number} add count {line.Flights.Count} on {line.Number}");
							}
						}
						if (startWaiting)//Situation when you need to expect
						{
							if (Number != 5)
							{
								var line = counts.FirstOrDefault(pair => pair.Value == counts.Values.Min()).Key;//Line selection with the shortest queue
								logger.Info($"Line {Number} add to queue of {line.Number} at {CurrentFlight.Number}");
								//Add flight to queue of next line in path
								counts.FirstOrDefault(pair => pair.Value == counts.Values.Min()).Key.Flights.Enqueue(new FlightPosition { Craft = CurrentFlight, Previous = this });
							}
							else//Situation where there may be a traffic jam
							{
								if (!NextForArrival[0].Busy)//For arrival flight we can move it from 5 to 6
								{
									logger.Info($"Line {Number} find that {NextForArrival[0].Number} is free and send {CurrentFlight.Number}");
									NextForArrival[0].TakeFlight(new FlightPosition { Craft = CurrentFlight, Previous = null });
									ReleaseLine();
								}
								else if (!NextForArrival[1].Busy)//For arrival flight we can move it from 5 to 7
								{
									logger.Info($"Line {Number} find that {NextForArrival[1].Number} is free and send {CurrentFlight.Number}");
									NextForArrival[1].TakeFlight(new FlightPosition { Craft = CurrentFlight, Previous = null });
									ReleaseLine();
								}
								else//If both 6 and 7 is busy
								{
									if (NextForArrival[0].CurrentFlight.Direction == 1)//Add to queue of 6, if flight on 6 is arrival
									{
										logger.Info($"Line {Number} add to queue of {NextForArrival[0].Number} at {CurrentFlight.Number}");
										NextForArrival[0].Flights.Enqueue(new FlightPosition { Craft = CurrentFlight, Previous = this });
									}
									else//Add to queue of 7
									{
										logger.Info($"Line {Number} add to queue of {NextForArrival[1].Number} at {CurrentFlight.Number}");
										NextForArrival[1].Flights.Enqueue(new FlightPosition { Craft = CurrentFlight, Previous = this });
									}
								}
							}
						}
					}
				}
				else//Departure logic
				{
					if (Number == 4)//Departure take a wing
					{
						logger.Info($"From line {Number} was take wing {CurrentFlight.Number} at {DateTime.Now.ToShortDateString()}");
						ReleaseLine();
					}
					else
					{
						bool startWaiting = true;
						Dictionary<Line, int> counts = new Dictionary<Line, int>();
						logger.Info($"Line {Number} start checking for next line");
						foreach (var line in NextForDeparture)//Checking next line in path
						{
							if (!line.Busy)//Relocation to next line
							{
								logger.Info($"Line {Number} find that {line.Number} is free and send {CurrentFlight.Number}");
								line.TakeFlight(new FlightPosition { Craft = CurrentFlight, Previous = null });
								ReleaseLine();
								startWaiting = false;
								break;
							}
							else//Form list of counts of next lines queues
							{
								logger.Info($"Line {Number} add count {line.Flights.Count} on {line.Number}");
								counts.Add(line, line.Flights.Count);
							}

						}
						if (startWaiting)//Situation when you need to expect
						{
							var line = counts.FirstOrDefault(pair => pair.Value == counts.Values.Min()).Key;//Line selection with the shortest queue
							logger.Info($"Line {Number} add to queue of {line.Number} at {CurrentFlight.Number}");
							//Add flight to queue of next line in path
							counts.FirstOrDefault(pair => pair.Value == counts.Values.Min()).Key.Flights.Enqueue(new FlightPosition { Craft = CurrentFlight, Previous = this });
						}
					}
				}
			}
		}
	}
}
