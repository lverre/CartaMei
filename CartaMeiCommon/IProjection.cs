using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartaMei.Common
{
    public interface IProjection
    {
        #region Properties

        string Name { get; }

        #endregion

        #region Functions

        double GetPixelCoordinates(double lat, double lon);

        // TODO: we probably need a function to draw a line as well (the line gets distorted with the projection)

        #endregion
    }
}
