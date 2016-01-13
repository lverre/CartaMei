using CartaMei.Common;
using CartaMei.Models;
using CartaMei.Tools;
using CartaMei.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows;

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
                Shortcut = new KeyGesture(Key.S, ModifierKeys.Control)
            };
            SaveMap.Click += onSaveMap;
            SaveMapTool = new ButtonModel()
            {
                Name = "Save Map"
            };
            SaveMapTool.Click += onSaveMap;

            SaveMapAs = new ButtonModel()
            {
                Name = "Save Map As"
            };
            SaveMapAs.Click += onSaveMapAs;

            Current.MapChanged += delegate (object sender, EventArgs e)
            {
                var map = Current.Map as INotifyPropertyChanged;
                if (map != null)
                {
                    map.PropertyChanged += delegate(object sender2, PropertyChangedEventArgs e2)
                    {
                        if (e2.PropertyName == nameof(IMap.IsDirty))
                        {
                            updateSaveEnabled();
                        }
                    };
                }
                updateSaveEnabled();
            };
            updateSaveEnabled();

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

        private static bool saveDirty()
        {
            if (Current.Map?.IsDirty == true)
            {
                var result = MessageBox.Show("The current map has been modified since it was last changed. If you continue, you will lose your changes.\nDo you wish to save it?", "Unsaved Map", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
                switch (result)
                {
                    case MessageBoxResult.Cancel:
                    case MessageBoxResult.None:
                    default:
                        return false;
                    case MessageBoxResult.No:
                        return true;
                    case MessageBoxResult.Yes:
                    case MessageBoxResult.OK:
                        onSaveMap(null, null);
                        return true;
                }
            }
            else
            {
                return true;
            }
        }

        private static void onNewMap(object sender, EventArgs args)
        {
            if (!saveDirty()) return;

            var model = new NewMapModel();
            var dialog = new NewMap()
            {
                Owner = Utils.MainWindow,
                DataContext = model
            };
            if (dialog.ShowDialog() == true)
            {
                Utils.Current.SetMap(model.CreateMap());
            }
        }

        private static void onOpenMap(object sender, EventArgs args)
        {
            if (!saveDirty()) return;

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

        private static void updateSaveEnabled()
        {
            var map = Current.Map;
            var isSaveEnabled = map != null && (map.IsDirty || map.FileName == null);
            SaveMap.IsEnabled = isSaveEnabled;
            SaveMapTool.IsEnabled = isSaveEnabled;
            SaveMapAs.IsEnabled = isSaveEnabled;
        }

        private static void onOptions(object sender, EventArgs args)
        {
        }

        #endregion
    }
}
