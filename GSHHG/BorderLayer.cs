﻿using CartaMei.Common;

namespace CartaMei.GSHHG
{
    public class BorderLayer : AGshhgLayer
    {
        #region Constants

        public const string LayerName = "Borders";
        public const string LayerDescription = "Borders from GSHHG data";

        #endregion

        #region Constructor

        public BorderLayer(IMap map)
            : base(map, BorderLayer.LayerName, PluginSettings.Instance.BorderThickness, PluginSettings.Instance.BorderBrush)
        { }

        #endregion

        #region AGshhgLayer

        public override PolygonType PolygonType { get { return PolygonType.Border; } }
        
        #endregion
    }
}
