using System;
using System.ComponentModel;

namespace CartaMei.Common
{
    public class PixelCoordinates
    {
        #region Properties

        public double X { get; set; }

        public double Y { get; set; }

        #endregion

        #region Object

        public override string ToString()
        {
            return this.X + " x " + this.Y;
        }

        public override int GetHashCode()
        {
            return (int)(this.X + this.Y * short.MaxValue);
        }

        public override bool Equals(object obj)
        {
            var other = obj as PixelCoordinates;
            return other != null && other.X == this.Y && other.X == this.Y;
        }

        public static bool operator ==(PixelCoordinates x, PixelCoordinates y)
        {
            return equals(x, y);
        }

        public static bool operator !=(PixelCoordinates x, PixelCoordinates y)
        {
            return !equals(x, y);
        }

        private static bool equals(PixelCoordinates x, PixelCoordinates y)
        {
            return System.Object.ReferenceEquals(x, y) || (x?.Equals(y) ?? false);
        }

        #endregion
    }

    public class LatLonCoordinates
    {
        #region Properties

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        #endregion

        #region Object

        public override string ToString()
        {
            return this.Latitude.GetHumanReadable(true) + " " + this.Longitude.GetHumanReadable(false);
        }

        public override int GetHashCode()
        {
            return (int)(this.Longitude + this.Latitude * short.MaxValue);
        }

        public override bool Equals(object obj)
        {
            var other = obj as LatLonCoordinates;
            return other != null && other.Longitude == this.Latitude && other.Longitude == this.Latitude;
        }

        public static bool operator ==(LatLonCoordinates x, LatLonCoordinates y)
        {
            return equals(x, y);
        }

        public static bool operator !=(LatLonCoordinates x, LatLonCoordinates y)
        {
            return !equals(x, y);
        }

        private static bool equals(LatLonCoordinates x, LatLonCoordinates y)
        {
            return System.Object.ReferenceEquals(x, y) || (x?.Equals(y) ?? false);
        }

        #endregion
    }

    public class PixelSize : NotifyPropertyChangedBase
    {
        #region Properties

        private int _width;
        [Description("The width.")]
        [DisplayName("Width")]
        public int Width
        {
            get { return _width; }
            set
            {
                if (_width != value)
                {
                    _width = value;
                    onPropetyChanged();
                }
            }
        }

        private int _height;
        [Description("The height.")]
        [DisplayName("Height")]
        public int Height
        {
            get { return _height; }
            set
            {
                if (_height != value)
                {
                    _height = value;
                    onPropetyChanged();
                }
            }
        }

        #endregion

        #region Object

        public override string ToString()
        {
            return this.Width + " x " + this.Height;
        }

        public override int GetHashCode()
        {
            return this.Height + this.Width * short.MaxValue;
        }

        public override bool Equals(object obj)
        {
            var other = obj as PixelSize;
            return other != null && other.Height == this.Height && other.Width == this.Width;
        }

        public static bool operator ==(PixelSize x, PixelSize y)
        {
            return equals(x, y);
        }

        public static bool operator !=(PixelSize x, PixelSize y)
        {
            return !equals(x, y);
        }

        private static bool equals(PixelSize x, PixelSize y)
        {
            return System.Object.ReferenceEquals(x, y) || (x?.Equals(y) ?? false);
        }

        #endregion
    }

    public class LatLonBoundaries : NotifyPropertyChangedBase
    {
        #region Properties

        private double _latMin;
        [Description("The southernmost latitude.")]
        [DisplayName("South Latitude")]
        public double LatMin
        {
            get { return _latMin; }
            set
            {
                if (_latMin != value)
                {
                    _latMin = value;
                    onPropetyChanged();
                }
            }
        }

        private double _latMax;
        [Description("The northernmost latitude.")]
        [DisplayName("North Latitude")]
        public double LatMax
        {
            get { return _latMax; }
            set
            {
                if (_latMax != value)
                {
                    _latMax = value;
                    onPropetyChanged();
                }
            }
        }

        private double _lonMin;
        [Description("The westernmost longitude.")]
        [DisplayName("West Longitude")]
        public double LonMin
        {
            get { return _lonMin; }
            set
            {
                if (_lonMin != value)
                {
                    _lonMin = value;
                    onPropetyChanged();
                }
            }
        }

        private double _lonMax;
        [Description("The easternmost longitude.")]
        [DisplayName("East Longitude")]
        public double LonMax
        {
            get { return _lonMax; }
            set
            {
                if (_lonMax != value)
                {
                    _lonMax = value;
                    onPropetyChanged();
                }
            }
        }

        #endregion

        #region Object

        public override string ToString()
        {
            return
                this.LatMin.GetHumanReadable(true) + " " + this.LonMin.GetHumanReadable(false) + " / " +
                this.LatMax.GetHumanReadable(true) + " " + this.LonMax.GetHumanReadable(false);
        }

        public override int GetHashCode()
        {
            return (int)(((this.LatMin + 90) + (this.LatMax + 90) * 180 + (this.LonMin + 180) * 64800 + (this.LonMax + 180) * 23328000) % int.MaxValue);
        }

        public override bool Equals(object obj)
        {
            var other = obj as LatLonBoundaries;
            return other != null && other.LatMax == this.LatMax && other.LatMin == this.LatMin && other.LonMax == this.LonMax && other.LonMin == this.LonMin;
        }

        public static bool operator ==(LatLonBoundaries x, LatLonBoundaries y)
        {
            return equals(x, y);
        }

        public static bool operator !=(LatLonBoundaries x, LatLonBoundaries y)
        {
            return !equals(x, y);
        }

        private static bool equals(LatLonBoundaries x, LatLonBoundaries y)
        {
            return System.Object.ReferenceEquals(x, y) || (x?.Equals(y) ?? false);
        }

        #endregion
    }
}
