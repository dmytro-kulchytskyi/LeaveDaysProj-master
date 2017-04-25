using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays
{
    public static class StringHelper
    {
        public static List<string> SplitByComma(this string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return new List<string>();
            }
            line = line.Trim(',');
            var roles = line.Split(',').ToList();
            if (roles.Count == 0)
            {
                return new List<string>();
            }
            else
            {
                return roles;
            }
        }
    }
}