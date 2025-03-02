namespace THYWebApp.Models
{
    public class Baggage
    {
        public int BaggageID { get; set; }
        public int? NonMemberCustomerID { get; set; }
        public int? UserID { get; set; }
        public int FlightID { get; set; }
        public decimal BaggageWeight { get; set; }
        public string BaggageType { get; set; }
        public string BaggageStatus { get; set; }
    }
}
