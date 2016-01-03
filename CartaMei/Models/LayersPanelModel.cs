using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartaMei.Models
{
    public class LayersPanelModel : NotifyPropertyChangedBase, IToolPanelModel
    {
        #region IToolPanelModel

        public string Title
        {
            get { return "Layers"; }
        }

        private MapModel _map;
        public MapModel Map
        {
            get { return _map; }
            set
            {
                if (_map != value)
                {
                    _map = value;
                    onPropetyChanged();
                }
            }
        }

        #endregion
    }
}
