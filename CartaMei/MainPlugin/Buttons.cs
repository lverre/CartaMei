﻿using CartaMei.Common;
using CartaMei.Tools;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CartaMei.MainPlugin
{
    internal static class Buttons
    {
        #region Constants

        private const string MapExtension = "cmm";
        private const string MapExtensionFilter = "CartaMei Maps (*.cmm)|*.cmm";

        #endregion

        #region Static Constructor

        static Buttons()
        {
            NewMap = new ButtonModel()
            {
                Name = "New Map",
                IsEnabled = true,
                Shortcut = new KeyGesture(Key.N, ModifierKeys.Control)
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
                Name = "Open Map",
                IsEnabled = true,
                Shortcut = new KeyGesture(Key.O, ModifierKeys.Control)
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
                Name = "Save Map",
                IsEnabled = isSaveEnabled(),
                Shortcut = new KeyGesture(Key.S, ModifierKeys.Control)
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

            SaveMapAs = new ButtonModel()
            {
                Name = "Save Map As",
                IsEnabled = isSaveEnabled()
            };
            SaveMapAs.Click += onSaveMapAs;

            Options = new ButtonModel()
            {
                Name = "Options",
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
                    SaveMap,
                    SaveMapAs
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
        internal static readonly ButtonModel SaveMapAs;
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
            System.Windows.Window dialog = null;// TODO
            dialog.Owner = Utils.MainWindow;
            dialog.ShowDialog();
        }

        private static void onOpenMap(object sender, EventArgs args)
        {
            var openDialog = new OpenFileDialog()
            {
                AddExtension = true,
                CheckFileExists = true,
                DefaultExt = "*." + MapExtension,
                Filter = MapExtensionFilter,
                Multiselect = false,
                RestoreDirectory = true,
                Title = "Open CartaMei Map"
            };
            if (openDialog.ShowDialog() == true)
            {
                // TODO: open file
            }
        }

        private static void onSaveMap(object sender, EventArgs args)
        {
            if (System.IO.File.Exists(Current.Map.FileName))
            {
                // TODO: save file
            }
            else
            {
                onSaveMapAs(sender, args);
            }
        }

        private static void onSaveMapAs(object sender, EventArgs args)
        {
            var saveDialog = new SaveFileDialog()
            {
                AddExtension = true,
                CheckPathExists = true,
                FileName = Current.Map.FileName ?? Current.Map.Name,
                DefaultExt = "*." + MapExtension,
                Filter = MapExtensionFilter,
                OverwritePrompt = true,
                RestoreDirectory = true,
                Title = "Save CartaMei Map"
            };
            
            if (saveDialog.ShowDialog() == true)
            {
                Current.Map.FileName = saveDialog.FileName;
                onSaveMap(sender, args);
            }
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
