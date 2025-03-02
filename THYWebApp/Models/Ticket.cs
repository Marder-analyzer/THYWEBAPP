using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace THYWebApp.Models
{
    public class Ticket
    {
        [Key]
        public required int TicketID { get; set; }
        [NotMapped]
        public int ReservationID { get; set; }
        [NotMapped]
        public int? FlightID { get; set; }
        [NotMapped]
        public int? DepartureGateID { get; set; }

        [NotMapped]
        public required string TicketStatus { get; set; }
        [NotMapped]
        public string? SeatNumber { get; set; }
        [NotMapped]
        public string? PassengerEmail { get; set; } // Kullanıcı veya yolcu e-posta
        [NotMapped]
        public string? PassengerPhoneNumber { get; set; } // Kullanıcı veya yolcu telefon numarası
        [NotMapped]
        public string? FirstName { get; set; }
        [NotMapped]
        public string LastName { get; set; }
        [NotMapped]
        public string FlightAirlineCompany { get; set; }
        [NotMapped]
        public string DepartureAirportName { get; set; }
        [NotMapped]
        public string ArrivalAirportName { get; set; }
        [NotMapped]
        public DateTime DepartureDate { get; set; }
        [NotMapped]
        public DateTime ReservationDate { get; set; }
        [NotMapped]

        public string FlighNumber { get; set; }

    }
}
