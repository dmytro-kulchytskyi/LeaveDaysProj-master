using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Models
{
    public class DefaultLicense
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual double Price { get; set; }

        private ISet<DefaultModule> _DefaultModules = new HashSet<DefaultModule>();
        public virtual ISet<DefaultModule> DefaultModules
        {
            get
            {
                return _DefaultModules;
            }
            set
            {
                _DefaultModules = value;
            }
        }
    }
}