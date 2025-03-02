using System.ComponentModel.DataAnnotations;

namespace THYWebApp.Models
{
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; }
        public int TicketID { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal PaymentAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
    }
}
