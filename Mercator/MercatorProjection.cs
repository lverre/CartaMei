using CartaMei.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartaMei.Mercator
{
    public class MercatorProjection : IProjection
    {
        #region Constants

        internal const string ProjectionName = "Mercator";
        internal const string ProjectionDescription = "Mercator projection";

        internal const double DefaultTransverseScaleFactor = .9996d;

        private static readonly LatLonBoundaries _limitBoundaries = new LatLonBoundaries() { LatMin = -85, LatMax = 85, LonMin = double.MinValue, LonMax = double.MaxValue };

        #endregion

        #region Constructor

        public MercatorProjection()
        {
            this.UseEllipsoid = false;
            this.UseTransverse = false;
            this.TransverseScaleFactor = DefaultTransverseScaleFactor;
        }

        #endregion

        #region Properties

        private bool _useEllipsoid;
        public bool UseEllipsoid
        {
            get { return _useEllipsoid; }
            set
            {
                if (_useEllipsoid != value)
                {
                    _useEllipsoid = value;
                    updateFormulae();
                }
            }
        }

        private bool _useTransverse;
        public bool UseTransverse
        {
            get { return _useTransverse; }
            set
            {
                if (_useTransverse != value)
                {
                    _useTransverse = value;
                    updateFormulae();
                }
            }
        }

        private double _transverseScaleFactor;
        public double TransverseScaleFactor
        {
            get { return _transverseScaleFactor; }
            set
            {
                if (_transverseScaleFactor != value)
                {
                    _transverseScaleFactor = value;
                    updateFormulae();
                }
            }
        }

        #endregion

        #region IProjection

        public virtual string Name { get { return ProjectionName; } }

        public virtual LatLonBoundaries LimitBoundaries { get { return _limitBoundaries; } }

        private IMap _map;
        public virtual IMap Map
        {
            get { return _map; }
            set
            {
                if (_map != value)
                {
                    _map = value;
                    // TODO
                }
            }
        }

        public virtual PixelCoordinates LatLonToPixel(LatLonCoordinates latLonCoordinates)
        {
            var longitude = latLonCoordinates.Longitude;
            var latitude = latLonCoordinates.Latitude;
            // TODO: make sure they are "in phase"
            if (latitude == 90)
            {
                latitude -= double.Epsilon;
            }
            else if (latitude == -90)
            {
                latitude += double.Epsilon;
            }
            return new PixelCoordinates()
            {
                X = _formulae.LongitudeToX(longitude),
                Y = _formulae.LatitudeToY(latitude)
            };
        }

        public virtual LatLonCoordinates PixelToLatLon(PixelCoordinates pixelCoordinates)
        {
            return new LatLonCoordinates()
            {
                Longitude = _formulae.XToLongitude(pixelCoordinates.X),
                Latitude = _formulae.YToLatitude(pixelCoordinates.Y)
            };
        }

        #endregion

        #region Formulae

        protected AMercatorFormulae _formulae;

        protected virtual void updateFormulae()
        {
            // TODO: Web Mercator
            if (this.UseEllipsoid)
            {
                _formulae = this.UseTransverse ? new TransverseEllipsoidFormulae() { ScaleFactor = this.TransverseScaleFactor } : new EllipsoidFormulae();
            }
            else
            {
                _formulae = this.UseTransverse ? new TransverseSphericalFormulae() { ScaleFactor = this.TransverseScaleFactor } : new SphericalFormulae();
            }
            _formulae.Projection = this;
        }

        #endregion

        #region Object

        public override string ToString()
        {
            return this.Name;
        }

        #endregion
    }
}
