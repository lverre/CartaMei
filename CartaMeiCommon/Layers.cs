using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CartaMei.Common
{
    public interface ILayer : INotifyPropertyChanged
    {
        #region Properties

        string Name { get; set; }

        IMapObject Root { get; set; }

        DrawingVisual Canvas { get; set; }

        #endregion

        #region Function

        void Redraw(IDrawContext context);

        #endregion
    }

    public interface IDrawContext
    {
        #region Properties

        IProjection Projection { get; }

        LatLonBoundaries Boundaries { get; }

        bool IsDesign { get; }

        bool IsExport { get; }

        long AnimationStep { get; }
        
        #endregion
    }
    
    public class DrawContext : IDrawContext
    {
        #region Constructor

        public DrawContext(LatLonBoundaries boundaries, IProjection projection, bool isDesign, bool isExport, long animationStep)
        {
            this.Boundaries = boundaries;
            this.Projection = projection;
            this.IsDesign = IsDesign;
            this.IsExport = IsExport;
            this.AnimationStep = animationStep;
        }

        #endregion

        #region Properties

        public LatLonBoundaries Boundaries { get; private set; }

        public IProjection Projection { get; private set; }

        public bool IsDesign { get; private set; }

        public bool IsExport { get; private set; }

        public long AnimationStep { get; private set; }

        #endregion
    }

    public class LatLonBoundaries
    {
        #region Constructor

        public LatLonBoundaries(double latMin, double latMax, double lonMin, double lonMax)
        {
            this.LatMin = latMin;
            this.LatMax = latMax;
            this.LonMin = lonMin;
            this.LonMax = lonMax;
        }

        #endregion

        #region Properties

        public double LatMin { get; private set; }

        public double LatMax { get; private set; }

        public double LonMin { get; private set; }

        public double LonMax { get; private set; }

        #endregion
    }
}
