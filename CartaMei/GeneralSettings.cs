using CartaMei.Common;
using System;
using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace CartaMei
{
    [Serializable]
    public class GeneralSettings : NotifyPropertyChangedBase
    {
        #region Instance

        public static GeneralSettings Instance
        {
            get
            {
                var instance = CartaMei.Properties.Settings.Default.GeneralSettings;
                if (instance == null)
                {
                    instance = new GeneralSettings();
                    CartaMei.Properties.Settings.Default.GeneralSettings = instance;
                }
                return instance;
            }
        }

        #endregion

        #region Constructor

        public GeneralSettings()
        {
            this.SetDefaults();
        }

        #endregion

        #region Properties

        private bool _useAntiAliasing;
        [DefaultValue(true)]
        [Category("Map")]
        [Description("Use anti-aliasing for the map.")]
        [DisplayName("Use Anti-Aliasing")]
        [PropertyOrder(0)]
        public bool UseAntiAliasing
        {
            get { return _useAntiAliasing; }
            set
            {
                if (_useAntiAliasing != value)
                {
                    _useAntiAliasing = value;
                    onPropetyChanged();
                }
            }
        }

        private bool _rotateReference;
        [DefaultValue(false)]
        [Category("Map")]
        [Description("When this option is used, the reference of sphere will be rotated to the center of the map.\nThis makes the shapes more accurate near the center of the map which is especially useful at high latitudes.")]
        [DisplayName("Rotate Reference")]
        [PropertyOrder(1)]
        public bool RotateReference
        {
            get { return _rotateReference; }
            set
            {
                if (value != _rotateReference)
                {
                    _rotateReference = value;
                    onPropetyChanged();
                }
            }
        }

        #endregion
    }
}
