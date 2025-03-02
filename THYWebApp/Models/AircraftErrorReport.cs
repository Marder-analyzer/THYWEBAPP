using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace THYWebApp.Models
{
    public class AircraftErrorReport
    {
        [Key]
        public int AircraftErrorReportID { get; set; }
        public int? AircraftStatusID { get; set; }
        public string AircraftErrorDescription { get; set; }
        public DateTime AircraftReportDate { get; set; }
        public string AircraftName { get; set; }
        public int AircraftID { get; set; }
    }
}
