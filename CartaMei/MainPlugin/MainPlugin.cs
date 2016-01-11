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
                    Items = new IButtonModel[] { Buttons.File, Buttons.Edit, Buttons.View, Buttons.Tools, Buttons.Help }
                };
            }
        }

        public override IEnumerable<IButtonModel> Toolbar
        {
            get
            {
                return new IButtonModel[] { Buttons.NewMap, Buttons.OpenMap, Buttons.SaveMap, new ButtonModel() { IsSeparator = true }, Buttons.Options };
            }
        }

        public override IEnumerable<Datum> Datums { get { return _datums; } }

        public override IDictionary<IAnchorableTool, DataTemplate> AnchorableTools
        {
            get
            {
                return null;// TODO: layers, properties
            }
        }

        public override object Settings
        {
            get { return MainPluginSettings.Instance; }
        }

        #endregion
    }
}
