using CartaMei.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartaMei.Mercator
{
    public abstract class AMercatorFormulae
    {
        #region Constants

        protected const double DegToRad = Math.PI / 180.0;
        protected const double RadToDeg = 180.0 / Math.PI;
        protected const double HalfPi = Math.PI / 2d;
        protected const double QuarterPi = Math.PI / 4d;

        #endregion

        #region Abstract

        public abstract double LongitudeToX(double longitude);

        public abstract double LatitudeToY(double latitude);

        public abstract double XToLongitude(double x);

        public abstract double YToLatitude(double y);

        #endregion

        #region Properties

        public MercatorProjection Projection { get; set; }

        #endregion

        #region Tools

        protected Datum Datum
        {
            get { return this.Projection.Map.Datum; }
        }

        protected static double radiansToDegrees(double rad)
        {
            return rad * RadToDeg;
        }

        protected static double degreesToRadians(double deg)
        {
            return deg * DegToRad;
        }

        #endregion
    }
}
