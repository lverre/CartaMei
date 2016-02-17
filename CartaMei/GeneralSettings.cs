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
                    instance = Common.Tools.GetDefault<GeneralSettings>();
                    CartaMei.Properties.Settings.Default.GeneralSettings = instance;
                }
                return instance;
            }
        }

        #endregion

        #region Properties

        private bool _UseAntiAliasing;
        [DefaultValue(true)]
        [Category("Map")]
        [Description("Use anti-aliasing for the map.")]
        [DisplayName("Use Anti-Aliasing")]
        [PropertyOrder(0)]
        public bool UseAntiAliasing
        {
            get { return _UseAntiAliasing; }
            set
            {
                if (_UseAntiAliasing != value)
                {
                    _UseAntiAliasing = value;
                    onPropetyChanged();
                }
            }
        }

        #endregion
    }
}
