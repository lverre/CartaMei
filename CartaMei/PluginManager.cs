using CartaMei.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CartaMei
{
    public class PluginManager
    {
        #region Constants

        public const string PluginsDirectory = "Plugins";

        #endregion

        #region Fields

        private CompositionContainer _container;

        #endregion

        #region Constructor

        public PluginManager()
        {
            var mainAsm = Assembly.GetExecutingAssembly();

            var pluginsDir = Path.Combine(Path.GetDirectoryName(mainAsm.Location), PluginsDirectory);
            if (!Directory.Exists(pluginsDir))
            {
                Directory.CreateDirectory(pluginsDir);
            }

            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(mainAsm));
            catalog.Catalogs.Add(new DirectoryCatalog(pluginsDir, "*.dll"));

            _container = new CompositionContainer(catalog);
        }

        #endregion

        #region Properties

        [ImportMany]
        public Lazy<IPlugin, IPluginMetadata>[] Plugins { get; set; }

        #endregion

        #region Public Functions

        public void Reload()
        {
            _container.ComposeParts(this);
        }

        #endregion
    }
}
