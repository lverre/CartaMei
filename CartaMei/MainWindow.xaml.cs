﻿using CartaMei.Common;
using CartaMei.Models;
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
        private MapModel _map;
        private LayersPanelModel _layers;
        private PropertiesPanelModel _properties;

        #endregion

        #region Constructor

        public MainWindow()
        {
            _model = new Models.MainWindowModel();
            rebuildModel();

            InitializeComponent();

            this.DataContext = _model;

            PluginManager.Instance.Reload();
            rebuildModel();
        }

        #endregion

        #region Tools

        private void rebuildModel()
        {
            rebuildMenu();
            rebuildToolbar();
            rebuildDocument();
            rebuildAnchorables();
        }

        private void rebuildMenu()
        {
            _model.Menu = PluginManager.Instance.Menus;
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

        private void rebuildDocument()
        {
            _map = new MapModel()
            {
                Name = "My Map",
                OLayers = new System.Collections.ObjectModel.ObservableCollection<ILayer>()
                {
                    // TODO: test with two background layers
                }
            };
            _map.ActiveObject = _map;
            _model.Document = _map;

        }

        private void rebuildAnchorables()
        {
            _layers = new LayersPanelModel() { Map = _map };
            _properties = new PropertiesPanelModel() { Map = _map };
            _model.Anchorables = new System.Collections.ObjectModel.ObservableCollection<IToolPanelModel>()
            {
                _layers,
                _properties
            };
        }

        #endregion
    }
}
