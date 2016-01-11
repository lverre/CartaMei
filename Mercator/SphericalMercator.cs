using CartaMei.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartaMei.Mercator
{
    public class SphericalFormulae : AMercatorFormulae
    {
        #region Constants

        internal const string ProjectionName = "Mercator (simplified)";
        internal const string ProjectionDescription = "Mercator projection (using the spherical formulae)";

        #endregion

        #region AMercator
        
        public override double LongitudeToX(double longitude)
        {
            return this.Datum.MajorRadius * degreesToRadians(longitude);
        }

        public override double LatitudeToY(double latitude)
        {
            if (latitude == 90)
            {
                latitude -= double.Epsilon;
            }
            else if (latitude == -90)
            {
                latitude += double.Epsilon;
            }

            var watch = System.Diagnostics.Stopwatch.StartNew();
            var phi = degreesToRadians(latitude);
            var sinPhi = Math.Sin(phi);
            var result = this.Datum.MajorRadius * Math.Log((1d + sinPhi) / (1d - sinPhi)) / 2;
            var res1Time = watch.ElapsedMilliseconds;
            watch.Restart();
            var result2 = this.Datum.MajorRadius / Math.Sinh(Math.Tan(phi));
            var res2Time = watch.ElapsedMilliseconds;
            return result;
        }

        public override double XToLongitude(double x)
        {
            return radiansToDegrees(x) / this.Datum.MajorRadius;
        }

        public override double YToLatitude(double y)
        {
            return radiansToDegrees(2 / Math.Tan(Math.Exp(y / this.Datum.MajorRadius)) - HalfPi);
        }

        #endregion
    }
}
