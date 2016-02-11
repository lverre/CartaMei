using CartaMei.Common;
using CartaMei.GSHHG;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CartaMei.GSHHG
{
    public class ShorelineLayer : ALayer
    {
        #region Constants

        public const string LayerName = "Shorelines";
        public const string LayerDescription = "Shorelines from GSHHG data";

        #endregion

        #region Fields

        private IMap _map;

        private string _mapsDir = null;
        
        private IDictionary<int, IPolygon<GSHHG2PolygonHeader, LatLonCoordinates>> _polygons = null;

        private IDictionary<int, ShorelinePolygonObject> _polygonObjects = null;

        private Canvas _container = new Canvas();

        #endregion

        #region Constructor

        public ShorelineLayer(IMap map)
        {
            _map = map;
        }

        #endregion

        #region Properties

        private Resolution _resolution;
        [Description("The resolution to use.")]
        [DisplayName("Resolution")]
        public Resolution Resolution
        {
            get { return _resolution; }
            set
            {
                if (value != _resolution)
                {
                    _resolution = value;
                    onPropetyChanged();
                }
            }
        }

        private bool _useAutoResolution;
        [Description("When enabled, this feature will use the best resolution given the map boundaries.")]
        [DisplayName("Auto Resolution")]
        public bool UseAutoResolution
        {
            get { return _useAutoResolution; }
            set
            {
                if (value != _useAutoResolution)
                {
                    _useAutoResolution = value;
                    onPropetyChanged();
                }
            }
        }

        #endregion

        #region ILayer

        public override UIElement CreateContainer()
        {
            return _container;
        }

        public override void Draw(IDrawContext context)
        {
            resetMapData(context);

            if (this.Root == null)
            {
                this.Root = new MapObject()
                {
                    IsActive = true,
                    Layer = this,
                    Name = "Shorelines",
                    Items = new ObservableCollection<IMapObject>()
                };
            }

            var toRemove = new List<int>();
            if (_polygonObjects != null) toRemove.AddRange(_polygonObjects.Keys);

            if (context.RedrawType == RedrawType.Reset)
            {
                removeChildren(toRemove);
                toRemove.Clear();
            }

            if (_polygons != null)
            {
                foreach (var item in _polygons)
                {
                    var id = item.Key;
                    var polygonObject = _polygonObjects.ContainsKey(id) ? _polygonObjects[id] : null;
                    if (item.Value.Intersects(context.Boundaries, context.Projection))
                    {
                        if (polygonObject != null)
                        {
                            toRemove.Remove(id);
                            updatePolygonPoints(polygonObject, context);
                        }
                        else
                        {
                            var visualPolygon = new System.Windows.Shapes.Path()
                            {
                                Stroke = PluginSettings.Instance.ShorelinesBrush,
                                StrokeThickness = PluginSettings.Instance.ShorelinesThickness,
                                Fill = item.Value.Header.IsLand ? PluginSettings.Instance.ShorelinesLandFill : PluginSettings.Instance.ShorelinesWaterFill,
                                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                                VerticalAlignment = System.Windows.VerticalAlignment.Top
                            };
                            _container.Children.Add(visualPolygon);
                            polygonObject = new ShorelinePolygonObject()
                            {
                                Id = id,
                                Polygon = item.Value,
                                IsActive = true,
                                Layer = this,
                                Name = "Polygon #" + id,
                                VisualShape = visualPolygon
                            };
                            updatePolygonPoints(polygonObject, context);
                            _polygonObjects.Add(id, polygonObject);
                            this.Root.Items.Add(polygonObject);
                        }
                    }
                }
            }

            removeChildren(toRemove);
        }

        #endregion

        #region Tools

        private void resetMapData(IDrawContext context)
        {
            var resolution = this.UseAutoResolution
                ? getResolution(context.Boundaries)
                : this.Resolution;
            var mapsDir = PluginSettings.Instance.MapsDirectory?.FullName;
            if (resolution != this.Resolution || mapsDir != _mapsDir)
            {
                if (_polygonObjects != null)
                {
                    removeChildren(new List<int>(_polygonObjects.Keys));
                }

                this.Resolution = resolution;
                _mapsDir = mapsDir;

                var reader = new GSHHG2Reader();
                _polygons = reader.Read(PluginSettings.Instance.MapsDirectory.FullName, PolygonType.ShoreLine, Resolution.Crude).Polygons;
                _polygonObjects = new Dictionary<int, ShorelinePolygonObject>();
            }
        }

        private Resolution getResolution(LatLonBoundaries boundaries)
        {
            return Resolution.Crude;// TODO: calc from boundaries
        }

        private void removeChildren(IList<int> ids)
        {
            while (ids.Count > 0)
            {
                var index = ids.Count - 1;
                var id = ids[index];
                ids.RemoveAt(index);
                var item = _polygonObjects[id];
                _polygonObjects.Remove(id);
                this.Root.Items.Remove(item);
                _container.Children.Remove(item.VisualShape);
            }
        }

        private void updatePolygonPoints(ShorelinePolygonObject polygonObject, IDrawContext context)
        {
            var projection = context.Projection;
            var points = polygonObject.Polygon.Points?.Select(point => (Point)projection.LatLonToPixel(point));
            var polygon = polygonObject.VisualShape as System.Windows.Shapes.Polygon;
            if (polygon != null)
            {
                polygon.Points = new PointCollection(points);
            }
            else
            {
                var bezier = new PolyBezierSegment()
                {
                    IsSmoothJoin = true,// TODO: check what that means
                    IsStroked = true,// TODO: check what that means
                    Points = new PointCollection(points)
                };
                var path = polygonObject.VisualShape as System.Windows.Shapes.Path;
                path.Data = new PathGeometry(new PathFigureCollection()
                {
                    new PathFigure()
                    {
                        IsClosed = true,
                        IsFilled = true,// TODO: check what that means
                        Segments = new PathSegmentCollection() { bezier },
                        StartPoint = points.First()
                    }
                });
            }
        }

        #endregion
    }

    public class ShorelinePolygonObject : MapLeafObject
    {
        #region Properties

        [Category("GSHHG")]
        [Description("The ID of the polygon.")]
        [DisplayName("ID")]
        [ReadOnly(true)]
        public int Id { get; set; }

        [Browsable(false)]
        public IPolygon<GSHHG2PolygonHeader, LatLonCoordinates> Polygon { get; set; }

        [Browsable(false)]
        public System.Windows.Shapes.Shape VisualShape { get; set; }

        #endregion
    }
}
