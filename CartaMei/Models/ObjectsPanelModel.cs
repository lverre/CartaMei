using CartaMei.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CartaMei.Models
{
    public class ObjectsPanelModel : NotifyPropertyChangedBase, IAnchorableTool
    {
        #region Fields

        private IMap _map;

        private IEnumerable<object> _items = null;
        
        #endregion

        #region Constructor

        public ObjectsPanelModel()
        {
            CurrentPropertyChangedEventHandler<IMap> mapChanged = delegate (CurrentPropertyChangedEventArgs<IMap> e)
            {
                _map = e.NewValue;
                _items = new object[] { _map };
                onPropetyChanged(nameof(ObjectsPanelModel.Items));
            };
            Current.MapChanged += mapChanged;
            mapChanged(new CurrentPropertyChangedEventArgs<IMap>(null, Current.Map));
        }
        
        #endregion

        #region IAnchorableTool

        public string Name
        {
            get { return "Objects"; }
        }

        #endregion

        #region Properties
        
        public IEnumerable<object> Items { get { return _items; } }

        #endregion
    }
}
