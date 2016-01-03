using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CartaMei.Common
{
    public class PluginManager
    {
        #region Fields

        private CompositionContainer _container;

        #endregion

        #region Constructor

        public PluginManager()
        {
            var mainAsm = Assembly.GetExecutingAssembly();

            var pluginsDir = Path.Combine(Path.GetDirectoryName(mainAsm.Location), "Plugins");
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

    public interface IPlugin
    {
        #region Properties
        
        #endregion
    }

    public interface IPluginMetadata
    {
        #region Properties

        [DefaultValue(null)]
        string License { get; }

        [DefaultValue(null)]
        string Name { get; }

        [DefaultValue(null)]
        string Description { get; }

        [DefaultValue(null)]
        string Author { get; }

        [DefaultValue(null)]
        string Link { get; }

        #endregion
    }

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PluginAttribute : ExportAttribute, IPluginMetadata
    {
        #region Constructor

        public PluginAttribute() : base(typeof(IPlugin)) { }

        #endregion

        #region IPluginMetadata

        public string License { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Author { get; set; }

        public string Link { get; set; }

        #endregion
    }
}
