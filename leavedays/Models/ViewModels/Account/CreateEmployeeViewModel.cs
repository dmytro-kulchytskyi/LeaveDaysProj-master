using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace leavedays.Models.ViewModels.Account
{
    public class CreateEmployeeViewModel
    {
        [Required]
        [StringLength(250)]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(250, MinimumLength = 6)]
        public string Password { get; set; }

        //public int CompanyId { get; set; }

        [Required]
        [StringLength(250)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(250)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        [ScaffoldColumn(false)]
        public string[] Roles { get; set; }
        
        public string RolesLine { get; set; }
    }
}