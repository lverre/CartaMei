using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CartaMei.Common
{
    public interface ILayerItem : INotifyPropertyChanged
    {
        #region Properties

        string Name { get; set; }

        ObservableCollection<ILayerItem> Children { get; }

        #endregion
    }

    public interface ILayer : ILayerItem, INotifyPropertyChanged
    {
        #region Properties
        
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

    public class LatLonBoundaries : NotifyPropertyChangedBase
    {
        #region Properties

        private double _latMin;
        [Description("The southernmost latitude.")]
        [DisplayName("South Latitude")]
        public double LatMin
        {
            get { return _latMin; }
            set
            {
                if (_latMin != value)
                {
                    _latMin = value;
                    onPropetyChanged();
                }
            }
        }

        private double _latMax;
        [Description("The northernmost latitude.")]
        [DisplayName("North Latitude")]
        public double LatMax
        {
            get { return _latMax; }
            set
            {
                if (_latMax != value)
                {
                    _latMax = value;
                    onPropetyChanged();
                }
            }
        }

        private double _lonMin;
        [Description("The westernmost longitude.")]
        [DisplayName("West Longitude")]
        public double LonMin
        {
            get { return _lonMin; }
            set
            {
                if (_lonMin != value)
                {
                    _lonMin = value;
                    onPropetyChanged();
                }
            }
        }

        private double _lonMax;
        [Description("The easternmost longitude.")]
        [DisplayName("East Longitude")]
        public double LonMax
        {
            get { return _lonMax; }
            set
            {
                if (_lonMax != value)
                {
                    _lonMax = value;
                    onPropetyChanged();
                }
            }
        }

        #endregion

        #region Object

        public override string ToString()
        {
            return 
                getHumanReadable(this.LatMin, true) + " " + getHumanReadable(this.LonMin, false) + " / " +
                getHumanReadable(this.LatMax, true) + " " + getHumanReadable(this.LonMax, false);
        }

        #endregion

        #region Tools

        private string getHumanReadable(double coordinate, bool isLatitude)
        {
            var isNegative = coordinate < 0;
            var cardinal = isLatitude
                ? (isNegative ? "S" : "N")
                : (isNegative ? "W" : "E");
            return Math.Round(Math.Abs(coordinate), 5) + "º" + cardinal;
        }

        #endregion
    }
}
