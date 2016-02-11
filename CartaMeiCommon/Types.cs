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

        public static implicit operator System.Windows.Point(PixelCoordinates coordinates)
        {
            return new System.Windows.Point(coordinates.X, coordinates.Y);
        }

        public static implicit operator PixelCoordinates(System.Windows.Point point)
        {
            return new PixelCoordinates() { X = point.X, Y = point.Y };
        }

        #endregion
    }

    public class LatLonCoordinates
    {
        #region Properties

        private double _longitude;
        public double Longitude
        {
            get { return _longitude; }
            set
            {
                _longitude = value;
                var val = value.FixCoordinate(180);
                if (this.SafeLongitude != val)
                {
                    this.SafeLongitude = val;
                }
            }
        }
        public double SafeLongitude { get; private set; }
        
        public double Latitude { get; set; }

        #endregion

        #region Object

        public override string ToString()
        {
            return this.Latitude.GetHumanReadable(true) + " " + this.Longitude.GetHumanReadable(false);
        }

        public override int GetHashCode()
        {
            return (int)(this.SafeLongitude + this.Latitude * short.MaxValue);
        }

        public override bool Equals(object obj)
        {
            var other = obj as LatLonCoordinates;
            return other != null && other.SafeLongitude == this.SafeLongitude && other.Latitude == this.Latitude;
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

        public static implicit operator System.Windows.Size(PixelSize size)
        {
            return new System.Windows.Size(size.Width, size.Height);
        }

        public static implicit operator PixelSize(System.Windows.Size size)
        {
            return new PixelSize() { Width = (int)size.Width, Height = (int)size.Height };
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
                var val = value.FixCoordinate(90);
                if (_latMin != val)
                {
                    _latMin = val;
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
                var val = value.FixCoordinate(90);
                if (_latMax != val)
                {
                    _latMax = val;
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
                var val = value.FixCoordinate(180);
                if (_lonMin != val)
                {
                    _lonMin = val;
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
                var val = value.FixCoordinate(180);
                if (_lonMax != val)
                {
                    _lonMax = val;
                    onPropetyChanged();
                }
            }
        }

        [Browsable(false)]
        public bool CrossesAntiMeridian { get { return this.LonMin > this.LonMax; } }
        
        [Browsable(false)]
        public double SafeLonMax { get { return this.CrossesAntiMeridian ? this.LonMax + 360 : this.LonMax; } }

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

        #region Functions

        /// <summary>
        /// Checks whether a point is within these limits.
        /// </summary>
        /// <param name="coordinates">The point to test.</param>
        /// <returns>true if <paramref name="coordinates"/> is within these limits, false otherwise.</returns>
        public bool Contains(LatLonCoordinates coordinates)
        {
            return 
                coordinates.SafeLongitude >= this.LonMin && coordinates.SafeLongitude <= this.SafeLonMax &&
                coordinates.Latitude >= this.LatMin && coordinates.Latitude <= this.LatMax;
        }

        /// <summary>
        /// Checks whether a rectangle intersects with this one.
        /// </summary>
        /// <param name="other">The rectangle to test.</param>
        /// <returns>true if <paramref name="other"/> intersects with this one, false otherwise.</returns>
        public bool Intersects(LatLonBoundaries other)
        {
            return
                this.LonMin <= other.SafeLonMax && this.SafeLonMax >= other.LonMin &&
                this.LatMin <= other.LatMax && this.LatMax >= other.LatMin;
        }

        #endregion
    }
}
