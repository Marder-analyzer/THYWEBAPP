namespace THYWebApp.Models.ViewModels
{
    public class ReservationViewModel
    {
        public string DepartureAirport { get; set; }
        public string ArrivalAirport { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string PassengerCount { get; set; }
        public string PackageType { get; set; }
        public decimal TotalPrice { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public List<string> Passengers { get; set; } = new List<string>();
    }

}
