using CartaMei.Common;
using System.ComponentModel;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace CartaMei.GSHHG
{
    public class RiverLayer : AGshhgLayer<PolygonObject>
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
            this.Items = null;
        }

        #endregion
        
        #region AGshhgLayer

        public override PolygonType PolygonType { get { return PolygonType.River; } }

        protected override bool ClosePolygons { get { return false; } }

        protected override IGshhgContainer<PolygonObject> getNewContainer()
        {
            return new GdiGshhgContainer<PolygonObject>(this);
        }

        protected override PolygonObject getNewPolygonObject(int id, IPolygon<GSHHG2PolygonHeader, LatLonCoordinates> polygon)
        {
            return new PolygonObject()
            {
                Id = id,
                Polygon = polygon,
                IsActive = true,
                Layer = this,
                Name = "River #" + id
            };
        }

        protected override void updateVisual(PolygonObject item)
        {
            item.Fill = null;
        }

        #endregion
    }
}
