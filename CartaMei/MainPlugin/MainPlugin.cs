﻿using CartaMei.Common;
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
                return null;// TODO: file, edit, etc.
            }
        }

        public IEnumerable<IButtonModel> Toolbar
        {
            get
            {
                return null;// TODO: new, open, save, etc.
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
            get
            {
                return null;// TODO
            }
        }

        #endregion
    }
}
