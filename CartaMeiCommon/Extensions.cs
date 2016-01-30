using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartaMei.Common
{
    public static class Extensions
    {
        public static string GetHumanReadable(this double coordinate, bool isLatitude)
        {
            var isNegative = coordinate < 0;
            var cardinal = isLatitude
                ? (isNegative ? "S" : "N")
                : (isNegative ? "W" : "E");
            return Math.Round(Math.Abs(coordinate), 5) + "º" + cardinal;
        }
    }
}
