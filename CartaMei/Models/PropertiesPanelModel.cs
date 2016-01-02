using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartaMei.Models
{
    public class PropertiesPanelModel : IToolPanelModel
    {
        #region IToolPanelModel

        public string Title
        {
            get { return "Properties"; }
        }

        #endregion
    }
}
