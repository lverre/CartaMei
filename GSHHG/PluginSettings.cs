using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CartaMei.GSHHG
{
    public class PluginSettings
    {
        #region Singleton

        private static readonly PluginSettings _instance = new PluginSettings();

        public static PluginSettings Instance { get { return _instance; } }

        private PluginSettings()
        {
            this.ShorelinesBrush = Brushes.Black;
            this.ShorelinesWaterFill = Brushes.Blue;
            this.ShorelinesLandFill = Brushes.Brown;
            this.ShorelinesThickness = 1;
        }

        #endregion

        #region Properties
        
        [Description("The path of the directory that contains the GSHHG map files.")]
        [DisplayName("Maps Directory")]
        public DirectoryInfo MapsDirectory { get; set; }

        public double ShorelinesThickness { get; set; }

        public Brush ShorelinesBrush { get; set; }

        public Brush ShorelinesWaterFill { get; set; }

        public Brush ShorelinesLandFill { get; set; }

        #endregion
    }
}
