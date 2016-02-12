using CartaMei.Common;
using System;
using System.ComponentModel;

namespace CartaMei.MainPlugin
{
    [Serializable]
    public class MainPluginSettings : NotifyPropertyChangedBase
    {
        #region Instance
        
        public static MainPluginSettings Instance
        {
            get
            {
                var instance = CartaMei.Properties.Settings.Default.MainPluginSettings;
                if (instance == null)
                {
                    instance = Common.Tools.GetDefault<MainPluginSettings>();
                    CartaMei.Properties.Settings.Default.MainPluginSettings = instance;
                }
                return instance;
            }
        }

        #endregion

        #region Properties
        
        #endregion
    }
}
