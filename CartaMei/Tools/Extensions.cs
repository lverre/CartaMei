using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CartaMei.Tools
{
    public static class Extensions
    {
        public static T GetCustomAttribute<T>(this Type type, bool inherit = false)
            where T : Attribute
        {
            var attributes = type.GetCustomAttributes(typeof(T), inherit);
            if (attributes != null)
            {
                return (T)attributes[0];
            }
            else
            {
                return null;
            }
        }

        public static T GetCustomAttribute<T>(this Assembly assembly, bool inherit = false)
            where T : Attribute
        {
            var attributes = assembly.GetCustomAttributes(typeof(T), inherit);
            if (attributes != null)
            {
                return (T)attributes[0];
            }
            else
            {
                return null;
            }
        }
    }
}
