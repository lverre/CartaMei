using CartaMei.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace CartaMei.GSHHG
{
    public abstract class AGSHHGReader<TPolygonHeader> : IGSHHGReader<TPolygonHeader>
        where TPolygonHeader : IPolygonHeader
    {
        #region IGSHHGReader

        public IPolygonDatabase<TPolygonHeader, LatLonCoordinates> Read(string fileName, PolygonType type, Resolution resolution)
        {
            if (!File.Exists(fileName) && Directory.Exists(fileName))
            {
                fileName = getFileName(fileName, type, resolution);
            }

            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException();
            }

            var polygons = new Dictionary<int, IPolygon<TPolygonHeader, LatLonCoordinates>>();
            var result = new PolygonDatabase<TPolygonHeader, LatLonCoordinates>(fileName, type, resolution, polygons);

            using (var stream = File.OpenRead(fileName))
            {
                using (var reader = new BinaryReader(stream))
                {
                    reader.BaseStream.Position = 0;

                    var roots = new List<IPolygon<TPolygonHeader>>();
                    var children = new List<IPolygon<TPolygonHeader>>();
                    while (reader.BaseStream.Length - reader.BaseStream.Position >= this.HeaderSize)
                    {
                        var item = readPolygon(reader);
                        item.Children = new List<IPolygon<TPolygonHeader>>();
                        polygons.Add(item.Header.Id, item);
                        if (item.Header.HasContainer())
                        {
                            children.Add(item);
                        }
                        else
                        {
                            roots.Add(item);
                        }
                    }

                    if (roots.Count == 0)
                    {
                        throw new InvalidDataException("There is no root node!");
                    }

                    while (children.Count != 0)
                    {
                        var item = children[children.Count - 1];
                        if (polygons.ContainsKey(item.Header.ContainerId))
                        {
                            ((IList<IPolygon<TPolygonHeader>>)polygons[item.Header.ContainerId].Children).Add(item);
                            children.RemoveAt(children.Count - 1);
                        }
                        else
                        {
                            throw new InvalidDataException("There are orphan nodes!");
                        }
                    }
                }
            }

            return result;
        }

        #endregion

        #region Properties

        public abstract int HeaderSize { get; }

        #endregion

        #region Virtual Functions

        protected virtual string getFileName(string dir, PolygonType type, Resolution resolution)
        {
            string fileName;
            switch (type)
            {
                case PolygonType.Border:
                    fileName = "wdb_borders_";
                    break;
                case PolygonType.River:
                    fileName = "wdb_rivers_";
                    break;
                case PolygonType.ShoreLine:
                    fileName = "gshhs_";
                    break;
                default:
                    throw new ArgumentException("Invalid type: " + type, "type");
            }
            switch (resolution)
            {
                case Resolution.Crude:
                    fileName += 'c';
                    break;
                case Resolution.Full:
                    fileName += 'f';
                    break;
                case Resolution.High:
                    fileName += 'h';
                    break;
                case Resolution.Intermediate:
                    fileName += 'i';
                    break;
                case Resolution.Low:
                    fileName += 'l';
                    break;
                default:
                    throw new ArgumentException("Invalid resolution: " + resolution, "resolution");
            }
            fileName += ".b";
            return Path.Combine(dir, fileName);
        }

        protected virtual IPolygon<TPolygonHeader> getNewPolygon()
        {
            return new Polygon<TPolygonHeader>();
        }

        protected virtual IPolygon<TPolygonHeader> readPolygon(BinaryReader reader)
        {
            var result = getNewPolygon();
            result.Header = readPolygonHeader(reader);
            var points = new LatLonCoordinates[result.Header.PointsCount];
            var lastLongitude = Double.NaN;
            for (int i = 0; i < points.Length; i++)
            {
                var point = readPoint(reader);
                if (!Double.IsNaN(lastLongitude) && Math.Abs(point.Longitude - lastLongitude) >= 180)
                {
                    point.Longitude = point.Longitude.FixCoordinate(false);
                }
                lastLongitude = point.Longitude;
                points[i] = point;
            }
            if (points[0] != points[points.Length - 1])
            {
                // Make sure the list is closed
                var pointsList = new List<LatLonCoordinates>(points);
                pointsList.Add(new LatLonCoordinates() { Latitude = points[0].Latitude, Longitude = points[0].Longitude });
                points = pointsList.ToArray();
            }
            result.Points = points;
            return result;
        }

        protected abstract TPolygonHeader readPolygonHeader(BinaryReader reader);

        protected virtual LatLonCoordinates readPoint(BinaryReader reader)
        {
            var x = reader.ReadInt32BE();
            var y = reader.ReadInt32BE();

            return new LatLonCoordinates()
            {
                Longitude = x / Constants.Million,
                Latitude = y / Constants.Million
            };
        }

        #endregion
    }
}
