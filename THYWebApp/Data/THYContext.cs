using Microsoft.EntityFrameworkCore;
using THYWebApp.Models;
using Microsoft.Data.SqlClient;

namespace THYWebApp.Data
{
    public class THYContext : DbContext
    {
        public THYContext(DbContextOptions<THYContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=MEHMETCAN;Initial Catalog=DBFly;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");
            }
        }
        public DbSet<UserType> UserType { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Memberr> Memberr { get; set; }
        public DbSet<NonMemberCustomer> NonMemberCustomer { get; set; }
        public DbSet<LoginRecord> LoginRecord { get; set; }
        public DbSet<Aircraft> Aircraft { get; set; }
        public DbSet<AircraftStatus> AircraftStatus { get; set; }
        public DbSet<AircraftErrorReport> AircraftErrorReport { get; set; }
        public DbSet<Airport> Airport { get; set; }
        public DbSet<AirportGate> AirportGate { get; set; }
        public DbSet<FlightPrice> FlightPrice { get; set; }
        public DbSet<Flight> Flight { get; set; }
        public DbSet<Flightt> Flightt { get; set; }
        public DbSet<Baggage> Baggage { get; set; }
        public DbSet<BaggagePrice> BaggagePrice { get; set; }
        public DbSet<BaggageGate> BaggageGate { get; set; }
        public DbSet<FlyPackets> FlyPacket { get; set; }
        public DbSet<Reservation> Reservation { get; set; }
        public DbSet<Ticket> Ticket { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<Seat> Seat { get; set; }
        public DbSet<AdminFlight> AdminFlight { get; set; }
    }

}
