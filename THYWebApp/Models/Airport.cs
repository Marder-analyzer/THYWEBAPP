using System.ComponentModel.DataAnnotations.Schema;

namespace THYWebApp.Models
{
    public class Airport
    {
        public int AirportID { get; set; }
        public required string AirportName { get; set; }

        [NotMapped]
        public required string AirportCity { get; set; }
        [NotMapped]
        public required string AirportCountry { get; set; }
        [NotMapped]
        public required string AirportCode { get; set; }
    }
}
