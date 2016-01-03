using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartaMei.Common
{
    public interface IMapObject
    {
        #region Properties

        string Name { get; set; }

        IEnumerable<IMapObject> Items { get; }

        ILayer Layer { get; set; }

        IEnumerable<IButtonModel> ContextMenu { get; }

        object Properties { get; }

        bool ShowOnDesign { get; }

        bool ShowOnExport { get; }

        bool IsActive { set; }

        #endregion
    }
}
