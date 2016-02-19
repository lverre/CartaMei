using CartaMei.Common;

namespace CartaMei.GSHHG
{
    public class RiverLayer : AGshhgLayer
    {
        #region Constants

        public const string LayerName = "Rivers";
        public const string LayerDescription = "Rivers from GSHHG data";

        #endregion
        
        #region Constructor

        public RiverLayer(IMap map)
            : base(map)
        {
            this.StrokeBrush = PluginSettings.Instance.ShorelinesWaterFill.GetFrozenCopy();

            this.Name = RiverLayer.LayerName;
        }

        #endregion
        
        #region AGshhgLayer

        public override PolygonType PolygonType { get { return PolygonType.River; } }
        
        #endregion
    }
}
