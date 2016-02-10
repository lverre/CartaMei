using CartaMei.Common;
using CartaMei.Models;
using CartaMei.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        }

        #endregion

        #region Tools

        private void rebuild()
        {
            rebuildMenu();
            rebuildToolbar();
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

        private void rebuildAnchorables()
        {
            var anchorables = new List<IAnchorableTool>();
            var templates = new Dictionary<Type, DataTemplate>()
            {
                {
                    typeof(IMap),
                    new DataTemplate()
                    {
                        VisualTree = new FrameworkElementFactory(typeof(Canvas))// TODO
                    }
                }
            };
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
            var adTemplateSelector = (AvalonDockItemTemplateSelector)this.Resources["adTemplateSelector"];
            if (adTemplateSelector != null)
            {
                adTemplateSelector.Templates = templates;
            }

            _model.Anchorables = new System.Collections.ObjectModel.ObservableCollection<IAnchorableTool>(anchorables);
        }

        #endregion
    }
}
