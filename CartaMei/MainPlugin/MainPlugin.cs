using CartaMei.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CartaMei.MainPlugin
{
    [Plugin(Name = "Main Plugin", Description = "Provides basic tools and the main menus.", License = "MIT")]
    public class MainPlugin : IPlugin
    {
        #region IPlugin

        public PluginMenu Menu
        {
            get
            {
                return new PluginMenu()
                {
                    ItemsLocation = PluginMenu.PluginMenuLocation.TopLevel,
                    Items = new IButtonModel[] { Buttons.File, Buttons.Edit, Buttons.View, Buttons.Tools, Buttons.Help }
                };
            }
        }

        public IEnumerable<IButtonModel> Toolbar
        {
            get
            {
                return new IButtonModel[] { Buttons.NewMap, Buttons.OpenMap, Buttons.SaveMap, new ButtonModel() { IsSeparator = true }, Buttons.Options };
            }
        }

        public IEnumerable<ILayer> LayerProviders
        {
            get
            {
                return null;// TODO: background layer
            }
        }

        public IEnumerable<IProjection> ProjectionProviders
        {
            get
            {
                return null;// TODO: identity projection
            }
        }

        public IDictionary<IAnchorableTool, DataTemplate> AnchorableTools
        {
            get
            {
                return null;// TODO: layers, properties
            }
        }

        public object Settings
        {
            get { return MainPluginSettings.Instance; }
        }

        #endregion
    }
}
