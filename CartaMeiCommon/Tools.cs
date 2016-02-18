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
            result.SetDefaults();
            return result;
        }

        public static void SetDefaults(this object obj)
        {
            if (obj != null)
            {
                foreach (var property in obj.GetType().GetPublicSetters())
                {
                    var defaultAttribute = property.GetCustomAttribute<DefaultValueAttribute>(true);
                    if (defaultAttribute != null)
                    {
                        property.SetValue(obj, defaultAttribute.Value);
                    }
                }
            }
        }

        public static PropertyInfo[] GetPublicSetters(this Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);
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

        public static void SafeFreeze(this Freezable freezable)
        {
            if ( freezable != null && !freezable.IsFrozen && freezable.CanFreeze)
            {
                freezable.Freeze();
            }
        }

        public static T GetFrozenCopy<T>(this T freezable) where T : Freezable
        {
            if (freezable == null) return null;
            return (T)freezable.GetAsFrozen();
        }
    }
}
