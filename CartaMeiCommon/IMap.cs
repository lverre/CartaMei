using System.Collections.Generic;

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

        string Version { get; set; }

        #endregion

        #region Map

        IProjection Projection { get; set; }

        ICollection<ILayer> Layers { get; set; }

        #endregion

        #endregion
    }
}
