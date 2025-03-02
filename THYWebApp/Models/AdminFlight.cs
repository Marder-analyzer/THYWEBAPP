using System.ComponentModel.DataAnnotations;

namespace THYWebApp.Models
{
    public class AdminFlight
    {
            [Key]
            public int? FlightID { get; set; } // Uçuş ID
            public string FlighNumber { get; set; } // Uçuş Numarası
            public string AircraftName { get; set; } // Uçağın Adı

            public string DepartureAirportName { get; set; } // Kalkış Havaalanı Adı
            public string ArrivalAirportName { get; set; } // Varış Havaalanı Adı
            public string DepartureAirportCode { get; set; } // Kalkış Havaalanı Kodu
            public string ArrivalAirportCode { get; set; } // Varış Havaalanı Kodu
            public int FlightDelayTime { get; set; } // Uçuş Gecikme Süresi
    }
}


