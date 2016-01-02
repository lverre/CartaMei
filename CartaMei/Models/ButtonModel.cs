using CartaMei.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CartaMei.Models
{
    public class ButtonModel : NotifyPropertyChangedBase, ICommand
    {
        #region Constructor

        public ButtonModel()
        {
            this.Children = null;
            this.Description = null;
            this.Icon = null;
            this.IsCheckable = false;
            this.IsChecked = false;
            this.IsEnabled = null;
            this.IsSeparator = false;
            this.IsVisible = true;
            this.Name = null;
        }

        #endregion
        
        #region Properties

        #region Observable

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

        private ObservableCollection<ButtonModel> _children;
        public ObservableCollection<ButtonModel> Children
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

        #endregion

        #region Events

        public event EventHandler Click;

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
    }
}
