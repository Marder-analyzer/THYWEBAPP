using System.ComponentModel.DataAnnotations;

namespace THYWebApp.Models
{
    public class NonMemberCustomer
    {
        [Key]
        public int NonMemberCustomerID { get; set; }
        public required string PassengerFirstName { get; set; }
        public string? PassengerMiddleName { get; set; }
        public required string PassengerLastName { get; set; }
        public string? PassengerTCKimlik { get; set; }
        public string? PassengerPassportNumber { get; set; }
        public required string PassengerEmail { get; set; }
        public required string PassengerPhoneNumber { get; set; }
        public required string PassengerNationality { get; set; }
        public required DateTime PassengerDateOfBirth { get; set; }

        public required string PassengerGender { get; set; }
    }
}
