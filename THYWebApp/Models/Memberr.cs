using System.ComponentModel.DataAnnotations;

namespace THYWebApp.Models
{
    public class Memberr
    {
        [Key]
        public int MemberID { get; set; }
        public int UserID { get; set; }
        public string MembershipNumber { get; set; }
        public int MemberMiles { get; set; }
        public DateTime MemberJoinDate { get; set; }
    }
}
