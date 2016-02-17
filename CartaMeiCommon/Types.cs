using System;
using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

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
                var val = value.FixCoordinate(false);
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

        [Browsable(false)]
        public bool IsEmpty
        {
            get { return this.Width == 0 && this.Height == 0; }
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

    /// <summary>
    /// The boundaries for a map are comprised of the coordinates of the center and the span in latitude and longitude. 
    /// The center can be anywhere, but the spans cannot be more than 180º for the latitude and 360º for the longitude (and both obviously need to be positive).
    /// </summary>
    public class LatLonBoundaries : NotifyPropertyChangedBase
    {
        #region Constants

        public const double MaxLatitudeSpan = 180;
        public const double MaxLongitudeSpan = 360;

        #endregion

        #region Constructors

        public LatLonBoundaries() { }

        public LatLonBoundaries(double centerLatitude, double centerLongitude, double latitudeSpan, double longitudeSpan)
        {
            _centerLatitude = centerLatitude;
            _centerLongitude = centerLongitude;
            _latitudeSpan = latitudeSpan;
            _longitudeSpan = longitudeSpan;
            updateCardinalPoints();
        }

        #endregion

        #region Properties

        private double _centerLatitude;
        [Description("The latitude at the center of the map.")]
        [DisplayName("Center Latitude")]
        [PropertyOrder(0)]
        public double CenterLatitude
        {
            get { return _centerLatitude; }
            set
            {
                var val = value.FixCoordinate(true);
                if (_centerLatitude != val)
                {
                    _centerLatitude = val;
                    onPropetyChanged();
                    updateCardinalPoints();
                }
            }
        }

        private double _centerLongitude;
        [Description("The longitude at the center of the map.")]
        [DisplayName("Center Longitude")]
        [PropertyOrder(2)]
        public double CenterLongitude
        {
            get { return _centerLongitude; }
            set
            {
                var val = value.FixCoordinate(false);
                if (_centerLongitude != val)
                {
                    _centerLongitude = val;
                    onPropetyChanged();
                    updateCardinalPoints();
                }
            }
        }

        private double _latitudeSpan;
        [Description("The number of latitude degrees that are shown in the map.")]
        [DisplayName("Latitude Span")]
        [PropertyOrder(1)]
        public double LatitudeSpan
        {
            get { return _latitudeSpan; }
            set
            {
                var val = Math.Min(Math.Abs(value), 180);
                if (_latitudeSpan != val)
                {
                    _latitudeSpan = val;
                    onPropetyChanged();
                    updateCardinalPoints();
                }
            }
        }

        private double _longitudeSpan;
        [Description("The number of longitude degrees that are shown in the map.")]
        [DisplayName("Longitude Span")]
        [PropertyOrder(3)]
        public double LongitudeSpan
        {
            get { return _longitudeSpan; }
            set
            {
                var val = Math.Min(Math.Abs(value), 360);
                if (_longitudeSpan != val)
                {
                    _longitudeSpan = val;
                    onPropetyChanged();
                    updateCardinalPoints();
                }
            }
        }

        #region Calculated

        [ReadOnly(true)]
        public double LatitudeHalfSpan { get; private set; }
        [ReadOnly(true)]
        public double LongitudeHalfSpan { get; private set; }
        [ReadOnly(true)]
        public double LeftNotBound { get; set; }
        [ReadOnly(true)]
        public double TopNotBound { get; set; }

        private double _topLatitude;
        [Description("The latitude at the top of the map.")]
        [DisplayName("Top Latitude")]
        [PropertyOrder(4)]
        [ReadOnly(true)]
        public double TopLatitude
        {
            get { return _topLatitude; }
            private set
            {
                var val = value.FixCoordinate(true);
                if (_topLatitude != val)
                {
                    _topLatitude = val;
                    onPropetyChanged();
                }
            }
        }

        private double _bottomLatitude;
        [Description("The latitude at the bottom of the map.")]
        [DisplayName("Bottom Latitude")]
        [PropertyOrder(5)]
        [ReadOnly(true)]
        public double BottomLatitude
        {
            get { return _bottomLatitude; }
            private set
            {
                var val = value.FixCoordinate(true);
                if (_bottomLatitude != val)
                {
                    _bottomLatitude = val;
                    onPropetyChanged();
                }
            }
        }

        private double _leftLongitude;
        [Description("The longitude at the left of the map.")]
        [DisplayName("Left Longitude")]
        [PropertyOrder(6)]
        [ReadOnly(true)]
        public double LeftLongitude
        {
            get { return _leftLongitude; }
            private set
            {
                var val = value.FixCoordinate(false);
                if (_leftLongitude != val)
                {
                    _leftLongitude = val;
                    onPropetyChanged();
                }
            }
        }

        private double _rightLongitude;
        [Description("The longitude at the right of the map.")]
        [DisplayName("Right Longitude")]
        [PropertyOrder(7)]
        [ReadOnly(true)]
        public double RightLongitude
        {
            get { return _rightLongitude; }
            private set
            {
                var val = value.FixCoordinate(false);
                if (_rightLongitude != val)
                {
                    _rightLongitude = val;
                    onPropetyChanged();
                }
            }
        }

        [Browsable(false)]
        public bool CrossesReferenceMeridian { get { return Math.Abs(this.CenterLongitude) <= this.LongitudeHalfSpan; } }

        [Browsable(false)]
        public bool CrossesAntiMeridian { get { return (180 - Math.Abs(this.CenterLongitude)) <= this.LongitudeHalfSpan; } }

        [Browsable(false)]
        public bool CrossesNorthPole { get { return Math.Abs(90 - this.CenterLatitude) <= this.LatitudeHalfSpan; } }
        
        [Browsable(false)]
        public bool CrossesSouthPole { get { return Math.Abs(-90 - this.CenterLatitude) <= this.LatitudeHalfSpan; } }

        #endregion

        #endregion

        #region Object

        public override string ToString()
        {
            return
                this.CenterLatitude.GetHumanReadable(true) + " +- " + this.LatitudeSpan / 2 + "º / " +
                this.CenterLongitude.GetHumanReadable(true) + " +- " + this.LongitudeSpan / 2 + "º";
        }

        public override int GetHashCode()
        {
            return (int)(((this.CenterLatitude + 90) + (this.CenterLongitude + 90) * 180 + (this.LatitudeSpan + 180) * 64800 + (this.LongitudeSpan + 180) * 23328000) % int.MaxValue);
        }

        public override bool Equals(object obj)
        {
            var other = obj as LatLonBoundaries;
            return other != null && 
                other.CenterLatitude == this.CenterLatitude && 
                other.CenterLongitude == this.CenterLongitude && 
                other.LatitudeSpan == this.LatitudeSpan && 
                other.LongitudeSpan == this.LongitudeSpan;
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
                Math.Abs(coordinates.Latitude.FixCoordinate(true) - this.CenterLatitude) <= (this.LatitudeSpan / 2) &&
                Math.Abs(coordinates.Longitude.FixCoordinate(true) - this.CenterLongitude) <= (this.LongitudeSpan / 2);
        }

        /// <summary>
        /// Checks whether a rectangle intersects with this one.
        /// </summary>
        /// <param name="other">The rectangle to test.</param>
        /// <returns>true if <paramref name="other"/> intersects with this one, false otherwise.</returns>
        public bool Intersects(LatLonBoundaries other)
        {
            var latDiff = Math.Abs(other.CenterLatitude.FixCoordinate(true) - this.CenterLatitude);
            var lonDiff = Math.Abs(other.CenterLongitude.FixCoordinate(false) - this.CenterLongitude);
            return
                Math.Abs(other.CenterLatitude.FixCoordinate(true) - this.CenterLatitude) <= ((this.LatitudeSpan + other.LatitudeSpan) / 2) &&
                Math.Abs(other.CenterLongitude.FixCoordinate(true) - this.CenterLongitude) <= ((this.LongitudeSpan + other.LongitudeSpan) / 2);
        }

        public LineCrossType GetLineCrossType(LatLonCoordinates point1, LatLonCoordinates point2)
        {
            throw new NotImplementedException();
            /*
            This will be rather complicated: consider a map centered on a pole...
            */
        }

        [Flags]
        public enum LineCrossType
        {
            None = 0,
            NorthToSouth = 1,
            SouthToNorth = 2,
            EastToWest = 4,
            WestToEast = 8,
        }

        #endregion

        #region Tools

        private readonly object _updateCardinalsLocker = new object();
        private void updateCardinalPoints()
        {
            lock (_updateCardinalsLocker)
            {
                this.LatitudeHalfSpan = this.LatitudeSpan / 2;
                this.LongitudeHalfSpan = this.LongitudeSpan / 2;
                this.TopNotBound = this.CenterLatitude + this.LatitudeHalfSpan;
                this.LeftNotBound = this.CenterLongitude - this.LongitudeHalfSpan;
                this.TopLatitude = this.CenterLatitude + this.LatitudeHalfSpan;
                this.BottomLatitude = this.CenterLatitude - this.LatitudeHalfSpan;
                this.LeftLongitude = this.CenterLongitude - this.LongitudeHalfSpan;
                this.RightLongitude = this.CenterLongitude + this.LongitudeHalfSpan;
            }
        }

        #endregion
    }
}
