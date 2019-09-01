using Airport.DL;
using Airport.Infra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Airport.Repo
{
	public class AirportRepo : IRepo
	{
		private static AirportContext _dbContext;
		public AirportRepo(AirportContext dbContext)
		{
			_dbContext = dbContext;
			//Add first clear snapshot
			lock (_dbContext)
			{
				LineSnapshot newSnapShot = new LineSnapshot
				{
					Time = DateTime.Now,
					Lines = new List<LineDB> {
				new LineDB { IsBusy = false, Number = 1, Direction = string.Empty, LineSnapshotId = 1 },
				new LineDB { IsBusy = false, Number = 2, Direction = string.Empty, LineSnapshotId = 1 },
				new LineDB { IsBusy = false, Number = 3, Direction = string.Empty, LineSnapshotId = 1 },
				new LineDB { IsBusy = false, Number = 4, Direction = string.Empty, LineSnapshotId = 1 },
				new LineDB { IsBusy = false, Number = 5, Direction = string.Empty, LineSnapshotId = 1 },
				new LineDB { IsBusy = false, Number = 6, Direction = string.Empty, LineSnapshotId = 1 },
				new LineDB { IsBusy = false, Number = 7, Direction = string.Empty, LineSnapshotId = 1 },
				new LineDB { IsBusy = false, Number = 8, Direction = string.Empty, LineSnapshotId = 1 }}
				};
				_dbContext.LinesSnapshots.Add(newSnapShot);
				_dbContext.SaveChanges();
			}
		}

		public void AddArival(Arrival arrival)
		{
			lock (_dbContext)
			{
				_dbContext.Arrivals.Add(arrival);
				_dbContext.SaveChanges();
			}
		}

		public void AddDeparture(Departure departure)
		{
			lock (_dbContext)
			{
				_dbContext.Departures.Add(departure);
				_dbContext.SaveChanges();
			}
		}

		public void AddLinesSnapshot(LineDB changedLine, int previousLineNumber)
		{
			lock (_dbContext)
			{
				LineSnapshot newSnapshot = new LineSnapshot()
				{
					Time = DateTime.Now,
					Lines = new List<LineDB>()
				};
				//Get last snapshot lines
				var tmp = _dbContext.LinesSnapshots.LastOrDefault().Lines;
				_dbContext.LinesSnapshots.Add(newSnapshot);
				_dbContext.SaveChanges();
				for (int i = 0; i < 8; i++)
				{
					newSnapshot.Lines.Add(new LineDB { LineSnapshot = newSnapshot, IsBusy = tmp[i].IsBusy, Number = tmp[i].Number, Direction = tmp[i].Direction, LineSnapshotId = newSnapshot.LineSnapshotId });
				}
				//Check for move from line to line
				if (previousLineNumber > 0)
				{
					//Clear previous position
					newSnapshot.Lines[previousLineNumber - 1].IsBusy = false;
					newSnapshot.Lines[previousLineNumber - 1].Direction = string.Empty;
				}
				//Correct current position with new data
				newSnapshot.Lines[changedLine.Number - 1].IsBusy = changedLine.IsBusy;
				newSnapshot.Lines[changedLine.Number - 1].Direction = changedLine.Direction;
				_dbContext.LinesSnapshots.Update(newSnapshot);
				_dbContext.SaveChanges();
			}
		}
		public void AddMove(FlightMove move)
		{
			lock (_dbContext)
			{
				_dbContext.Moves.Add(move);
				_dbContext.SaveChanges();
			}
		}
	}
}
