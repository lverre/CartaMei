using CartaMei.Common;

namespace CartaMei.MainPlugin
{
    class EquirectangularProjection : IProjection
    {
        internal const string ProjectionName = "Equirectangular Projection";
        internal const string ProjectionDescription = "The simplest projection there can be: coordinates are just scaled to fit in the canvas. That means the areas far from the equator and the reference longitude appear much bigger than they are in reality.";

        #region IProjection

        public LatLonBoundaries LimitBoundaries
        {
            get { return null; }
        }

        public IMap Map { get; set; }

        public string Name
        {
            get { return ProjectionName; }
        }

        public PixelCoordinates LatLonToPixel(LatLonCoordinates latLonCoordinates)
        {
            return new PixelCoordinates()
            {
                X = this.Map.Size.Width * (latLonCoordinates.SafeLongitude - this.Map.Boundaries.LonMin) / (this.Map.Boundaries.LonMax - this.Map.Boundaries.LonMin),
                Y = this.Map.Size.Height * (this.Map.Boundaries.LatMax - latLonCoordinates.Latitude) / (this.Map.Boundaries.LatMax - this.Map.Boundaries.LatMin)
            };
        }

        public LatLonCoordinates PixelToLatLon(PixelCoordinates pixelCoordinates)
        {
            return new LatLonCoordinates()
            {
                Longitude = this.Map.Boundaries.LonMin + pixelCoordinates.X * (this.Map.Boundaries.LonMax - this.Map.Boundaries.LonMin) / this.Map.Size.Width,
                Latitude = this.Map.Boundaries.LatMax - pixelCoordinates.Y * (this.Map.Boundaries.LatMax - this.Map.Boundaries.LatMin) / this.Map.Size.Height
            };
        }

        #endregion
    }
}
