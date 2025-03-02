using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace THYWebApp.Models
{
    public class Flight
    {
        [Key]
        public int FlightID { get; set; }

        [Required]
        public required string FlighNumber { get; set; }

        [NotMapped]
        public int? AircraftID { get; set; } // Nullable

        [NotMapped]
        public int? DepartureAirportID { get; set; }

        [NotMapped]
        public int? ArrivalAirportID { get; set; }

        public required DateTime FlightDepartureDateTime { get; set; }

        public required DateTime FlightArrivalDateTime { get; set; }

        [NotMapped]
        public DateTime? FlightDelayTime { get; set; }

        [NotMapped]
        public int? DepartureGateID { get; set; }

        [NotMapped]
        public int? ArrivalGateID { get; set; }

        [Required]
        public required string FlightStatus { get; set; } 

        [Required]
        public required string FlightAirlineCompany { get; set; }

        [NotMapped]
        public int? FlightPriceID { get; set; }

        // NotMapped alanlar (Stored Procedure'den gelen ekstra sütunlar için)
        [NotMapped]
        public string? AircraftName { get; set; }

        public decimal? RegularFlight { get; set; }

        public decimal? ExtraFlight { get; set; }

        public string? TravelDuration { get; set; }

        public decimal? PremiumFlight { get; set; }

        public decimal? BusinessFlight { get; set; }
        [NotMapped]
        public string? DepartureAirportName { get; set; }
        [NotMapped]
        public string? ArrivalAirportName { get; set; }

        public required decimal FlightPrice { get; set; } // Stored procedure'den dönen fiyat için
    }
}
