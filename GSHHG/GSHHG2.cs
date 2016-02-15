using CartaMei.Common;
using System;
using System.IO;

namespace CartaMei.GSHHG
{
    public class GSHHG2Version : IGSHHGVersion<GSHHG2PolygonHeader>
    {
        #region Static Fields

        private static readonly IGSHHGReader<GSHHG2PolygonHeader> _reader = new GSHHG2Reader();

        #endregion

        #region IGSHHGVersion

        #region Properties

        public int Id { get { return 9; } }

        public string Version { get { return "2.0"; } }

        public string ReleaseDate { get { return "2009-07-15"; } }

        public string Description
        {
            get
            {
                return @"Version 2.0 July 15, 2009: Differs from the previous version 1.x in
the following ways.

1.  Free from internal and external crossings and erratic spikes
    at all five resolutions.
2.  The original Eurasiafrica polygon has been split into Eurasia
    (polygon # 0) and Africa (polygon # 1) along the Suez canal.
3.  The original Americas polygon has now been split into North
    America (polygon # 2) and South America (polygon # 3) along
    the Panama canal.
4.  Antarctica is now polygon # 4 and Australia is polygon # 5, in
    all the five resolutions.
5.  Fixed numerous problems, including missing islands and lakes
    in the Amazon and Nile deltas.
6.  Flagged ""riverlakes"" which are the fat part of major rivers so
    they may easily be identified by users.
7.  Determined container ID for all polygons (== -1 for level 1
    polygons) which is the ID of the polygon that contains a smaller
    polygon.
8.  Determined full-resolution ancestor ID for lower res polygons,
    i.e., the ID of the polygon that was reduced to yield the lower-
    res version.
9.  Ensured consistency across resolutions (i.e., a feature that is
    an island at full resolution should not become a lake in low!).
10. Sorted tables on level, then on the area of each feature.
11. Made sure no feature is missing in one resolution but
    present in the next lower resolution.
12. Store both the actual area of the lower-res polygons and the
    area of the full-resolution ancestor so users may exclude fea-
    tures that represent less that a fraction of the original full
    area.

There was some duplication and wrong levels assigned to maritime
political boundaries in the Persian Gulf that has been fixed.

These changes required us to enhance the GSHHG C-structure used to
read and write the data.";
            }
        }

        public IGSHHGReader<GSHHG2PolygonHeader, LatLonCoordinates> Reader { get { return _reader; } }

        #endregion

        #endregion

        #region Functions
        
        public bool IsCompatibleWith(int version)
        {
            return version >= this.Id;// this is the latest version as far as the data structure is concerned (as of 2016-01-01).
        }

        #endregion
    }

    public class GSHHG2Reader : AGSHHGReader<GSHHG2PolygonHeader>
    {
        #region Constants

        private const int IdEurasia = 0;
        private const int IdAfrica = 1;
        private const int IdNorthAmerica = 2;
        private const int IdSouthAmerica = 3;
        private const int IdAntarctica = 4;
        private const int IdAustralia = 5;
        
        public const int StructSizePoint = sizeof(Int32) * 2;
        public const int StructSizeHeader = sizeof(Int32) * 11;

        #endregion

        #region AGSHHGReader

        public override int HeaderSize { get { return StructSizeHeader; } }

        protected override GSHHG2PolygonHeader readPolygonHeader(BinaryReader reader)
        {
            Int32[] elements = new int[StructSizeHeader / sizeof(Int32)];
            int index;
            for (index = 0; index < elements.Length && reader.BaseStream.Position != reader.BaseStream.Length; index++)
            {
                elements[index] = reader.ReadInt32BE();
            }

            if (index < elements.Length)
            {
                throw new InvalidDataException();
            }
            else
            {
                return GSHHG2PolygonHeader.Create(elements[0], elements[1], elements[2], elements[3], elements[4], elements[5], elements[6], elements[7], elements[8], elements[9], elements[10]);
            }
        }

        #endregion

        #region Tools

        private static EarthContinent getContinent(int id)
        {
            switch (id)
            {
                case IdAfrica:
                    return EarthContinent.Africa;
                case IdAntarctica:
                    return EarthContinent.Antarctica;
                case IdAustralia:
                    return EarthContinent.Australia;
                case IdEurasia:
                    return EarthContinent.Eurasia;
                case IdNorthAmerica:
                    return EarthContinent.AmericaCentral | EarthContinent.AmericaNorth;
                case IdSouthAmerica:
                    return EarthContinent.AmericaSouth;
                default:
                    return EarthContinent.Unknown;
            }
        }

        #endregion
    }

    public class GSHHG2PolygonHeader : IPolygonHeader
    {
        #region Fields

        private int id;         /* unique polygon id number, starting at 0 */
        private int n;          /* Number of points in this polygon */
        private int flag;       /* = level + version << 8 + greenwich << 16 + source << 24 + river << 25 */
        /* flag contains 5 items, as follows:
         * low byte:    level = flag & 255: Values: 1 land, 2 lake, 3 island_in_lake, 4 pond_in_island_in_lake
         * 2nd byte:    version = (flag >> 8) & 255: Values: Should be 12 for GSHHG release 12 (i.e., version 2.2)
         * 3rd byte:    greenwich = (flag >> 16) & 1: Values: Greenwich is 1 if Greenwich is crossed
         * 4th byte:    source = (flag >> 24) & 1: Values: 0 = CIA WDBII, 1 = WVS
         * 4th byte:    river = (flag >> 25) & 1: Values: 0 = not set, 1 = river-lake and level = 2
         */
        private int west;    /* min/max extent in micro-degrees */
        private int east;
        private int south;
        private int north;
        private int area;       /* Area of polygon in 1/10 km^2 */
        private int area_full;  /* Area of original full-resolution polygon in 1/10 km^2 */
        private int container;  /* Id of container polygon that encloses this polygon (-1 if none) */
        private int ancestor;   /* Id of ancestor polygon in the full resolution set that was the source of this polygon (-1 if none) */

        #endregion

        #region IPolygonHeader
        
        public int Id { get { return this.id; } }

        public int ContainerId { get { return this.container; } }
        
        public LatLonBoundaries Boundaries { get; private set; }

        public int PointsCount { get { return this.n; } }

        #endregion

        #region Properties

        #region Ids

        public int AncestorId { get { return this.ancestor; } }

        public bool HasAncestor { get { return this.ancestor != -1; } }

        #endregion

        #region Flags

        public int Flag { get { return this.flag; } }

        public GSHHGFlag Flags { get { return (GSHHGFlag)((uint)this.flag); } }

        public bool IsLand { get { return this.Flags.HasFlag(GSHHGFlag.Land); } }

        public bool IsWater { get { return !this.Flags.HasFlag(GSHHGFlag.Land); } }

        public bool IsSea { get { return !this.Flags.HasFlag(GSHHGFlag.Land) && !this.Flags.HasFlag(GSHHGFlag.Lake) && !this.Flags.HasFlag(GSHHGFlag.PondInIslandInLake); } }

        public bool IsLake { get { return !this.Flags.HasFlag(GSHHGFlag.Land) && this.Flags.HasFlag(GSHHGFlag.Lake) && !this.Flags.HasFlag(GSHHGFlag.PondInIslandInLake); } }

        public bool IsPondInIslandInLake { get { return !this.Flags.HasFlag(GSHHGFlag.Land) && this.Flags.HasFlag(GSHHGFlag.PondInIslandInLake); } }

        public bool IsContinent { get { return this.Flags.HasFlag(GSHHGFlag.Land) && !this.Flags.HasFlag(GSHHGFlag.Lake); } }

        public bool IsIsland { get { return this.Flags.HasFlag(GSHHGFlag.Land) && this.Flags.HasFlag(GSHHGFlag.Lake); } }

        public byte Version { get { return (byte)(this.flag >> 8 & 255); } }

        public bool DoesCrossGreenwich { get { return this.Flags.HasFlag(GSHHGFlag.GreenwichIsCrossed); } }

        public GSHHGSource Source { get { return this.Flags.HasFlag(GSHHGFlag.SourceIsWVS) ? GSHHGSource.WVS : GSHHGSource.WDBII; } }

        public bool IsRiverLake { get { return this.Flags.HasFlag(GSHHGFlag.RiverLake); } }

        public bool IsRiverLevel { get { return this.Flags.HasFlag(GSHHGFlag.RiverLevel); } }

        #endregion

        #region Coordinates

        public int WestMicroDegrees { get { return this.west; } }

        public double WestDegrees { get { return this.west / Constants.Million; } }

        public int EastMicroDegrees { get { return this.east; } }

        public double EastDegrees { get { return this.east / Constants.Million; } }

        public int SouthMicroDegrees { get { return this.south; } }

        public double SouthDegrees { get { return this.south / Constants.Million; } }

        public int NorthMicroDegrees { get { return this.north; } }

        public double NorthDegrees { get { return this.north / Constants.Million; } }

        #endregion

        #region Area

        public int AreaDeciSquareKM { get { return this.area; } }

        public double AreaSquareKM { get { return this.area / 10d; } }

        public int AreaFullDeciSquareKM { get { return this.area_full; } }

        public double AreaFullSquareKM { get { return this.area_full / 10d; } }

        #endregion

        #endregion

        #region Object

        public override int GetHashCode()
        {
            return this.id;
        }

        private static readonly System.Text.RegularExpressions.Regex _removeFlagsBits = new System.Text.RegularExpressions.Regex(",?Bit\\d+");

        public override string ToString()
        {
            var flags = _removeFlagsBits.Replace(this.Flags.ToString().Replace(", ", ","), "");
            return
                "#" + this.Id + ", " +
                this.PointsCount + " points, " +
                getCoord(this.EastDegrees, true) + " " +
                getCoord(this.WestDegrees, true) + " " +
                getCoord(this.NorthDegrees, false) + " " +
                getCoord(this.SouthDegrees, false) + ", " +
                this.AreaSquareKM + " km2 / " +
                this.AreaFullSquareKM + " km2, " +
                "container: " + this.ContainerId + ", " +
                "ancestor: " + this.AncestorId + ", " +
                flags;
        }

        private string getCoord(double degrees, bool isEW)
        {
            var neg = degrees < 0;
            var card = isEW
                ? (neg ? "W" : "E")
                : (neg ? "S" : "N");
            return Math.Abs(degrees) + " º" + card;
        }

        #endregion

        #region Static Functions (Constructors)

        public static GSHHG2PolygonHeader Create(int id, int n, int flag, int west, int east, int south, int north, int area, int area_full, int container, int ancestor)
        {
            var result = new GSHHG2PolygonHeader();
            result.id = id;
            result.n = n;
            result.flag = flag;
            result.west = west;
            result.east = east;
            result.south = south;
            result.north = north;
            result.area = area;
            result.area_full = area_full;
            result.container = container;
            result.ancestor = ancestor;
            result.Boundaries = new LatLonBoundaries()
            {
                CenterLatitude = (north + south) / (2 * Constants.Million),
                LatitudeSpan = Math.Abs(north - south) / Constants.Million,
                CenterLongitude = (east + west) / (2 * Constants.Million),
                LongitudeSpan = Math.Abs(east - west) / Constants.Million
            };
            return result;
        }

        #endregion
    }
}
