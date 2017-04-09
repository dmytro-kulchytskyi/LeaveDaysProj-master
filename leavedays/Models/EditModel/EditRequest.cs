using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace leavedays.Models.EditModel
{
    public class EditRequest
    {
        public int UserId { get; set; }

        public int CompanyId { get; set; }

        [Display(Name ="Status")]
        public string Status { get; set; }

        [Required(ErrorMessage = "The field must be required!")]
        [DataType(DataType.MultilineText)]
        [Display(Name ="Base")]
        public string RequestBase { get; set; }

        [Required(ErrorMessage ="The field must be required!")]
        [Display(Name ="Leave interval")]
        public string VacationDates { get; set;}
    }
}