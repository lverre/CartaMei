using CartaMei.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        [Browsable(false)]
        public virtual bool SupportsReferenceChange
        {
            get { return false; }
        }

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

        public virtual LatLonCoordinates GetLatLonCenterForZoom(PixelCoordinates fixedPoint, double newLonSpan, double newLatSpan, double lonFactor, double latFactor)
        {
            // TODO: fix latitude
            var xFactor = fixedPoint.X / this.Map.Size.Width;
            var yFactor = fixedPoint.Y / this.Map.Size.Height;
            var latLonFixedPoint = this.PixelToLatLon(fixedPoint);
            return new LatLonCoordinates()
            {
                Latitude = latLonFixedPoint.Latitude + newLatSpan * (yFactor - .5d),
                Longitude = latLonFixedPoint.Longitude + newLonSpan * (.5d - xFactor)
            };
        }

        // Bounds the map to the traditional map (can't cross poles / can't cross antimeridian)
        public virtual LatLonBoundaries BoundMap(double centerLatitude, double centerLongitude, double latitudeSpan, double longitudeSpan)
        {
            latitudeSpan = Math.Min(Math.Abs(latitudeSpan), LatLonBoundaries.MaxLatitudeSpan);
            longitudeSpan = Math.Min(Math.Abs(longitudeSpan), LatLonBoundaries.MaxLatitudeSpan);

            var halfLatSpan = latitudeSpan / 2;
            if (centerLatitude + halfLatSpan > 90)
            {
                centerLatitude = 90 - halfLatSpan;
            }
            else if (centerLatitude - halfLatSpan < -90)
            {
                centerLatitude = halfLatSpan - 90;
            }

            var halfLonSpan = longitudeSpan / 2;
            if (centerLongitude + halfLonSpan > 180)
            {
                centerLongitude = 180 - halfLonSpan;
            }
            else if (centerLongitude - halfLonSpan < -180)
            {
                centerLongitude = halfLonSpan - 180;
            }

            return new LatLonBoundaries(centerLatitude, centerLongitude, latitudeSpan, longitudeSpan);
        }

        public virtual bool IsInMap(LatLonBoundaries rectangle)
        {
            return rectangle.Intersects(this.Map.Boundaries);
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
