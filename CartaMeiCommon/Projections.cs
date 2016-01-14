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
        
        #endregion

        #region Functions

        Coordinates2D LatLonToPixel(Coordinates2D latLonCoordinates);

        Coordinates2D PixelToLatLon(Coordinates2D pixelCoordinates);

        // TODO: we probably need a function to draw a line as well (the line gets distorted with the projection)

        #endregion
    }
}
