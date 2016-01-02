using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartaMei.Models
{
    public class LayersPanelModel : IToolPanelModel
    {
        #region IToolPanelModel

        public string Title
        {
            get { return "Layers"; }
        }

        #endregion
    }
}
