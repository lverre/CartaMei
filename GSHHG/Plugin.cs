using CartaMei.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CartaMei.GSHHG
{
    [Plugin(License = "MIT")]
    public class GSHHGPlugin : GraphicalPluginBase
    {
        #region IPlugin
        
        public override IEnumerable<PluginItemProvider<ILayer>> LayerProviders
        {
            get
            {
                // The layers should allow presets (for the brush for example)
                return null;// TODO: provide ShorelinesLayer, ...
            }
        }

        public override object Settings { get { return PluginSettings.Instance; } }

        #endregion
    }
}
