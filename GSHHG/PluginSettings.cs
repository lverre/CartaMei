using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using CartaMei.Common;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace CartaMei.GSHHG
{
    public class PluginSettings : NotifyPropertyChangedBase
    {
        #region Singleton

        private static readonly PluginSettings _instance = new PluginSettings();

        public static PluginSettings Instance { get { return _instance; } }

        private PluginSettings()
        {
            this.ShorelinesThickness = 1;
            this.ShorelinesBrush = Brushes.Black;
            this.ShorelinesWaterFill = Brushes.Blue;
            this.ShorelinesLandFill = Brushes.Brown;
            this.UseAutoResolution = true;
            this.Resolution = Resolution.Low;
        }

        #endregion

        #region Properties

        #region General

        private string _mapsDirectory;
        [Description("The path of the directory that contains the GSHHG map files.")]
        [DisplayName("Maps Directory")]
        [Category("General")]
        [PropertyOrder(0)]
        public string MapsDirectory
        {
            get { return _mapsDirectory; }
            set
            {
                if (_mapsDirectory != value)
                {
                    _mapsDirectory = value;
                    onPropetyChanged();
                }
            }
        }

        private bool _useAutoResolution;
        [Description("When enabled, this feature will use the best resolution given the map boundaries.")]
        [DisplayName("Auto Resolution")]
        [Category("General")]
        [PropertyOrder(1)]
        public bool UseAutoResolution
        {
            get { return _useAutoResolution; }
            set
            {
                if (value != _useAutoResolution)
                {
                    _useAutoResolution = value;
                    onPropetyChanged();
                }
            }
        }

        private Resolution _resolution;
        [Description("The resolution to use.")]
        [DisplayName("Resolution")]
        [Category("General")]
        [PropertyOrder(2)]
        public Resolution Resolution
        {
            get { return _resolution; }
            set
            {
                if (value != _resolution)
                {
                    _resolution = value;
                    onPropetyChanged();
                }
            }
        }

        #endregion

        #region Shorelines

        private double _shorelinesThickness;
        [Description("The thickness of the contour of the shorelines.")]
        [DisplayName("Shorelines Thickness")]
        [Category("Shorelines")]
        [PropertyOrder(0)]
        public double ShorelinesThickness
        {
            get { return _shorelinesThickness; }
            set
            {
                if (_shorelinesThickness != value)
                {
                    _shorelinesThickness = value;
                    onPropetyChanged();
                }
            }
        }

        private Brush _shorelinesBrush;
        [Description("The brush used to draw the contour of the shorelines.")]
        [DisplayName("Shorelines Brush")]
        [Category("Shorelines")]
        [PropertyOrder(1)]
        public Brush ShorelinesBrush
        {
            get { return _shorelinesBrush; }
            set
            {
                if (_shorelinesBrush != value)
                {
                    _shorelinesBrush = value;
                    onPropetyChanged();
                }
            }
        }

        private Brush _shorelinesWaterFill;
        [Description("The brush used to fill water areas.")]
        [DisplayName("Water Fill")]
        [Category("Shorelines")]
        [PropertyOrder(2)]
        public Brush ShorelinesWaterFill
        {
            get { return _shorelinesWaterFill; }
            set
            {
                if (_shorelinesWaterFill != value)
                {
                    _shorelinesWaterFill = value;
                    onPropetyChanged();
                }
            }
        }

        private Brush _shorelinesLandFill;
        [Description("The brush used to fill land areas.")]
        [DisplayName("Land Fill")]
        [Category("Shorelines")]
        [PropertyOrder(3)]
        public Brush ShorelinesLandFill
        {
            get { return _shorelinesLandFill; }
            set
            {
                if (_shorelinesLandFill != value)
                {
                    _shorelinesLandFill = value;
                    onPropetyChanged();
                }
            }
        }

        #endregion

        #endregion
    }
}
