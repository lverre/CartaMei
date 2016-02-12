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
            EventHandler mapChanged = delegate (object sender, EventArgs e)
            {
                _map = Current.Map;
                _items = new object[] { _map };
                onPropetyChanged(nameof(ObjectsPanelModel.Items));
            };
            Current.MapChanged += mapChanged;
            mapChanged(this, null);
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
