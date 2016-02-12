using CartaMei.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartaMei.GSHHG
{
    public static class Extensions
    {
        public static bool Intersects<TPolygonHeader, TPolygonPoint>(this IPolygon<TPolygonHeader, TPolygonPoint> item, LatLonBoundaries boundaries, IProjection projection)
            where TPolygonHeader : IPolygonHeader
            where TPolygonPoint : LatLonCoordinates
        {
            return item?.Header?.Boundaries?.Intersects(boundaries) ?? false;
        }
    }
}
