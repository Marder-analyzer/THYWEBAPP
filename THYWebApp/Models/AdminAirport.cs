using System.ComponentModel.DataAnnotations;

namespace THYWebApp.Models
{
    public class AdminAirport
    {
        [Required]
        [MaxLength(255)]
        public string AirportName { get; set; }

        [MaxLength(255)]
        public string City { get; set; }

        [MaxLength(255)]
        public string Country { get; set; }

        [MaxLength(10)]
        public string AirportCode { get; set; }
    }
}
