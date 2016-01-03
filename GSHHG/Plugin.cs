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
    public class GSHHGPlugin : IPlugin
    {
        #region IPlugin
        
        public PluginMenu Menu { get { return null; } }

        public IEnumerable<IButtonModel> Toolbar { get { return null; } }

        public IEnumerable<ILayer> LayerProviders
        {
            get
            {
                // The layers should allow presets (for the brush for example)
                return null;// TODO: provide ShorelinesLayer, ...
            }
        }

        public IEnumerable<IProjection> ProjectionProviders { get { return null; } }

        public IDictionary<IAnchorableTool, DataTemplate> AnchorableTools { get { return null; } }

        public object Settings
        {
            get { return PluginSettings.Instance; }
        }

        #endregion
    }
}
