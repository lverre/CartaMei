using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartaMei
{
    public class GeneralSettings
    {
        #region Singleton

        private static readonly GeneralSettings _instance = new GeneralSettings();

        public static GeneralSettings Instance { get { return _instance; } }

        private GeneralSettings() { }

        #endregion

        #region Properties

        #endregion
    }
}
