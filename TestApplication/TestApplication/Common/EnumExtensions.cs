using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestApplication.Common
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumvalue)
        {
            var type = enumvalue.GetType();
            var member = type.GetMember(enumvalue.ToString()).FirstOrDefault();
            var attr = member?.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();
            return ((DisplayAttribute)attr)?.Name ?? member.Name;
        }
    }
}
