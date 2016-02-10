using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartaMei.Common
{
    public interface IMapObject : INotifyPropertyChanged
    {
        #region Properties

        string Name { get; set; }

        ICollection<IMapObject> Items { get; }

        ILayer Layer { get; set; }

        IEnumerable<IButtonModel> ContextMenu { get; }

        object Properties { get; }

        bool ShowOnDesign { get; }

        bool ShowOnExport { get; }

        bool IsActive { get; set; }

        #endregion
    }
    
    public class MapObject : NotifyPropertyChangedBase, IMapObject
    {
        #region Properties

        private string _name;
        [Category("General")]
        [Description("The name of the object.")]
        [DisplayName("Name")]
        public virtual string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    onPropetyChanged();
                }
            }
        }

        private ICollection<IMapObject> _items;
        [Browsable(false)]
        public virtual ICollection<IMapObject> Items
        {
            get { return _items; }
            set
            {
                if (value != _items)
                {
                    _items = value;
                    onPropetyChanged();
                }
            }
        }

        private ILayer _layer;
        [Browsable(false)]
        public virtual ILayer Layer
        {
            get { return _layer; }
            set
            {
                if (value != _layer)
                {
                    _layer = value;
                    onPropetyChanged();
                }
            }
        }

        [Browsable(false)]
        public virtual IEnumerable<IButtonModel> ContextMenu { get { return null; } }

        [Browsable(false)]
        public virtual object Properties { get { return this; } }

        [Category("General")]
        [Description("When on, this means that the object will appear on the design canvas.")]
        [DisplayName("Show On Design")]
        public virtual bool ShowOnDesign { get { return true; } }

        [Category("General")]
        [Description("When on, this means that the object will appear on the exported document.")]
        [DisplayName("Show On Export")]
        public virtual bool ShowOnExport { get { return true; } }

        private bool _isActive;
        [Category("General")]
        [Description("When off, this means that the object will not appear at all.")]
        [DisplayName("Is Active")]
        public virtual bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (value != _isActive)
                {
                    _isActive = value;
                    onPropetyChanged();
                }
            }
        }

        #endregion
    }

    public class MapLeafObject : NotifyPropertyChangedBase, IMapObject
    {
        #region Properties

        private string _name;
        [Category("General")]
        [Description("The name of the object.")]
        [DisplayName("Name")]
        public virtual string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    onPropetyChanged();
                }
            }
        }

        [Browsable(false)]
        public virtual ICollection<IMapObject> Items { get { return null; } }

        private ILayer _layer;
        [Browsable(false)]
        public virtual ILayer Layer
        {
            get { return _layer; }
            set
            {
                if (value != _layer)
                {
                    _layer = value;
                    onPropetyChanged();
                }
            }
        }

        [Browsable(false)]
        public virtual IEnumerable<IButtonModel> ContextMenu { get { return null; } }

        [Browsable(false)]
        public virtual object Properties { get { return this; } }
        
        [Category("General")]
        [Description("When on, this means that the object will appear on the design canvas.")]
        [DisplayName("Show On Design")]
        public virtual bool ShowOnDesign { get { return true; } }
        
        [Category("General")]
        [Description("When on, this means that the object will appear on the exported document.")]
        [DisplayName("Show On Export")]
        public virtual bool ShowOnExport { get { return true; } }

        private bool _isActive;
        [Category("General")]
        [Description("When off, this means that the object will not appear at all.")]
        [DisplayName("Is Active")]
        public virtual bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (value != _isActive)
                {
                    _isActive = value;
                    onPropetyChanged();
                }
            }
        }

        #endregion
    }
}
