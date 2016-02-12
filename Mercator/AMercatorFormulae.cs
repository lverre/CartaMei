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

        protected const double DegToRad = Math.PI / 180d;
        protected const double RadToDeg = 180d / Math.PI;
        protected const double TwoPi = Math.PI * 2d;
        protected const double FourPi = Math.PI * 4d;
        protected const double HalfPi = Math.PI / 2d;
        protected const double QuarterPi = Math.PI / 4d;

        #endregion

        #region Abstract

        /// <summary>
        /// Converts the longitude to the x coordinate (in meters).
        /// </summary>
        /// <param name="longitude">The longitude to convert.</param>
        /// <returns>The x coordinate (in meters) corresponding to <paramref name="longitude"/>.</returns>
        public abstract double LongitudeToX(double longitude);

        /// <summary>
        /// Converts the latitude to the y coordinate (in meters).
        /// </summary>
        /// <param name="latitude">The latitude to convert.</param>
        /// <returns>The y coordinate (in meters) corresponding to <paramref name="latitude"/>.</returns>
        public abstract double LatitudeToY(double latitude);

        /// <summary>
        /// Converts x coordinate (in meters) to the longitude.
        /// </summary>
        /// <param name="x">The x coordinate to convert.</param>
        /// <returns>The longitude corresponding to <paramref name="x"/>.</returns>
        public abstract double XToLongitude(double x);

        /// <summary>
        /// Converts y coordinate (in meters) to the latitude.
        /// </summary>
        /// <param name="y">The y coordinate to convert.</param>
        /// <returns>The latitude corresponding to <paramref name="y"/>.</returns>
        public abstract double YToLatitude(double y);

        #endregion

        #region Properties

        public MercatorProjection Projection { get; set; }

        #endregion

        #region Tools

        protected virtual Datum Datum
        {
            get { return this.Projection.Map.Datum; }
        }

        protected virtual double R
        {
            get { return this.MapSize.Width; }
        }

        protected virtual LatLonBoundaries MapBoundaries
        {
            get { return this.Projection.Map.Boundaries; }
        }

        protected virtual PixelSize MapSize
        {
            get { return this.Projection.Map.Size; }
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
