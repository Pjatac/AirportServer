using Microsoft.EntityFrameworkCore;

namespace Airport.DL
{
	public class AirportContext : DbContext
	{
		public DbSet<LineSnapshot> LinesSnapshots { get; set; }
		public DbSet<LineDB> Lines { get; set; }
		public DbSet<FlightMove> Moves { get; set; }
		public DbSet<Arrival> Arrivals { get; set; }
		public DbSet<Departure> Departures { get; set; }
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite("Data Source=airport.db");
		}
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<LineSnapshot>()
				.HasMany(p => p.Lines)
				.WithOne(u => u.LineSnapshot)
				.HasForeignKey(p => p.LineSnapshotId);
		}
	}
}