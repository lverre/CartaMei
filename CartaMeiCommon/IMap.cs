using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartaMei.Common
{
    public interface IMap
    {
        #region Properties

        #region Metadata

        string Name { get; set; }

        string Description { get; set; }

        string Author { get; set; }

        string CreatedOn { get; set; }

        string License { get; set; }

        string version { get; set; }

        #endregion

        #region Map

        IProjection Projection { get; set; }

        SortedSet<ILayer> Layers { get; set; }

        #endregion

        #endregion
    }
}
