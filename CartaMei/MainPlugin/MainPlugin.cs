using CartaMei.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CartaMei.MainPlugin
{
    [Plugin(Name = "Main Plugin", Description = "Provides basic tools and the main menus.", License = "MIT")]
    public class MainPlugin : GraphicalPluginBase
    {
        #region Constants

        private static readonly Datum[] _datums = new Datum[] { Datum.WGS84 };

        #endregion

        #region GraphicalPluginBase

        public override PluginMenu Menu
        {
            get
            {
                return new PluginMenu()
                {
                    ItemsLocation = PluginMenu.PluginMenuLocation.TopLevel,
                    Items = new IButtonModel[] { Buttons.File, Buttons.Edit, Buttons.View, Buttons.Layers, Buttons.Tools, Buttons.Help }
                };
            }
        }

        public override IEnumerable<IButtonModel> ToolBar
        {
            get
            {
                return new IButtonModel[]
                {
                    Buttons.NewMapTool, Buttons.OpenMapTool, Buttons.SaveMapTool,
                    new ButtonModel() { IsSeparator = true },
                    Buttons.AddLayerTool, Buttons.RemoveLayerTool, 
                    new ButtonModel() { IsSeparator = true },
                    Buttons.OptionsTool
                };
            }
        }

        public override IEnumerable<IStatusItem> StatusBar
        {
            get
            {
                return new IStatusItem[]
                {
                    // TODO: progress bar for save/load, text
                };
            }
        }

        public override IEnumerable<PluginItemProvider<IProjection>> ProjectionProviders
        {
            get
            {
                yield return new PluginItemProvider<IProjection>()
                {
                    Name = EquirectangularProjection.ProjectionName,
                    Description = EquirectangularProjection.ProjectionDescription,
                    Create = delegate (IMap map) { return new EquirectangularProjection() { Map = map }; }
                };
                yield break;
            }
        }

        public override IEnumerable<Datum> Datums { get { return _datums; } }

        public override IEnumerable<Tuple<IAnchorableTool, DataTemplate>> AnchorableTools
        {
            get
            {
                yield return new Tuple<IAnchorableTool, DataTemplate>(new Models.LayersPanelModel(), new DataTemplate()
                {
                    VisualTree = new FrameworkElementFactory(typeof(Templates.LayersTemplate))
                });
                yield return new Tuple<IAnchorableTool, DataTemplate>(new Models.PropertiesPanelModel(), new DataTemplate()
                {
                    VisualTree = new FrameworkElementFactory(typeof(Templates.PropertiesTemplate))
                });
                yield break;
            }
        }

        public override object Settings
        {
            get { return MainPluginSettings.Instance; }
        }

        #endregion
    }
}
