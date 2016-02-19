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
                    instance = new PluginSettings();
                    CartaMei.GSHHG.Properties.Settings.Default.PluginSettings = instance;
                }
                return instance;
            }
        }

        #endregion

        #region Constructor

        public PluginSettings()
        {
            this.SetDefaults();
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
        
        private Resolution _resolution;
        [DefaultValue(Resolution.Crude)]
        [Description("The resolution to use.")]
        [DisplayName("Resolution")]
        [Category("General")]
        [PropertyOrder(1)]
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
        [PropertyOrder(2)]
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

        private double _shorelineThickness;
        [DefaultValue(1)]
        [Description("The thickness of the contour of the shorelines.")]
        [DisplayName("Shoreline Thickness")]
        [Category("Shorelines")]
        [PropertyOrder(0)]
        public double ShorelineThickness
        {
            get { return _shorelineThickness; }
            set
            {
                if (_shorelineThickness != value)
                {
                    _shorelineThickness = value;
                    onPropetyChanged();
                }
            }
        }

        private Color _shorelineBrush;
        [DefaultValue(typeof(Color), "#FF000000")]
        [Description("The Color used to draw the contour of the shorelines.")]
        [DisplayName("Shoreline Brush")]
        [Category("Shorelines")]
        [PropertyOrder(1)]
        public Color ShorelineBrush
        {
            get { return _shorelineBrush; }
            set
            {
                if (_shorelineBrush != value)
                {
                    _shorelineBrush = value;
                    onPropetyChanged();
                }
            }
        }

        private bool _shorelinesUseWaterForBackground;
        [DefaultValue(true)]
        [Description("Choose this option if you want the background of the shorelines layers to always match that of the water.\nNote that this will be automatically set only if the layer is added first.")]
        [DisplayName("Shorelines Background Is Water")]
        [Category("Shorelines")]
        [PropertyOrder(2)]
        public bool ShorelinesUseWaterForBackground
        {
            get { return _shorelinesUseWaterForBackground; }
            set
            {
                if (_shorelinesUseWaterForBackground != value)
                {
                    _shorelinesUseWaterForBackground = value;
                    onPropetyChanged();
                }
            }
        }

        private Color _shorelinesBackground;
        [DefaultValue(typeof(Color), "#00000000")]
        [Description("The Color used to fill the background of the shorelines layers.")]
        [DisplayName("Shorelines Background")]
        [Category("Shorelines")]
        [PropertyOrder(3)]
        public Color ShorelinesBackground
        {
            get { return _shorelinesBackground; }
            set
            {
                if (_shorelinesBackground != value)
                {
                    _shorelinesBackground = value;
                    onPropetyChanged();
                }
            }
        }

        private Color _shorelinesWaterFill;
        [DefaultValue(typeof(Color), "#FFF0F8FF")]
        [Description("The Color used to fill water areas.")]
        [DisplayName("Water Fill")]
        [Category("Shorelines")]
        [PropertyOrder(4)]
        public Color ShorelinesWaterFill
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

        private Color _shorelinesLandFill;
        [DefaultValue(typeof(Color), "#FFF4A460")]
        [Description("The Color used to fill land areas.")]
        [DisplayName("Land Fill")]
        [Category("Shorelines")]
        [PropertyOrder(5)]
        public Color ShorelinesLandFill
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

        private Color _antarcticaFill;
        [DefaultValue(typeof(Color), "#FFF4A460")]
        [Description("The Color used to fill Antarctica.")]
        [DisplayName("Antarctica Fill")]
        [Category("Shorelines")]
        [PropertyOrder(6)]
        public Color AntarcticaFill
        {
            get { return _antarcticaFill; }
            set
            {
                if (_antarcticaFill != value)
                {
                    _antarcticaFill = value;
                    onPropetyChanged();
                }
            }
        }

        private Color _antarcticaIceFrontFill;
        [DefaultValue(typeof(Color), "#FFFFFFFF")]
        [Description("The Color used to fill the Antarctica ice front.")]
        [DisplayName("Antarctica Ice Front Fill")]
        [Category("Shorelines")]
        [PropertyOrder(7)]
        public Color AntarcticaIceFrontFill
        {
            get { return _antarcticaIceFrontFill; }
            set
            {
                if (_antarcticaIceFrontFill != value)
                {
                    _antarcticaIceFrontFill = value;
                    onPropetyChanged();
                }
            }
        }

        #endregion

        #region Rivers

        private double _riverThickness;
        [DefaultValue(2)]
        [Description("The thickness of the contour of the rivers.")]
        [DisplayName("River Thickness")]
        [Category("Rivers")]
        [PropertyOrder(0)]
        public double RiverThickness
        {
            get { return _riverThickness; }
            set
            {
                if (_riverThickness != value)
                {
                    _riverThickness = value;
                    onPropetyChanged();
                }
            }
        }

        private Color _riverBrush;
        [DefaultValue(typeof(Color), "#FFF0F8FF")]
        [Description("The Color used to draw the contour of the rivers.")]
        [DisplayName("River Brush")]
        [Category("Rivers")]
        [PropertyOrder(1)]
        public Color RiverBrush
        {
            get { return _riverBrush; }
            set
            {
                if (_riverBrush != value)
                {
                    _riverBrush = value;
                    onPropetyChanged();
                }
            }
        }

        #endregion

        #region Border

        private double _borderThickness;
        [DefaultValue(1)]
        [Description("The thickness of the contour of the borders.")]
        [DisplayName("Border Thickness")]
        [Category("Borders")]
        [PropertyOrder(0)]
        public double BorderThickness
        {
            get { return _borderThickness; }
            set
            {
                if (_borderThickness != value)
                {
                    _borderThickness = value;
                    onPropetyChanged();
                }
            }
        }

        private Color _borderBrush;
        [DefaultValue(typeof(Color), "#FF000000")]
        [Description("The Color used to draw the contour of the borders.")]
        [DisplayName("Border Brush")]
        [Category("Borders")]
        [PropertyOrder(1)]
        public Color BorderBrush
        {
            get { return _borderBrush; }
            set
            {
                if (_borderBrush != value)
                {
                    _borderBrush = value;
                    onPropetyChanged();
                }
            }
        }

        #endregion

        #endregion
    }
}
