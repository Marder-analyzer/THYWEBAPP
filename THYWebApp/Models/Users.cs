using System.ComponentModel.DataAnnotations;
using THYWebApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.ComponentModel.DataAnnotations.Schema; // Dapper kullanıyorsanız bu gerekli



namespace THYWebApp.Models
{
    public class Users
    {
        [Key]
        public int UserID { get; set; } // NOT NULL
        public int UserTypeID { get; set; } // NULLABLE
        public string UserName { get; set; } = string.Empty; // NOT NULL
        public string? UserMiddleName { get; set; } // NULLABLE
        public string UserLastName { get; set; } = string.Empty; // NOT NULL
        public string? UserPassportNumber { get; set; } // NULLABLE
        public string UserNationality { get; set; } = string.Empty; // NOT NULL
        public DateTime UserDateOfBirth { get; set; } // NOT NULL
        public string? UserEmail { get; set; } // NULLABLE
        public string? UserPhone { get; set; } // NULLABLE
        [StringLength(11, ErrorMessage = "UserTCKimlik must be 11 characters.")]
        public string? UserTCKimlik { get; set; } // NULLABLE
        public string UserPassword { get; set; } = string.Empty; // NOT NULL
        public string? UserAddress { get; set; } // NULLABLE
    }
}
