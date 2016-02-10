using System;

namespace CartaMei.Common
{
    public static class Tools
    {
        public static double FixCoordinate(this double coordinate, int max)
        {
            var max2 = max * 2;
            if (coordinate >= 0)
            {
                coordinate = (coordinate + max) % max2 - max;
                if (coordinate == (0 - max)) coordinate = max;
            }
            else
            {
                coordinate = 0 - ((Math.Abs(coordinate) + max) % max2 - max);
                if (coordinate == max) coordinate = 0 - max;
            }
            return coordinate;
        }

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
