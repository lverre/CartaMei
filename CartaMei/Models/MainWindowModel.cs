using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CartaMei.Common;
using System.Windows;

namespace CartaMei.Models
{
    public class MainWindowModel : NotifyPropertyChangedBase
    {
        #region Constructor

        public MainWindowModel()
        {
            Current.MapChanged += delegate (CurrentPropertyChangedEventArgs<IMap> e1)
            {
                this.Document = e1.NewValue;
                
                var map = Current.Map as INotifyPropertyChanged;
                if (map != null)
                {
                    map.PropertyChanged += delegate (object s2, PropertyChangedEventArgs e2)
                    {
                        switch (e2.PropertyName)
                        {
                            case nameof(IMap.FileName):
                            case nameof(IMap.IsDirty):
                            case nameof(IMap.Name):
                                updateTitle();
                                break;
                        }
                    };
                }
                updateTitle();
            };
            updateTitle();
        }

        #endregion

        #region Properties

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    onPropetyChanged();
                }
            }
        }

        private IEnumerable<IButtonModel> _menu;
        public IEnumerable<IButtonModel> Menu
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

        private IEnumerable<IButtonModel> _tools;
        public IEnumerable<IButtonModel> Tools
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

        private IEnumerable<object> _statusItems;
        public IEnumerable<object> StatusItems
        {
            get { return _statusItems; }
            set
            {
                if (_statusItems != value)
                {
                    _statusItems = value;
                    onPropetyChanged();
                }
            }
        }

        private IMap _document;
        public IMap Document
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
        public IEnumerable<IMap> Documents { get { return new IMap[] { this.Document }; } }

        private ObservableCollection<IAnchorableTool> _anchorables;
        public ObservableCollection<IAnchorableTool> Anchorables
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

        #region Tools

        private void updateTitle()
        {
            var title = "Carta Mei";
            if (this.Document != null)
            {
                title += " - " + this.Document.Name;
                if (this.Document.IsDirty)
                {
                    title += "*";
                }
            }
            this.Title = title;
        }

        #endregion
    }
}
