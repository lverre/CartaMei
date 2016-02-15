namespace CartaMei.Common
{
    public interface IProjection : IHasName
    {
        #region Properties
        
        IMap Map { get; set; }
        
        #endregion

        #region Functions

        PixelCoordinates LatLonToPixel(LatLonCoordinates latLonCoordinates);

        LatLonCoordinates PixelToLatLon(PixelCoordinates pixelCoordinates);

        LatLonBoundaries BoundMap(double centerLatitude, double centerLongitude, double latitudeSpan, double longitudeSpan);

        // TODO: we need a function to draw a line as well (the line gets distorted with the projection)

        #endregion
    }
}
