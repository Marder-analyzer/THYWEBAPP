namespace THYWebApp.Models.ViewModels
{
    public class AdminViewModel
    {
        public List<Aircraft> Aircrafts { get; set; }
        public List<Flight> Flights { get; set; }

        public AdminViewModel()
        {
            Aircrafts = new List<Aircraft>();
            Flights = new List<Flight>();
        }
    }
}

