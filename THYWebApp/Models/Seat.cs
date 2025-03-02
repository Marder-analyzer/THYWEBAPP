using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace THYWebApp.Models
{
    public class Seat
    {
        [Key]
        public required int SeatID { get; set; } // Koltuk için benzersiz kimlik

        public required int FlightID { get; set; } // Bilet ID

        public required string SeatNumber { get; set; } // İlişkilendirilmiş Bilet
        public string SeatStatus { get; set; } // Prosedürden dönen ek bilgi
    }
}
