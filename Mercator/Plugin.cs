﻿using CartaMei.Common;
using System.Collections.Generic;

namespace CartaMei.Mercator
{
    [Plugin(License = "MIT")]
    public class MercatorPlugin : PluginBase
    {
        #region Constants

        private static readonly PluginItemProvider<IProjection> _provider = new PluginItemProvider<IProjection>()
        {
            Name = MercatorProjection.ProjectionName,
            Description = MercatorProjection.ProjectionDescription,
            Create = delegate() { return new MercatorProjection() { UseEllipsoid = true }; }
        };

        #endregion

        #region IPlugin

        public override IEnumerable<PluginItemProvider<IProjection>> ProjectionProviders
        {
            get
            {
                yield return _provider;
                yield break;
            }
        }
        
        #endregion
    }
}
