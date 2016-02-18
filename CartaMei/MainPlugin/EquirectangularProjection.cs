using CartaMei.Common;
using System;

namespace CartaMei.MainPlugin
{
    class EquirectangularProjection : IProjection
    {
        internal const string ProjectionName = "Equirectangular Projection";
        internal const string ProjectionDescription = "The simplest projection there can be: coordinates are just scaled to fit in the canvas. That means the areas far from the equator and the reference longitude appear much bigger than they are in reality.";

        #region IProjection

        public virtual IMap Map { get; set; }

        public virtual string Name
        {
            get { return ProjectionName; }
        }

        public virtual PixelCoordinates LatLonToPixel(LatLonCoordinates latLonCoordinates)
        {
            return new PixelCoordinates()
            {
                X = this.Map.Size.Width * (latLonCoordinates.SafeLongitude - this.Map.Boundaries.LeftNotBound) / this.Map.Boundaries.LongitudeSpan,
                Y = this.Map.Size.Height * (this.Map.Boundaries.TopNotBound - latLonCoordinates.Latitude) / this.Map.Boundaries.LatitudeSpan
            };
        }

        public virtual LatLonCoordinates PixelToLatLon(PixelCoordinates pixelCoordinates)
        {
            return new LatLonCoordinates()
            {
                Longitude = this.Map.Boundaries.LeftNotBound + pixelCoordinates.X * this.Map.Boundaries.LongitudeSpan / this.Map.Size.Width,
                Latitude = this.Map.Boundaries.TopNotBound - pixelCoordinates.Y * this.Map.Boundaries.LatitudeSpan / this.Map.Size.Height
            };
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

        // Bounds the map to the traditional map (can't cross poles / can't cross antimeridian)
        public virtual LatLonBoundaries BoundMap(double centerLatitude, double centerLongitude, double latitudeSpan, double longitudeSpan)
        {
            latitudeSpan = Math.Min(Math.Abs(latitudeSpan), LatLonBoundaries.MaxLatitudeSpan);
            longitudeSpan = Math.Min(Math.Abs(longitudeSpan), LatLonBoundaries.MaxLongitudeSpan);

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

            return new LatLonBoundaries(centerLatitude, centerLongitude, latitudeSpan, longitudeSpan);
        }

        #endregion
    }
}
