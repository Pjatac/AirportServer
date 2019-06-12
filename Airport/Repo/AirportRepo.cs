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
			if (!_dbContext.LinesSnapshots.Any())
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

		public void AddLinesSnapshot(LineDB changedLine)
		{
			lock (_dbContext)
			{
				var newSnapshot = new LineSnapshot { LineSnapshotId = 0, Time = DateTime.Now };
				_dbContext.LinesSnapshots.Add(newSnapshot);
				_dbContext.SaveChanges();
				newSnapshot = _dbContext.LinesSnapshots.LastOrDefault();
				var tmpLines = new List<LineDB>();
				tmpLines = _dbContext.LinesSnapshots.FirstOrDefault(l => l.LineSnapshotId == newSnapshot.LineSnapshotId - 1).Lines;
				newSnapshot.Lines = new List<LineDB>(tmpLines);
				newSnapshot.Lines.FirstOrDefault(l => l.Number == changedLine.Number).IsBusy = changedLine.IsBusy;
				newSnapshot.Lines.FirstOrDefault(l => l.Number == changedLine.Number).Direction = changedLine.Direction;
				for (int i = 0; i < 8; i++)
				{
					var tmp = new LineDB { LineSnapshot = newSnapshot, IsBusy = newSnapshot.Lines[i].IsBusy, Number = i + 1, Direction = newSnapshot.Lines[i].Direction, LineSnapshotId = newSnapshot.LineSnapshotId };
					newSnapshot.Lines[i] = tmp;
				}
				_dbContext.Update(_dbContext.LinesSnapshots.LastOrDefault());
				_dbContext.SaveChanges();
			}
		}
		public void AddMove(FlightMove move)
		{
			lock (_dbContext)
			{
				move.FlightMoveId = 0;
				_dbContext.Moves.Add(move);
				_dbContext.SaveChanges();
			}
		}
	}
}
