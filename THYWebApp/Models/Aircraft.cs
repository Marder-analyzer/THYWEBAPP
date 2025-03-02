using System.ComponentModel.DataAnnotations;

namespace THYWebApp.Models
{
    public class Aircraft
    {
        [Key]
        public required int AircraftID { get; set; }
        public required string AircraftName { get; set; }
        public required string AircraftModel { get; set; }
        public required DateTime AircraftProductionDate { get; set; }
        public required int AircraftSeatCapacity { get; set; }
    }
}
