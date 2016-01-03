using CartaMei.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CartaMei.Common
{
    public interface IButtonModel : INotifyPropertyChanged, ICommand
    {
        #region Properties
        
        string Name { get; }

        string Icon { get; }

        string Description { get; }

        bool IsCheckable { get; }

        bool IsChecked { get; }

        bool IsVisible { get; }

        bool? IsEnabled { get; }
        
        ObservableCollection<IButtonModel> Children { get; }

        bool IsSeparator { get; }
        
        #endregion

        #region Events

        event EventHandler Click;

        #endregion
    }

    public class ButtonModel : IButtonModel, INotifyPropertyChanged, ICommand
    {
        #region Constructor

        public ButtonModel()
        {
            this.IsVisible = true;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void onPropetyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region ICommand

        public event EventHandler CanExecuteChanged;

        protected virtual void onCanExecuteChanged()
        {
            this.CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public virtual bool CanExecute(object parameter)
        {
            return this.IsEnabled ?? false;
        }

        public virtual void Execute(object parameter)
        {
            this.Click?.Invoke(this, new EventArgs());
        }

        #endregion

        #region IButtonModel

        #region Properties
        
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    onPropetyChanged();
                }
            }
        }

        private string _icon;
        public string Icon
        {
            get { return _icon; }
            set
            {
                if (_icon != value)
                {
                    _icon = value;
                    onPropetyChanged();
                }
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    onPropetyChanged();
                }
            }
        }
        
        private bool _isCheckable;
        public bool IsCheckable
        {
            get { return _isCheckable; }
            set
            {
                if (_isCheckable != value)
                {
                    _isCheckable = value;
                    onPropetyChanged();
                }
            }
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    onPropetyChanged();
                }
            }
        }

        private bool _isVisible;
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    onPropetyChanged();
                }
            }
        }

        private bool? _isEnabled;
        public bool? IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    onPropetyChanged();
                }
            }
        }

        private ObservableCollection<IButtonModel> _children;
        public ObservableCollection<IButtonModel> Children
        {
            get { return _children; }
            set
            {
                if (_children != value)
                {
                    _children = value;
                    onPropetyChanged();
                }
            }
        }

        private bool _isSeparator;
        public bool IsSeparator
        {
            get { return _isSeparator; }
            set
            {
                if (_isSeparator != value)
                {
                    _isSeparator = value;
                    onPropetyChanged();
                }
            }
        }
        
        #endregion

        #region Events

        public event EventHandler Click;

        #endregion

        #endregion
    }
}
