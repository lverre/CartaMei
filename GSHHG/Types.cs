using CartaMei.Common;
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
        where TPoint : LatLonCoordinates
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
        where TPoint : LatLonCoordinates
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

    public class Polygon<THeader> : Polygon<THeader, LatLonCoordinates>, IPolygon<THeader> where THeader : IPolygonHeader { }
    
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
