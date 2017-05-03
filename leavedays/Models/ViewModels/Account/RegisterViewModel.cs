using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace leavedays.Models.ViewModels.Account
{
    public class CreateUserModel
    {
        [Required(ErrorMessage = "You must select a license")]
        public string LicenseName { get; set; }

        [ScaffoldColumn(false)]
        public string RolesLine { get; set; }

        [ScaffoldColumn(false)]
        public IEnumerable<string> Roles { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone(ErrorMessage = "Bad phone number")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(250)]
        public string CompanyName { get; set; }

        [Required]
        [StringLength(250)]
        [RegularExpression("^([A-Za-z0-9])+$", ErrorMessage = "Invalid URL")]
        public string CompanyUrl { get; set; }

        [Required]
        [StringLength(250)]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(250, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [StringLength(250)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(250)]
        public string LastName { get; set; }

        [ScaffoldColumn(false)]
        public IList<DefaultLicense> LicenseList { get; set; }
    }
}