﻿using System.Windows;

namespace CartaMei.Common
{
    public interface IProjection : IHasName
    {
        #region Properties
        
        IMap Map { get; set; }

        bool SupportsReferenceChange { get; }

        bool CanUseTransformForTranslate { get; }

        #endregion

        #region Functions

        PixelCoordinates LatLonToPixel(LatLonCoordinates latLonCoordinates);

        LatLonCoordinates PixelToLatLon(PixelCoordinates pixelCoordinates);

        LatLonCoordinates GetLatLonCenterForZoom(PixelCoordinates fixedPoint, double newLonSpan, double newLatSpan, double lonFactor, double latFactor);

        LatLonBoundaries BoundMap(double centerLatitude, double centerLongitude, double latitudeSpan, double longitudeSpan);

        bool IsInMap(LatLonBoundaries rectangle);

        // TODO: we need a function to draw a line as well (the line gets distorted with the projection)

        #endregion
    }
}
