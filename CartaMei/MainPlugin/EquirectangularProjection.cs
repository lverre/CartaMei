using CartaMei.Common;
using System;
using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace CartaMei.MainPlugin
{
    class EquirectangularProjection : NotifyPropertyChangedBase, IProjection
    {
        #region Constants

        internal const string ProjectionName = "Equirectangular Projection";
        internal const string ProjectionDescription = "The simplest projection there can be: coordinates are just scaled to fit in the canvas. That means the areas far from the equator and the reference longitude appear much bigger than they are in reality.";

        #endregion

        #region IProjection

        [Browsable(false)]
        public virtual IMap Map { get; set; }

        [Browsable(false)]
        public virtual string Name
        {
            get { return ProjectionName; }
        }

        [Browsable(false)]
        public virtual bool SupportsReferenceChange
        {
            get { return true; }
        }

        public virtual PixelCoordinates LatLonToPixel(LatLonCoordinates latLonCoordinates)
        {
            if (this.Map.RotateReference)
            {
                double latitudeOffset = latLonCoordinates.Latitude - latLonCoordinates.Latitude.FixCoordinate(true);
                double longitudeOffset = latLonCoordinates.Longitude - latLonCoordinates.Longitude.FixCoordinate(false);
                if (latitudeOffset != 0 || longitudeOffset != 0)
                {
                    latLonCoordinates = new LatLonCoordinates()
                    {
                        Latitude = latLonCoordinates.Latitude - latitudeOffset,
                        Longitude = latLonCoordinates.Longitude - longitudeOffset
                    };
                }
                latLonCoordinates = this.Map.Boundaries.Center.GetNewCoordinates(latLonCoordinates);
                if (latitudeOffset != 0 || longitudeOffset != 0)
                {
                    latLonCoordinates.Latitude += latitudeOffset;
                    latLonCoordinates.Longitude += longitudeOffset;
                }
                return new PixelCoordinates()
                {
                    X = this.Map.Size.Width * (latLonCoordinates.Longitude + this.Map.Boundaries.LongitudeHalfSpan) / this.Map.Boundaries.LongitudeSpan,
                    Y = this.Map.Size.Height * (this.Map.Boundaries.LatitudeHalfSpan - latLonCoordinates.Latitude) / this.Map.Boundaries.LatitudeSpan
                };
            }
            else
            {
                return new PixelCoordinates()
                {
                    X = this.Map.Size.Width * (latLonCoordinates.Longitude - this.Map.Boundaries.LeftNotBound) / this.Map.Boundaries.LongitudeSpan,
                    Y = this.Map.Size.Height * (this.Map.Boundaries.TopNotBound - latLonCoordinates.Latitude) / this.Map.Boundaries.LatitudeSpan
                };
            }
        }

        public virtual LatLonCoordinates PixelToLatLon(PixelCoordinates pixelCoordinates)
        {
            if (this.Map.RotateReference)
            {
                var result = new LatLonCoordinates()
                {
                    Longitude = pixelCoordinates.X * this.Map.Boundaries.LongitudeSpan / this.Map.Size.Width - this.Map.Boundaries.LongitudeHalfSpan,
                    Latitude = this.Map.Boundaries.LatitudeHalfSpan - pixelCoordinates.Y * this.Map.Boundaries.LatitudeSpan / this.Map.Size.Height
                };
                result = this.Map.Boundaries.Center.GetNewCoordinates(new LatLonCoordinates() { Latitude = 0, Longitude = 0 }).GetNewCoordinates(result);
                return result;
            }
            else
            {
                return new LatLonCoordinates()
                {
                    Longitude = this.Map.Boundaries.LeftNotBound + pixelCoordinates.X * this.Map.Boundaries.LongitudeSpan / this.Map.Size.Width,
                    Latitude = this.Map.Boundaries.TopNotBound - pixelCoordinates.Y * this.Map.Boundaries.LatitudeSpan / this.Map.Size.Height
                };
            }
        }

        public virtual LatLonCoordinates GetLatLonCenterForZoom(PixelCoordinates fixedPoint, double newLonSpan, double newLatSpan, double lonFactor, double latFactor)
        {
            var xFactor = fixedPoint.X / this.Map.Size.Width;
            var yFactor = fixedPoint.Y / this.Map.Size.Height;
            var latLonFixedPoint = this.PixelToLatLon(fixedPoint);
            return new LatLonCoordinates()
            {
                Latitude = latLonFixedPoint.Latitude + newLatSpan * (yFactor - .5d),
                Longitude = latLonFixedPoint.Longitude + newLonSpan * (.5d - xFactor)
            };
        }

        public virtual LatLonBoundaries BoundMap(double centerLatitude, double centerLongitude, double latitudeSpan, double longitudeSpan)
        {
            latitudeSpan = Math.Min(Math.Abs(latitudeSpan), LatLonBoundaries.MaxLatitudeSpan);
            longitudeSpan = Math.Min(Math.Abs(longitudeSpan), LatLonBoundaries.MaxLongitudeSpan);

            if (this.Map.RotateReference)
            {
                centerLatitude = centerLatitude.FixCoordinate(true);
                centerLongitude = centerLongitude.FixCoordinate(false);
            }
            else
            {
                // Bounds the map to the traditional map (can't cross poles / can't cross antimeridian)
                var halfLatSpan = latitudeSpan / 2;
                if (centerLatitude + halfLatSpan > 90)
                {
                    centerLatitude = 90 - halfLatSpan;
                }
                else if (centerLatitude - halfLatSpan < -90)
                {
                    centerLatitude = halfLatSpan - 90;
                }

                var halfLonSpan = longitudeSpan / 2;
                if (centerLongitude + halfLonSpan > 180)
                {
                    centerLongitude = 180 - halfLonSpan;
                }
                else if (centerLongitude - halfLonSpan < -180)
                {
                    centerLongitude = halfLonSpan - 180;
                }
            }
            
            return new LatLonBoundaries(centerLatitude, centerLongitude, latitudeSpan, longitudeSpan);
        }

        public virtual bool IsInMap(LatLonBoundaries rectangle)
        {
            LatLonBoundaries mapBoundaries = this.Map.Boundaries;
            if (this.Map.RotateReference)
            {
                var rectangleCenter = mapBoundaries.Center.GetNewCoordinates(rectangle.Center);
                rectangle = new LatLonBoundaries(rectangleCenter.Latitude, rectangleCenter.Longitude, rectangle.LatitudeSpan, rectangle.LongitudeSpan);
                mapBoundaries = new LatLonBoundaries(0, 0, mapBoundaries.LatitudeSpan, mapBoundaries.LongitudeSpan);
            }
            return rectangle.Intersects(mapBoundaries);
        }

        #endregion
    }
}
