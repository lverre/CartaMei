using System;
using System.Collections.Generic;

namespace CartaMei.GSHHG
{
    #region Constants

    public static class Constants
    {
        public const double Million = 1000000d;
    }

    #endregion

    #region Classes

    public class PolygonDatabase<THeader, TPoint> : IPolygonDatabase<THeader, TPoint>
        where THeader : IPolygonHeader
        where TPoint : IPolygonPoint
    {
        #region Constructors

        public PolygonDatabase(string fileName, PolygonType type, Resolution resolution, IDictionary<int, IPolygon<THeader, TPoint>> polygons)
        {
            this.FileName = fileName;
            this.PolygonType = type;
            this.Resolution = resolution;
            this.Polygons = polygons;
        }

        #endregion

        #region IPolygonDatabase

        public virtual PolygonType PolygonType { get; private set; }

        public virtual Resolution Resolution { get; private set; }

        public virtual IDictionary<int, IPolygon<THeader, TPoint>> Polygons { get; private set; }

        #endregion

        #region Properties

        public virtual string FileName { get; private set; }

        #endregion
    }
    
    public class Polygon<THeader, TPoint> : IPolygon<THeader, TPoint>
        where THeader : IPolygonHeader
        where TPoint : IPolygonPoint
    {
        #region IPolygon

        #region Properties

        public virtual THeader Header { get; set; }

        public virtual IEnumerable<TPoint> Points{ get; set; }

        public virtual IPolygon<THeader, TPoint> Parent { get; set; }

        public virtual IEnumerable<IPolygon<THeader, TPoint>> Children { get; set; }

        #endregion

        #endregion

        #region Object

        public override int GetHashCode()
        {
            return this.Header.Id;
        }

        public override string ToString()
        {
            return this.ToString(false, false);
        }

        #endregion
    }

    public class Polygon<THeader> : Polygon<THeader, PolygonPoint>, IPolygon<THeader> where THeader : IPolygonHeader { }

    public class PolygonPoint : IPolygonPoint
    {
        #region Properties

        /// <summary>
        /// Gets or sets the x (longitude) coordinate, in degrees, of the point.
        /// </summary>
        public virtual double X { get; set; }

        /// <summary>
        /// Gets or sets the y (latitude) coordinate, in degrees, of the point.
        /// </summary>
        public virtual double Y { get; set; }

        #endregion

        #region Object

        /// <summary>
        /// Gets a string representation of this object: X ºE/W Y ºN/S.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString()
        {
            return 
                Math.Abs(this.X) + " º" + (this.X >= 0 ? "W" : "E") + " " +
                Math.Abs(this.Y) + " º" + (this.Y >= 0 ? "N" : "S");
        }

        #endregion
    }

    public class CoordinatesRectangle
    {
        #region Constructor
        
        /// <summary>
        /// Creates a coordinates rectangle object.
        /// </summary>
        /// <param name="east">The east limit coordinates, in micro-degrees.</param>
        /// <param name="west">The west limit coordinates, in micro-degrees.</param>
        /// <param name="south">The south limit coordinates, in micro-degrees.</param>
        /// <param name="north">The north limit coordinates, in micro-degrees.</param>
        public CoordinatesRectangle(int east, int west, int south, int north)
            : this(east / Constants.Million, west / Constants.Million, south / Constants.Million, north / Constants.Million)
        { }

        /// <summary>
        /// Creates a limits object.
        /// </summary>
        /// <param name="east">The east limit coordinates, in degrees.</param>
        /// <param name="west">The west limit coordinates, in degrees.</param>
        /// <param name="south">The south limit coordinates, in degrees.</param>
        /// <param name="north">The north limit coordinates, in degrees.</param>
        public CoordinatesRectangle(double east, double west, double south, double north)
        {
            this.East = fixCoordinate(east, 180);
            this.West = fixCoordinate(west, 180);
            this.North = fixCoordinate(north, 90);
            this.South = fixCoordinate(south, 90);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the east limit coordinates, in degrees.
        /// </summary>
        public virtual double East { get; private set; }

        /// <summary>
        /// Gets the west limit coordinates, in degrees.
        /// </summary>
        public virtual double West { get; private set; }

        /// <summary>
        /// Gets the south limit coordinates, in degrees.
        /// </summary>
        public virtual double South { get; private set; }

        /// <summary>
        /// Gets the north limit coordinates, in degrees.
        /// </summary>
        public virtual double North { get; private set; }

        #endregion

        #region Functions

        /// <summary>
        /// Checks whether a point is within these limits.
        /// </summary>
        /// <param name="point">The point to test.</param>
        /// <returns>true if <paramref name="point"/> is within these limits, false otherwise.</returns>
        public bool Contains(PolygonPoint point)
        {
            return
                isBetween(point.X, this.West, this.East) &&
                isBetween(point.Y, this.South, this.North);
        }

        /// <summary>
        /// Checks whether a rectangle intersects with this one..
        /// </summary>
        /// <param name="rectangle">The rectangle to test.</param>
        /// <returns>true if <paramref name="rectangle"/> intersects with this one, false otherwise.</returns>
        public bool Intersects(CoordinatesRectangle rectangle)
        {
            return
                isBetween(rectangle.East, this.West, this.East) ||
                isBetween(rectangle.West, this.West, this.East) ||
                isBetween(rectangle.South, this.South, this.North) ||
                isBetween(rectangle.North, this.South, this.North);
        }

        private static bool isBetween(double x, double min, double max)
        {
            return x >= min && x <= max;
        }

        #endregion

        #region Object

        /// <summary>
        /// Gets a string representation of this object: show NW / SE.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString()
        {
            return 
                new PolygonPoint() { X = this.West, Y = this.North }.ToString() + " / " +
                new PolygonPoint() { X = this.East, Y = this.South }.ToString();
        }

        #endregion

        #region Tools

        private static double fixCoordinate(double coordinate, int max)
        {
            if (coordinate > max)
            {
                coordinate = (coordinate % (max * 2)) - (max * 2);
            }
            else if (coordinate < (0 - max))
            {
                coordinate = (coordinate % (max * 2)) + (max * 2);
            }
            return coordinate;
        }

        #endregion
    }

    #endregion

    #region Enums
    
    [Flags]
    public enum GSHHGFlag : uint
    {
        None =                  0x00000000,
        Land =                  0x00000001,
        Lake =                  0x00000002,
        IslandInLake =          0x00000003,
        PondInIslandInLake =    0x00000004,
        Bit8 =                  0x00000008,
        Bit10 =                 0x00000010,
        Bit20 =                 0x00000020,
        Bit40 =                 0x00000040,
        Bit80 =                 0x00000080,
        Bit100 =                0x00000100,
        Bit200 =                0x00000200,
        Bit400 =                0x00000400,
        Bit800 =                0x00000800,
        Bit1000 =               0x00001000,
        Bit2000 =               0x00002000,
        Bit4000 =               0x00004000,
        Bit8000 =               0x00008000,
        GreenwichIsCrossed =    0x00010000,
        Bit20000 =              0x00020000,
        Bit40000 =              0x00040000,
        Bit80000 =              0x00080000,
        Bit100000 =             0x00100000,
        Bit200000 =             0x00200000,
        Bit400000 =             0x00400000,
        Bit800000 =             0x00800000,
        SourceIsWVS =           0x01000000,
        RiverLake =             0x02000000,
        RiverLevel =            0x04000000,
        Bit8000000 =            0x08000000,
        Bit10000000 =           0x10000000,
        Bit20000000 =           0x20000000,
        Bit40000000 =           0x40000000,
        Bit80000000 =           0x80000000
    }
    
    /// <summary>
    /// Describes the possible sources of GSHHG data.
    /// </summary>
    public enum GSHHGSource
    {
        /// <summary>
        /// CIA WDBII.
        /// </summary>
        WDBII,
        /// <summary>
        /// WVS.
        /// </summary>
        WVS,
        /// <summary>
        /// AC.
        /// </summary>
        AC
    }

    /// <summary>
    /// Describes the possible types of lines a polygon represents.
    /// </summary>
    public enum PolygonType
    {
        /// <summary>
        /// The polygon represents a shore line.
        /// </summary>
        ShoreLine,
        /// <summary>
        /// The polygon represents a country border.
        /// </summary>
        Border,
        /// <summary>
        /// The polygon represents a river.
        /// </summary>
        River
    }

    /// <summary>
    /// Describes the possible resolutions of a GSHHG file.
    /// </summary>
    public enum Resolution
    {
        /// <summary>
        /// Full (very high) resolution.
        /// </summary>
        Full = 0,
        /// <summary>
        /// High resolution.
        /// </summary>
        High = 1,
        /// <summary>
        /// Intermediate resolution.
        /// </summary>
        Intermediate = 2,
        /// <summary>
        /// Low resolution.
        /// </summary>
        Low = 3,
        /// <summary>
        /// Crude (very low) resolution.
        /// </summary>
        Crude = 4
    }

    /// <summary>
    /// Describes the possible things a polygon encloses.
    /// </summary>
    public enum Level
    {
        /// <summary>
        /// Unknown polygon level.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// The polygon represents land (in a continent or in a sea island).
        /// </summary>
        Land = 1,
        /// <summary>
        /// The polygon represents a lake.
        /// </summary>
        Lake = 2,
        /// <summary>
        /// The polygon represents an island in a lake.
        /// </summary>
        IslandInLake = 3,
        /// <summary>
        /// The polygon represents a pond in an island in a lake.
        /// </summary>
        PondInIslandInLake = 4,
        /// <summary>
        /// The polygon represents the Antarctic ice front.
        /// </summary>
        AntarcticIceFront = 5,
        /// <summary>
        /// The polygon represents the Antarctic grounding line.
        /// </summary>
        AntarcticGroundingLine = 6
    }

    /// <summary>
    /// Describes the Earth Continents.
    /// </summary>
    [Flags]
    public enum EarthContinent
    {
        /// <summary>
        /// Unknown continent.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Africa.
        /// </summary>
        Africa = 1,
        /// <summary>
        /// Central America.
        /// </summary>
        AmericaCentral = 2,
        /// <summary>
        /// North America.
        /// </summary>
        AmericaNorth = 4,
        /// <summary>
        /// South America.
        /// </summary>
        AmericaSouth = 8,
        /// <summary>
        /// America (north, central and south).
        /// </summary>
        America = 14,
        /// <summary>
        /// Antartica.
        /// </summary>
        Antarctica = 16,
        /// <summary>
        /// Asia.
        /// </summary>
        Asia = 32,
        /// <summary>
        /// Australia.
        /// </summary>
        Australia = 64,
        /// <summary>
        /// Europe.
        /// </summary>
        Europe = 128,
        /// <summary>
        /// Eurasia (Europe and Asia).
        /// </summary>
        Eurasia = 160
    }

    #endregion
}
