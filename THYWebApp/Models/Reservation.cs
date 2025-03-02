using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace THYWebApp.Models
{
    public class Reservation
    {
        [Key]
        public required int ReservationID { get; set; }
        public int NonMemberCustomerID { get; set; }
        public int UserID { get; set; }
        public int FlightID { get; set; }
        public int FlyPacketsID { get; set; }
        public DateTime? ReservationDate { get; set; }
        public required string ReservationStatus { get; set; }
    }
    

}
