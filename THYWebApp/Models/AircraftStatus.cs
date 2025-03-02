namespace THYWebApp.Models
{
    public class AircraftStatus
    {
        public int AircraftStatusID { get; set; }
        public int AircraftID { get; set; }
        public decimal AircraftFuelLevel { get; set; }
        public DateTime AircraftLastMaintenanceDate { get; set; }
        public bool AirplaneStatus { get; set; }
    }
}
