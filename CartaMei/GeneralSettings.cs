using CartaMei.Common;
using System;
using System.ComponentModel;

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

        #endregion
    }
}
