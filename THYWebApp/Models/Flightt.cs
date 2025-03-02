using System.ComponentModel.DataAnnotations;

namespace THYWebApp.Models
{
    public class Flightt
    {
        [Key]
        public int FlightID { get; set; }
        public string FlightNumber { get; set; }
        public DateTime FlightDepartureDateTime { get; set; }
        public DateTime FlightArrivalDateTime { get; set; }
        public string DepartureAirportName { get; set; }
        public string ArrivalAirportName { get; set; }
        public string FlightStatus { get; set; } // Durum (Kalkış Yaptı, İniş Yaptı, Planlandı)
    }

}
