using CartaMei.Common;
using System.Collections.Generic;

namespace CartaMei.GSHHG
{
    public interface IGSHHGVersion<THeader, TPoint>
        where THeader : IPolygonHeader
        where TPoint : LatLonCoordinates
    {
        #region Properties

        int Id { get; }

        string Version { get; }

        string ReleaseDate { get; }

        string Description { get; }

        IGSHHGReader<THeader, TPoint> Reader { get; }

        #endregion

        #region Functions

        bool IsCompatibleWith(int version);

        #endregion
    }

    public interface IGSHHGVersion<THeader> : IGSHHGVersion<THeader, LatLonCoordinates> where THeader : IPolygonHeader { }

    public interface IGSHHGReader<THeader, TPoint>
        where THeader : IPolygonHeader
        where TPoint : LatLonCoordinates
    {
        #region Functions

        IPolygonDatabase<THeader, TPoint> Read(string fileName, PolygonType type, Resolution resolution);
        
        #endregion
    }

    public interface IGSHHGReader<THeader> : IGSHHGReader<THeader, LatLonCoordinates> where THeader : IPolygonHeader { }

    public interface IPolygonDatabase<THeader, TPoint>
        where THeader : IPolygonHeader
        where TPoint : LatLonCoordinates
    {
        #region Properties
        
        PolygonType PolygonType { get; }

        Resolution Resolution { get; }

        IDictionary<int, IPolygon<THeader, TPoint>> Polygons { get; }

        #endregion
    }

    public interface IPolygonDatabase<THeader> : IPolygonDatabase<THeader, LatLonCoordinates> where THeader : IPolygonHeader { }

    public interface IPolygon<THeader, TPoint>
        where THeader : IPolygonHeader
        where TPoint : LatLonCoordinates
    {
        #region Properties

        THeader Header { get; set; }

        IEnumerable<TPoint> Points { get; set; }

        IPolygon<THeader, TPoint> Parent { get; set; }

        IEnumerable<IPolygon<THeader, TPoint>> Children { get; set; }

        #endregion
    }

    public interface IPolygon<THeader> : IPolygon<THeader, LatLonCoordinates> where THeader : IPolygonHeader { }

    public interface IPolygonHeader
    {
        #region Properties
        
        int Id { get; }

        int ContainerId { get; }
        
        LatLonBoundaries Boundaries { get; }
        
        int PointsCount { get; }

        #endregion
    }
}
