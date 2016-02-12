using CartaMei.Common;
using CartaMei.Models;
using CartaMei.WPF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace CartaMei
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        private MainWindowModel _model;

        #endregion

        #region Constructor

        public MainWindow()
        {
            Tools.Utils.Touch();// We need to do that before any plugin is loaded so we "own" Current.
            Tools.Utils.MainWindow = this;

            _model = new Models.MainWindowModel();
            this.DataContext = _model;
            Tools.Utils.MainWindowModel = _model;

            this.InitializeComponent();
            
            PluginManager.Instance.Reload();
            rebuild();

#if DEBUG
            var newMap = new NewMapModel()
            {
                Datum = Datum.WGS84,
                Projection = PluginManager.Instance.ProjectionProviders.First(),
                Name = "test"
            };
            var map = newMap.CreateMap();
            Tools.Utils.Current.SetMap(map);
            map.ActiveObject = map;
            map.Layers.Add(PluginManager.Instance.LayerProviders.First().Create(map));
#endif
        }

#endregion

        #region Event Handlers

        private void loaded(object sender, RoutedEventArgs e)
        {
            loadLayout();
        }

        private void closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            saveLayout();

            foreach (var plugin in PluginManager.Instance.Plugins.Keys)
            {
                plugin.Unload();
            }
        }

        #endregion

        #region Layout

        private void saveLayout()
        {
            var xmlLayoutSerializer = new XmlLayoutSerializer(_dockingManager);
            var stringBuilder = new StringBuilder();
            using (var textWriter = new StringWriter(stringBuilder))
            {
                xmlLayoutSerializer.Serialize(textWriter);
            }
            Properties.Settings.Default.Layout = stringBuilder.ToString();
            Properties.Settings.Default.Save();
        }

        private void loadLayout()
        {
            var serializedLayout = Properties.Settings.Default.Layout;
            if (!string.IsNullOrEmpty(serializedLayout))
            {
                var xmlLayoutSerializer = new XmlLayoutSerializer(_dockingManager);
                using (var stringReader = new StringReader(serializedLayout))
                {
                    xmlLayoutSerializer.Deserialize(stringReader);
                }
            }
        }

        #endregion

        #region Tools

        private void rebuild()
        {
            rebuildMenu();
            rebuildToolbar();
            rebuildStatusBar();
            rebuildAnchorables();
        }

        private void rebuildMenu()
        {
            _model.Menu = PluginManager.Instance.Menus;
            this.InputBindings.Clear();
            addInputBindings(_model.Menu);
        }

        private void addInputBindings(IEnumerable<IButtonModel> items)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    var shortcut = item.Shortcut;
                    if (shortcut != null)
                    {
                        this.InputBindings.Add(new InputBinding(item, shortcut));
                    }
                    addInputBindings(item.Children);
                }
            }
        }

        private void rebuildToolbar()
        {
            var tools = new List<IButtonModel>();
            if (PluginManager.Instance.Toolbar != null)
            {
                var isFirst = true;
                foreach (var item in PluginManager.Instance.Toolbar)
                {
                    tools.AddRange(item.Item2);
                    if (!isFirst) tools.Add(new ButtonModel() { IsSeparator = true });
                }
            }
            _model.Tools = tools;
        }

        private void rebuildStatusBar()
        {
            var statusItems = new List<IStatusItem>();
            if (PluginManager.Instance.StatusItems != null)
            {
                var isFirst = true;
                foreach (var item in PluginManager.Instance.StatusItems)
                {
                    statusItems.AddRange(item.Item2);
                    if (!isFirst) statusItems.Add(new SeparatorStatusItem() { IsVisible = true });
                }
            }
            _model.StatusItems = statusItems;
        }
        
        private void rebuildAnchorables()
        {
            var anchorables = new List<IDockElement>();
            var templates = new Dictionary<Type, DataTemplate>();
            if (PluginManager.Instance.AnchorableTools != null)
            {
                foreach (var item in PluginManager.Instance.AnchorableTools)
                {
                    if (item.Item1 != null)
                    {
                        anchorables.Add(item.Item1);
                        templates[item.Item1.GetType()] = item.Item2;
                    }
                }
            }
            templates[typeof(MapModel)] = new DataTemplate()
            {
                VisualTree = new FrameworkElementFactory(typeof(WPF.LayersContainer))
            };
            var adTemplateSelector = (DictionaryTemplateSelector)this.Resources["adTemplateSelector"];
            if (adTemplateSelector != null)
            {
                adTemplateSelector.Templates = templates;
            }

            _model.Anchorables = new System.Collections.ObjectModel.ObservableCollection<IDockElement>(anchorables);
        }

        #endregion
    }
}
