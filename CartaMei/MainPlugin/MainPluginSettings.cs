using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartaMei.MainPlugin
{
    public class MainPluginSettings
    {
        #region Singleton

        private static readonly MainPluginSettings _instance = new MainPluginSettings();

        public static MainPluginSettings Instance { get { return _instance; } }

        private MainPluginSettings() { }

        #endregion

        #region Properties

        #endregion
    }
}
