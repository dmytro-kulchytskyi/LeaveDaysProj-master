using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace leavedays.Models.ViewModels.Account
{
    public class CreateCompanyViewModel
    {
        [Required]
        [StringLength(250)]
        public string CompanyName {get; set;}

        [Required]
        [StringLength(250)]
        [RegularExpression("^([A-Za-z0-9])+$", ErrorMessage = "Company URL must contains only letters and digits")]
        public string CompanyUrl { get; set; }


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

        [ScaffoldColumn(false)]
        public string[] Roles { get; set; }

        public string RolesLine { get; set; }
    }
}