namespace THYWebApp.Models
{
    public class LoginRecord
    {
        public int LoginRecordID { get; set; }
        public int UserID { get; set; }
        public DateTime LoginDate { get; set; }
        public string LoginMethod { get; set; }
    }
}
