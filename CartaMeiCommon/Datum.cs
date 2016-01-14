using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartaMei.Common
{
    public struct Datum : IHasName
    {
        #region Constants

        public static readonly Datum WGS84 = new Datum("WGS84", 6378137, 298.257223563);

        #endregion
        
        #region Constructor

        public Datum(string name, double majorRadius, double inverseFlattening)
        {
            this.Name = name;
            this.MajorRadius = majorRadius;
            this.InverseFlattening = inverseFlattening;
            this.Flattening = 1d / this.InverseFlattening;
            this.MinorRadius = 1d - this.Flattening;
            this.RadiiRatio = this.MinorRadius / this.MajorRadius;
            this.Excentricity = Math.Sqrt(1d - (this.RadiiRatio * this.RadiiRatio));
            this.HalfExcentricity = this.Excentricity / 2d;
        }

        #endregion

        #region IHasName

        /// <summary>
        /// Gets the name of the datum.
        /// </summary>
        public string Name { get; private set; }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the inverse flattening of the spheroid.
        /// </summary>
        /// <seealso cref="Flattening"/>
        public double InverseFlattening { get; private set; }
        
        /// <summary>
        /// Gets the flattening of the spheroid.
        /// </summary>
        /// <seealso cref="InverseFlattening"/>
        public double Flattening { get; private set; }

        /// <summary>
        /// Gets the major (equatorial) radius (in meters) of the spheroid.
        /// </summary>
        /// <seealso cref="MinorRadius"/>
        public double MajorRadius { get; private set; }

        /// <summary>
        /// Gets the minor (polar) radius (in meters) of the spheroid.
        /// </summary>
        /// <seealso cref="MajorRadius"/>
        public double MinorRadius { get; private set; }

        /// <summary>
        /// Gets the radii ratio of the spheroid.
        /// </summary>
        public double RadiiRatio { get; private set; }

        /// <summary>
        /// Gets the excentricity of the spheroid.
        /// </summary>
        public double Excentricity { get; private set; }
        
        /// <summary>
        /// Gets the excentricity of the spheroid.
        /// </summary>
        public double HalfExcentricity { get; private set; }

        #endregion

        #region Object, Operators

        public override string ToString()
        {
            return this.Name;
        }

        public override bool Equals(object obj)
        {
            return obj is Datum && this == (Datum)obj;
        }

        public override int GetHashCode()
        {
            return (int)(this.MajorRadius * this.InverseFlattening);
        }

        public static bool operator ==(Datum a, Datum b)
        {
            // Major radius and inverse flattening are enough (the other values can be calculated from them)
            return
                a.MajorRadius == b.MajorRadius &&
                a.InverseFlattening == b.InverseFlattening;
        }

        public static bool operator !=(Datum a, Datum b)
        {
            return !(a == b);
        }

        #endregion
    }
}
