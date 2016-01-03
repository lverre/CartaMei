﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CartaMei.Common;

namespace CartaMei.Models
{
    public class MainWindowModel : NotifyPropertyChangedBase
    {
        #region Properties

        private IEnumerable<ButtonModel> _menu;
        public IEnumerable<ButtonModel> Menu
        {
            get { return _menu; }
            set
            {
                if (_menu != value)
                {
                    _menu = value;
                    onPropetyChanged();
                }
            }
        }

        private IEnumerable<ButtonModel> _tools;
        public IEnumerable<ButtonModel> Tools
        {
            get { return _tools; }
            set
            {
                if (_tools != value)
                {
                    _tools = value;
                    onPropetyChanged();
                }
            }
        }

        private MapModel _document;
        public MapModel Document
        {
            get { return _document; }
            set
            {
                if (_document != value)
                {
                    _document = value;
                    onPropetyChanged();
                    onPropetyChanged("Documents");
                }
            }
        }
        public IEnumerable<MapModel> Documents { get { return new MapModel[] { this.Document }; } }

        private ObservableCollection<IToolPanelModel> _anchorables;
        public ObservableCollection<IToolPanelModel> Anchorables
        {
            get { return _anchorables; }
            set
            {
                if (_anchorables != value)
                {
                    _anchorables = value;
                    onPropetyChanged();
                }
            }
        }

        #endregion
    }
}
