using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartaMei.GSHHG
{
    public static class Helpers
    {
        #region Polygon

        private static string toString<THeader, TPoint>(IPolygon<THeader, TPoint> polygon, bool showChildren, bool showPoints, int level)
            where THeader : IPolygonHeader
            where TPoint : IPolygonPoint
        {
            var indentation = "";
            for (int i = 0; i < level; i++) indentation += "\t";

            var result = indentation + polygon.Header.ToString();
            if (showPoints)
            {
                foreach (var point in polygon.Points)
                {
                    result += Environment.NewLine + indentation + "\t" +
                        Math.Abs(point.X) + " º" + (point.X >= 0 ? "W" : "E") + " " +
                        Math.Abs(point.Y) + " º" + (point.Y >= 0 ? "N" : "S");
                }
            }
            if (showChildren)
            {
                foreach (var child in polygon.Children)
                {
                    result += Environment.NewLine + toString(child, showChildren, showPoints, level + 1);
                }
            }
            return result;
        }

        public static string ToString<THeader, TPoint>(this IPolygon<THeader, TPoint> polygon, bool showChildren, bool showPoints)
            where THeader : IPolygonHeader
            where TPoint : IPolygonPoint
        {
            return toString(polygon, showChildren, showPoints, 0);
        }

        #endregion

        #region Polygon Header

        public static bool HasContainer(this IPolygonHeader header) { return header.ContainerId != -1; }

        #endregion
    }

    public static class BigEndianBinaryReaderHelper
    {
        public static byte[] Reverse(this byte[] b)
        {
            Array.Reverse(b);
            return b;
        }

        public static UInt16 ReadUInt16BE(this BinaryReader reader)
        {
            return BitConverter.ToUInt16(reader.ReadBytesRequired(sizeof(UInt16)).Reverse(), 0);
        }

        public static Int16 ReadInt16BE(this BinaryReader reader)
        {
            return BitConverter.ToInt16(reader.ReadBytesRequired(sizeof(Int16)).Reverse(), 0);
        }

        public static UInt32 ReadUInt32BE(this BinaryReader reader)
        {
            return BitConverter.ToUInt32(reader.ReadBytesRequired(sizeof(UInt32)).Reverse(), 0);
        }

        public static Int32 ReadInt32BE(this BinaryReader reader)
        {
            return BitConverter.ToInt32(reader.ReadBytesRequired(sizeof(Int32)).Reverse(), 0);
        }

        public static byte[] ReadBytesRequired(this BinaryReader reader, int byteCount)
        {
            var result = reader.ReadBytes(byteCount);

            if (result.Length != byteCount)
                throw new EndOfStreamException(string.Format("{0} bytes required from stream, but only {1} returned.", byteCount, result.Length));

            return result;
        }
    }
}
