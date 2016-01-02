using System.Collections.Generic;

namespace CartaMei.GSHHG
{
    public interface IGSHHGVersion<THeader, TPoint>
        where THeader : IPolygonHeader
        where TPoint : IPolygonPoint
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

    public interface IGSHHGVersion<THeader> : IGSHHGVersion<THeader, PolygonPoint> where THeader : IPolygonHeader { }

    public interface IGSHHGReader<THeader, TPoint>
        where THeader : IPolygonHeader
        where TPoint : IPolygonPoint
    {
        #region Functions

        IPolygonDatabase<THeader, TPoint> Read(string fileName, PolygonType type, Resolution resolution);
        
        #endregion
    }

    public interface IGSHHGReader<THeader> : IGSHHGReader<THeader, PolygonPoint> where THeader : IPolygonHeader { }

    public interface IPolygonDatabase<THeader, TPoint>
        where THeader : IPolygonHeader
        where TPoint : IPolygonPoint
    {
        #region Properties
        
        PolygonType PolygonType { get; }

        Resolution Resolution { get; }

        IDictionary<int, IPolygon<THeader, TPoint>> Polygons { get; }

        #endregion
    }

    public interface IPolygonDatabase<THeader> : IPolygonDatabase<THeader, PolygonPoint> where THeader : IPolygonHeader { }

    public interface IPolygon<THeader, TPoint>
        where THeader : IPolygonHeader
        where TPoint : IPolygonPoint
    {
        #region Properties

        THeader Header { get; set; }

        IEnumerable<TPoint> Points { get; set; }

        IPolygon<THeader, TPoint> Parent { get; set; }

        IEnumerable<IPolygon<THeader, TPoint>> Children { get; set; }

        #endregion
    }

    public interface IPolygon<THeader> : IPolygon<THeader, PolygonPoint> where THeader : IPolygonHeader { }

    public interface IPolygonHeader
    {
        #region Properties
        
        int Id { get; }

        int ContainerId { get; }
        
        CoordinatesRectangle Limits { get; }
        
        int PointsCount { get; }

        #endregion
    }

    public interface IPolygonPoint
    {
        #region Properties

        /// <summary>
        /// Gets the x (longitude) coordinate, in degrees, of the point.
        /// </summary>
        double X { get; }

        /// <summary>
        /// Gets the y (latitude) coordinate, in degrees, of the point.
        /// </summary>
        double Y { get; }

        #endregion
    }
}
