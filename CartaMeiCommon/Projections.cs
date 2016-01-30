using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartaMei.Common
{
    public interface IProjection : IHasName
    {
        #region Properties
        
        IMap Map { get; set; }

        LatLonBoundaries LimitBoundaries { get; }
        
        #endregion

        #region Functions

        PixelCoordinates LatLonToPixel(LatLonCoordinates latLonCoordinates);

        LatLonCoordinates PixelToLatLon(PixelCoordinates pixelCoordinates);

        // TODO: we probably need a function to draw a line as well (the line gets distorted with the projection)

        #endregion
    }
}
