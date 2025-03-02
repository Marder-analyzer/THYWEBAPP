namespace THYWebApp.Models.ViewModels
{
    public class SearchFlightsViewModel
    {
        public List<string> Airports { get; set; } = new List<string>();
        public string FlightType { get; set; }
        public DateTime DepartureDate { get; set; }
        public string DepartureAirport { get; set; }
        public string ArrivalAirport { get; set; }
        public string ErrorMessage { get; set; }
    }
}
