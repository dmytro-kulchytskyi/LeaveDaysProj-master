using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Models
{
    public class Role : IRole<int>
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
    }
}