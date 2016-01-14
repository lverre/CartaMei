using CartaMei.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartaMei.Models
{
    public class LayersPanelModel : NotifyPropertyChangedBase, IAnchorableTool
    {
        #region Constructor

        public LayersPanelModel()
        {
            Current.MapChanged += delegate (object sender, EventArgs e)
            {
                onPropetyChanged(nameof(LayersPanelModel.Layers));
            };
        }
        
        #endregion

        #region IAnchorableTool

        public string Name
        {
            get { return "Layers"; }
        }

        #endregion

        #region Properties

        public ObservableCollection<ILayer> Layers { get { return Current.Map?.Layers; } }

        #endregion
    }
}
