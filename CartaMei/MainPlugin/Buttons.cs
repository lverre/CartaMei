using CartaMei.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartaMei.MainPlugin
{
    internal static class Buttons
    {
        #region Static Constructor

        static Buttons()
        {
            NewMap = new ButtonModel()
            {
                Name = "_New Map",
                IsEnabled = true
            };
            NewMap.Click += onNewMap;
            NewMapTool = new ButtonModel()
            {
                Name = "New Map",
                IsEnabled = true
            };
            NewMapTool.Click += onNewMap;

            OpenMap = new ButtonModel()
            {
                Name = "_Open Map",
                IsEnabled = true
            };
            OpenMap.Click += onOpenMap;
            OpenMapTool = new ButtonModel()
            {
                Name = "Open Map",
                IsEnabled = true
            };
            OpenMapTool.Click += onOpenMap;

            SaveMap = new ButtonModel()
            {
                Name = "_Save Map",
                IsEnabled = isSaveEnabled()
            };
            SaveMap.Click += onSaveMap;
            SaveMapTool = new ButtonModel()
            {
                Name = "Save Map",
                IsEnabled = isSaveEnabled()
            };
            SaveMapTool.Click += onSaveMap;
            Current.MapChanged += delegate (object sender, EventArgs e)
            {
                SaveMap.IsEnabled = isSaveEnabled();
                SaveMapTool.IsEnabled = isSaveEnabled();
            };

            Options = new ButtonModel()
            {
                Name = "_Options",
                IsEnabled = true
            };
            Options.Click += onOptions;
            OptionsTool = new ButtonModel()
            {
                Name = "Options",
                IsEnabled = true
            };
            OptionsTool.Click += onOptions;

            File = new ButtonModel()
            {
                Name = "_File",
                IsEnabled = true,
                Children = new System.Collections.ObjectModel.ObservableCollection<IButtonModel>()
                {
                    NewMap,
                    OpenMap,
                    SaveMap
                }
            };

            Edit = new ButtonModel()
            {
                Name = "_Edit",
                IsEnabled = true
            };

            View = new ButtonModel()
            {
                Name = "_View",
                IsEnabled = true
            };

            Tools = new ButtonModel()
            {
                Name = "_Tools",
                IsEnabled = true,
                Children = new System.Collections.ObjectModel.ObservableCollection<IButtonModel>()
                {
                    Options
                }
            };

            Help = new ButtonModel()
            {
                Name = "_Help",
                IsEnabled = true
            };
        }

        #endregion

        #region Menus

        internal static readonly ButtonModel NewMap;
        internal static readonly ButtonModel OpenMap;
        internal static readonly ButtonModel SaveMap;
        internal static readonly ButtonModel Options;

        internal static readonly ButtonModel File;
        internal static readonly ButtonModel Edit;
        internal static readonly ButtonModel View;
        internal static readonly ButtonModel Tools;
        internal static readonly ButtonModel Help;

        #endregion

        #region Toolbar

        internal static readonly ButtonModel NewMapTool;
        internal static readonly ButtonModel OpenMapTool;
        internal static readonly ButtonModel SaveMapTool;
        internal static readonly ButtonModel OptionsTool;

        #endregion

        #region Tools

        private static void onNewMap(object sender, EventArgs args)
        {
        }

        private static void onOpenMap(object sender, EventArgs args)
        {
        }

        private static void onSaveMap(object sender, EventArgs args)
        {
        }

        private static void onOptions(object sender, EventArgs args)
        {
        }

        private static bool? isSaveEnabled()
        {
            return Current.Map != null;
        }

        #endregion
    }
}
