using System;
using System.ComponentModel;
using System.Windows.Media;
using CartaMei.Common;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace CartaMei.GSHHG
{
    [Serializable]
    public class PluginSettings : NotifyPropertyChangedBase
    {
        #region Instance

        public static PluginSettings Instance
        {
            get
            {
                var instance = CartaMei.GSHHG.Properties.Settings.Default.PluginSettings;
                if (instance == null)
                {
                    instance = Common.Tools.GetDefault<PluginSettings>();
                    CartaMei.GSHHG.Properties.Settings.Default.PluginSettings = instance;
                }
                return instance;
            }
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
        [DefaultValue(true)]
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
        [DefaultValue(Resolution.Crude)]
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

        private bool _useCurvedLines;
        [DefaultValue(true)]
        [Description("When enabled, this feature display curved (bezier) lines instead of straight lines.")]
        [DisplayName("Curved Lines")]
        [Category("General")]
        [PropertyOrder(1)]
        public bool UseCurvedLines
        {
            get { return _useCurvedLines; }
            set
            {
                if (value != _useCurvedLines)
                {
                    _useCurvedLines = value;
                    onPropetyChanged();
                }
            }
        }

        #endregion

        #region Shorelines

        private double _shorelinesThickness;
        [DefaultValue(1)]
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
        [DefaultValue(typeof(SolidColorBrush), "#FF000000")]
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
        [DefaultValue(typeof(SolidColorBrush), "#FFF0F8FF")]
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
        [DefaultValue(typeof(SolidColorBrush), "#FFF4A460")]
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
