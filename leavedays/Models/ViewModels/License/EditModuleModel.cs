using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace leavedays.Models.ViewModels.License
{
    public class EditModuleModel
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }

        [StringLength(200, MinimumLength = 5)]
        public string Description { get; set; }

        [RegularExpression(@"^\d+((\.)?\d+)?$")]
        public double Price { get; set; }

        [ScaffoldColumn(false)]
        public List<DefaultLicense> ModuleLicenses { get; set; }

        [ScaffoldColumn(false)]
        public List<DefaultLicense> AllLicenses { get; set; }
   }
}