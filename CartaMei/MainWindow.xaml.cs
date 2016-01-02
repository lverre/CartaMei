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
        private MapModel _document;

        #endregion

        #region Constructor

        public MainWindow()
        {
            _model = new Models.MainWindowModel();
            rebuildModel();

            InitializeComponent();

            this.DataContext = _model;
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
            _model.Menu = new ButtonModel[]
            {
                new ButtonModel()
                {
                    Name = "_File",
                    IsEnabled = true,
                    Children = new System.Collections.ObjectModel.ObservableCollection<ButtonModel>()
                    {
                        new ButtonModel() { Name = "New", IsEnabled = true },
                        new ButtonModel() { Name = "Open", IsEnabled = false },
                        new ButtonModel() { Name = "Save", IsEnabled = true },
                        new ButtonModel() { IsSeparator = true },
                        new ButtonModel() { Name = "New Layer", IsEnabled = true }
                    }
                },
                new ButtonModel() { Name = "_Edit", IsEnabled = true },
                new ButtonModel() { Name = "_View", IsEnabled = true },
                new ButtonModel() { Name = "_Tools", IsEnabled = true },
                new ButtonModel() { Name = "_Help", IsEnabled = true }
            };
        }

        private void rebuildToolbar()
        {
            _model.Tools = new ButtonModel[]
            {
                new ButtonModel() { Name = "New", IsEnabled = true },
                new ButtonModel() { Name = "Open", IsEnabled = false },
                new ButtonModel() { Name = "Save", IsEnabled = true },
                new ButtonModel() { IsSeparator = true },
                new ButtonModel() { Name = "New Layer", IsEnabled = true }
            };
        }

        private void rebuildDocument()
        {
            _document = new MapModel() { Title = "My Map" };
            _model.Document = _document;
        }

        private void rebuildAnchorables()
        {
            _model.Anchorables = new System.Collections.ObjectModel.ObservableCollection<IToolPanelModel>()
            {
                new LayersPanelModel(),
                new PropertiesPanelModel() 
            };
        }

        #endregion
    }
}
