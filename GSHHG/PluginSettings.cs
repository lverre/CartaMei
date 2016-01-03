using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartaMei.GSHHG
{
    public class PluginSettings
    {
        #region Singleton

        private static readonly PluginSettings _instance = new PluginSettings();

        public static PluginSettings Instance { get { return _instance; } }

        private PluginSettings() { }

        #endregion

        #region Properties

        #endregion
    }
}
