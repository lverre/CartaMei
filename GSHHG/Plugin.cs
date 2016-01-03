using CartaMei.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartaMei.GSHHG
{
    [Plugin(License = "blah")]
    public class GSHHGPlugin : IPlugin
    {
        #region IPlugin
        
        public PluginMenu Menu { get { return null; } }

        public IEnumerable<IButtonModel> Toolbar { get { return null; } }

        public IEnumerable<ILayer> LayerProviders
        {
            get
            {
                return null;// TODO: provide ShorelinesLayer, ...
            }
        }

        public IEnumerable<IProjection> ProjectionProviders { get { return null; } }

        public object Settings
        {
            get
            {
                return null;// TODO
            }
        }

        #endregion
    }
}
