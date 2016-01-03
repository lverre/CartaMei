using CartaMei.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using CartaMei.Tools;

namespace CartaMei
{
    public class PluginManager
    {
        #region Singleton

        private static readonly PluginManager _instance = new PluginManager();

        internal static PluginManager Instance { get { return _instance; } }

        private PluginManager()
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

        #region Constants

        public const string PluginsDirectory = "Plugins";

        #endregion

        #region Fields

        private CompositionContainer _container;

        #endregion

        #region MEF

        [ImportMany]
        private Lazy<IPlugin, IPluginMetadata>[] LoadedPlugins { get; set; }

        #endregion

        #region Properties

        public IDictionary<IPlugin, IPluginMetadata> Plugins { get; private set; }

        public IEnumerable<IButtonModel> Menus { get; private set; }

        public IList<Tuple<string, IEnumerable<IButtonModel>>> Toolbar { get; private set; }

        public IEnumerable<ILayer> LayerProviders { get; private set; }

        public IEnumerable<IProjection> ProjectionProviders { get; private set; }

        public IDictionary<IAnchorableTool, DataTemplate> AnchorableTools { get; private set; }

        public IDictionary<IPlugin, object> PluginsSettings { get; private set; }

        #endregion

        #region Public Functions

        public void Reload()
        {
            _container.ComposeParts(this);

            var plugins = new Dictionary<IPlugin, IPluginMetadata>();
            var menus = new List<IButtonModel>();
            IButtonModel toolsMenu = null;
            var toolPluginMenus = new List<IButtonModel>();
            var toolbar = new List<Tuple<string, IEnumerable<IButtonModel>>>();
            var layerProviders = new List<ILayer>();
            var projectionProviders = new List<IProjection>();
            var anchorableTools = new Dictionary<IAnchorableTool, DataTemplate>();
            var pluginsSettings = new Dictionary<IPlugin, object>();

            foreach (var item in this.LoadedPlugins)
            {
                var plugin = item.Value;
                var metadata = new PluginMetadata(item.Metadata, plugin.GetType().Assembly);
                plugins.Add(plugin, metadata);
                
                var pMenu = plugin.Menu;
                if (pMenu != null && pMenu.Items.Any())
                {
                    switch (pMenu.ItemsLocation)
                    {
                        case PluginMenu.PluginMenuLocation.TopLevel:
                            menus.AddRange(pMenu.Items);
                            if (toolsMenu == null)
                            {
                                toolsMenu = pMenu.Items.FirstOrDefault(m => m.Name == "Tools" || m.Name == "_Tools");
                            }
                            break;
                        case PluginMenu.PluginMenuLocation.ToolsMenu:
                            toolPluginMenus.AddRange(pMenu.Items);
                            break;
                        case PluginMenu.PluginMenuLocation.ToolsSubmenu:
                            toolPluginMenus.Add(new ButtonModel()
                            {
                                Name = metadata.Name,
                                Children = new System.Collections.ObjectModel.ObservableCollection<IButtonModel>(pMenu.Items)
                            });
                            break;
                    }
                }

                var pToolbar = plugin.Toolbar;
                if (pToolbar != null && pToolbar.Any())
                {
                    toolbar.Add(new Tuple<string, IEnumerable<IButtonModel>>(metadata.Name, plugin.Toolbar));
                }

                var pLayerProviders = plugin.LayerProviders;
                if (pLayerProviders != null && pLayerProviders.Any())
                {
                    layerProviders.AddRange(pLayerProviders);
                }

                var pProjectionProviders = plugin.ProjectionProviders;
                if (pProjectionProviders != null && pProjectionProviders.Any())
                {
                    projectionProviders.AddRange(pProjectionProviders);
                }

                var pAnchorableTools = plugin.AnchorableTools;
                if (pAnchorableTools != null && pAnchorableTools.Any())
                {
                    foreach (var tool in pAnchorableTools)
                    {
                        anchorableTools[tool.Key] = tool.Value;
                    }
                }

                var pSettings = plugin.Settings;
                if (pSettings != null)
                {
                    pluginsSettings.Add(plugin, pSettings);
                }
            }

            if (toolsMenu == null)
            {
                toolsMenu = new ButtonModel()
                {
                    Name = "_Tools"
                };
            }
            else
            {
                toolsMenu.Children.Add(new ButtonModel() { IsSeparator = true });
            }
            foreach (var item in toolPluginMenus)
            {
                toolsMenu.Children.Add(item);
            }

            this.Plugins = plugins;
            this.Menus = menus;
            this.Toolbar = toolbar;
            this.LayerProviders = layerProviders;
            this.ProjectionProviders = projectionProviders;
            this.AnchorableTools = anchorableTools;
            this.PluginsSettings = pluginsSettings;
        }

        #endregion
    }

    public class PluginMetadata : IPluginMetadata
    {
        #region Constructor

        public PluginMetadata(IPluginMetadata reference, Assembly assembly)
        {
            var asmNameData = assembly.GetName();
            this.Name = reference.Name ?? asmNameData.Name;
            this.Version = reference.Version ?? asmNameData.Version.ToString();
            this.Description = reference.Description ?? assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
            this.Author = reference.Author ?? assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;
            this.Copyright = reference.Copyright ?? assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright;
            this.Trademark = reference.Trademark ?? assembly.GetCustomAttribute<AssemblyTrademarkAttribute>()?.Trademark;
        }

        #endregion

        #region IPluginMetadata

        public string License { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Author { get; set; }

        public string Copyright { get; set; }

        public string Trademark { get; set; }

        public string Version { get; set; }

        public string Link { get; set; }

        #endregion
    }
}
