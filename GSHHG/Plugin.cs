using CartaMei.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
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
                yield return new PluginItemProvider<ILayer>()
                {
                    Name = ShorelineLayer.LayerName,
                    Description = ShorelineLayer.LayerDescription,
                    Create = delegate (IMap map)
                    {
                        if (!Directory.Exists(PluginSettings.Instance.MapsDirectory?.FullName))
                        {
                            // TODO: alert + show options window
                            PluginSettings.Instance.MapsDirectory = new DirectoryInfo(@"C:\Users\Laurian\Downloads\indy maps\gshhg-bin-2.3.4");
                        }
                        return new ShorelineLayer(map);
                    }
                };
                // TODO: provide other kind of layers provided by gshhg data
                yield break;
            }
        }

        public override IEnumerable<IStatusItem> StatusBar
        {
            get
            {
                yield return Utils.Instance.StatusText;
                yield return Utils.Instance.StatusProgress;
                yield break;
            }
        }

        public override object Settings { get { return PluginSettings.Instance; } }

        #endregion
    }
}
