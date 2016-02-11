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
        #region Fields

        protected LatLonBoundaries _cacheBoundaries = null;
        protected PixelSize _cacheSize = null;
        protected double _cacheYMin = double.NaN;
        protected double _cacheYMax = double.NaN;
        protected double _cacheYFactor = double.NaN;
        protected double _cacheR = double.NaN;

        #endregion

        #region AMercator

        public override double LongitudeToX(double longitude)
        {
            return getR() * (longitude - this.MapBoundaries.LonMin);
        }

        public override double LatitudeToY(double latitude)
        {
            return (latitudeToY(latitude) - getYMin()) * _cacheYFactor;
        }

        public override double XToLongitude(double x)
        {
            return this.MapBoundaries.LonMin + radiansToDegrees(x) / this.R;
        }

        public override double YToLatitude(double y)
        {
            return radiansToDegrees(2 * Math.Atan(Math.Exp((getYMin() + y) / this.R)) - HalfPi);
        }

        #endregion
        
        #region Tools

        private double latitudeToY(double latitude)
        {
            var phi = degreesToRadians(latitude);
            var sinPhi = Math.Sin(phi);
            return this.MapSize.Height * (.5d + Math.Log((1 + sinPhi) / (1 - sinPhi)) / FourPi);
        }

        private double getYMin()
        {
            updateCache();
            return _cacheYMin;
        }

        private double getR()
        {
            updateCache();
            return _cacheR;
        }

        protected virtual void updateCache()
        {
            if (this.MapBoundaries != _cacheBoundaries || this.MapSize != _cacheSize)
            {
                // Warning: This is not thread-safe!
                _cacheBoundaries = this.MapBoundaries;
                _cacheSize = this.MapSize;
                _cacheYMin = latitudeToY(_cacheBoundaries.LatMin);
                _cacheYMax = latitudeToY(_cacheBoundaries.LatMax);
                _cacheYFactor = _cacheSize.Height / (_cacheYMax - _cacheYMin);
                _cacheR = this.R / (this.MapBoundaries.LonMax - this.MapBoundaries.LonMin);
            }
        }

        #endregion
    }
}
