using CartaMei.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartaMei.Mercator
{
    public class EllipsoidFormulae : AMercatorFormulae
    {
        #region Constants
        
        private const int MaxInverseLatitudeSteps = 15;
        private const double MinInverseLatitudeResolution = 0.000000001d;

        #endregion

        #region AMercatorFormulae

        public override double LongitudeToX(double longitude)
        {
            return degreesToRadians(longitude) * this.Datum.MajorRadius;
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

            var phi = degreesToRadians(latitude);
                var eSinPhi = this.Datum.Excentricity * Math.Sin(phi);
                return 
                    this.Datum.MajorRadius * 
                    Math.Log(
                        Math.Tan(QuarterPi + phi / 2d) * 
                        Math.Pow((1d - eSinPhi) / (1d + eSinPhi), this.Datum.HalfExcentricity)
                        );
        }

        public override double XToLongitude(double x)
        {
            return radiansToDegrees(x) / this.Datum.MajorRadius;
        }

        public override double YToLatitude(double y)
        {
            double ts = Math.Exp((0 - y) / this.Datum.MajorRadius);
            double phi = HalfPi - 2 * Math.Atan(ts);
            double deltaPhi = 1d;
            for (int i = 0; Math.Abs(deltaPhi) > MinInverseLatitudeResolution && i < MaxInverseLatitudeSteps; i++)
            {
                var eSinPhi = this.Datum.Excentricity * Math.Sin(phi);
                var newPhi = HalfPi - 2 * Math.Atan(ts * Math.Pow((1d - eSinPhi) / (1d + eSinPhi), this.Datum.HalfExcentricity));
                deltaPhi = newPhi - phi;
                phi = newPhi;
            }
            return radiansToDegrees(phi);
        }
        
        #endregion
    }
}
