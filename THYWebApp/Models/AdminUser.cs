using System.ComponentModel.DataAnnotations;

namespace THYWebApp.Models
{
    public class AdminUser
    {
            [Required]
            [MaxLength(11)]
            public string UserTCKimlik { get; set; }

            [Required]
            public int UserTypeID { get; set; }

            [Required]
            [MaxLength(100)]
            public string UserName { get; set; }

            [MaxLength(100)]
            public string UserMiddleName { get; set; }

            [Required]
            [MaxLength(100)]
            public string UserLastName { get; set; }

            [Required]
            [MaxLength(50)]
            public string UserNationality { get; set; }

            [Required]
            public DateTime UserDateOfBirth { get; set; }

            [Required]
            [MaxLength(100)]
            [EmailAddress]
            public string UserEmail { get; set; }

            [Required]
            [MaxLength(100)]
            public string UserPassword { get; set; }
    }

}

