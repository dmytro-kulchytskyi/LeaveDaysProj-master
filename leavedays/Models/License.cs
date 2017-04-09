using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Models
{
    public class License
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual double Price { get; set; }

        private ISet<Module> _Modules = new HashSet<Module>();
        public virtual ISet<Module> Modules
        {
            get
            {
                return _Modules;
            }
            set
            {
                _Modules = value;
            }
        }
    }
}
