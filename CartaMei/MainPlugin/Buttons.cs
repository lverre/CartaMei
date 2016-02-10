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

            AddLayer = new ButtonModel()
            {
                Name = "Add Layer",
                IsEnabled = true
            };
            AddLayerTool = new ButtonModel()
            {
                Name = "Add Layer"
            };

            RemoveLayer = new ButtonModel()
            {
                Name = "Remove Layer"
            };
            AddLayer.Click += onRemoveLayer;
            RemoveLayerTool = new ButtonModel()
            {
                Name = "Remove Layer"
            };
            RemoveLayerTool.Click += onRemoveLayer;

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
                updateMapChanged();
                updateSaveEnabled();
            };
            updateMapChanged();
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

            Layers = new ButtonModel()
            {
                Name = "_Layers",
                IsEnabled = true,
                Children = new System.Collections.ObjectModel.ObservableCollection<IButtonModel>()
                {
                    AddLayer,
                    RemoveLayer
                }
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

            PluginManager.Instance.Reloaded += pluginsReloaded;
        }

        #endregion

        #region Menus

        internal static readonly ButtonModel NewMap;
        internal static readonly ButtonModel OpenMap;
        internal static readonly ButtonModel SaveMap;
        internal static readonly ButtonModel SaveMapAs;
        internal static readonly ButtonModel AddLayer;
        internal static readonly ButtonModel RemoveLayer;
        internal static readonly ButtonModel Options;

        internal static readonly ButtonModel File;
        internal static readonly ButtonModel Edit;
        internal static readonly ButtonModel View;
        internal static readonly ButtonModel Layers;
        internal static readonly ButtonModel Tools;
        internal static readonly ButtonModel Help;

        #endregion

        #region Toolbar

        internal static readonly ButtonModel NewMapTool;
        internal static readonly ButtonModel OpenMapTool;
        internal static readonly ButtonModel SaveMapTool;
        internal static readonly ButtonModel AddLayerTool;
        internal static readonly ButtonModel RemoveLayerTool;
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
                (Current.Map as MapModel).ActiveObject = Current.Map;
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

        private static void updateMapChanged()
        {
            var hasMap = Current.Map != null;
            AddLayer.IsEnabled = hasMap;
            AddLayerTool.IsEnabled = hasMap;
        }

        private static void updateSaveEnabled()
        {
            var map = Current.Map;
            var isSaveEnabled = map != null && (map.IsDirty || map.FileName == null);
            SaveMap.IsEnabled = isSaveEnabled;
            SaveMapTool.IsEnabled = isSaveEnabled;
            SaveMapAs.IsEnabled = isSaveEnabled;
        }
        
        private static void onRemoveLayer(object sender, EventArgs args)
        {
        }

        private static void onOptions(object sender, EventArgs args)
        {
        }

        #endregion

        #region Event Handlers

        private static void pluginsReloaded(object sender, EventArgs e)
        {
            AddLayer.Children = new System.Collections.ObjectModel.ObservableCollection<IButtonModel>();
            AddLayerTool.Children = new System.Collections.ObjectModel.ObservableCollection<IButtonModel>();
            var layersProviders = PluginManager.Instance.LayerProviders;
            foreach (var lp in layersProviders)
            {
                AddLayer.Children.Add(getButton(lp));
                AddLayerTool.Children.Add(getButton(lp));
            }
        }

        private static IButtonModel getButton(PluginItemProvider<ILayer> lp)
        {
            var button = new ButtonModel()
            {
                Name = lp.Name,
                Description = lp.Description,
                IsEnabled = true
            };
            button.Click += delegate (object sender, EventArgs e)
            {
                Current.Map.Layers.Add(lp.Create(Current.Map));
            };
            return button;
        }
        
        #endregion
    }
}
