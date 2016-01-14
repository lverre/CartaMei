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

        #endregion

        #region IProjection

        public string Name { get { return ProjectionName; } }

        private IMap _map;
        public IMap Map
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

        public virtual Coordinates2D LatLonToPixel(Coordinates2D latLonCoordinates)
        {
            // TODO: bound values, call the abstract functions
            return new Coordinates2D()
            {
                X = 0,
                Y = 0
            };
        }

        public virtual Coordinates2D PixelToLatLon(Coordinates2D pixelCoordinates)
        {
            // TODO: bound values, call the abstract functions
            return new Coordinates2D()
            {
                X = 0,
                Y = 0
            };
        }

        #endregion

        #region Formulae

        private AMercatorFormulae _formulae;

        private void updateFormulae()
        {
            // TODO: Transverse Mercator, Web Mercator
            if (this.UseEllipsoid)
            {
                _formulae = new EllipsoidFormulae();
            }
            else
            {
                _formulae = new SphericalFormulae();
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
