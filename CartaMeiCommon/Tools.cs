using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;

namespace CartaMei.Common
{
    public static class Tools
    {
        public static T GetDefault<T>() where T : new()
        {
            var result = typeof(T).GetDefault();
            return result != null ? (T)result : default(T);
        }
        
        public static object GetDefault(this Type type)
        {
            var result = Activator.CreateInstance(type);
            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty))
            {
                var defaultAttribute = property.GetCustomAttribute<DefaultValueAttribute>(true);
                if (defaultAttribute != null)
                {
                    property.SetValue(result, defaultAttribute.Value);
                }
            }
            return result;
        }

        public static double FixCoordinate(this double coordinate, bool isLatitude)
        {
            return FixCoordinate(coordinate, isLatitude ? 90 : 180);
        }

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

        public static void SafeInvalidate(this UIElement uiElement)
        {
            uiElement.Dispatcher.InvokeAsync(() => uiElement.InvalidateVisual());
        }
    }
}
