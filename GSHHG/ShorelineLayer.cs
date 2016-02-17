using CartaMei.Common;
using CartaMei.GSHHG;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using System;
using System.Windows.Shapes;
using System.Threading;

namespace CartaMei.GSHHG
{
    public class ShorelineLayer : ALayer
    {
        #region Constants

        public const string LayerName = "Shorelines";
        public const string LayerDescription = "Shorelines from GSHHG data";

        #endregion

        #region Fields
        
        private Resolution _loadedResolution;

        private readonly object _readerLocker = new object();

        private string _mapsDir = null;
        
        private IDictionary<int, IPolygon<GSHHG2PolygonHeader, LatLonCoordinates>> _polygons = null;

        private IDictionary<int, ShorelinePolygonObject> _polygonObjects = null;

        private ShorelineContainer _container = null;
        
        private readonly object _drawLocker = new object();

        #endregion

        #region Constructor

        public ShorelineLayer(IMap map)
            : base(map)
        {
            _loadedResolution = (Resolution)int.MaxValue;// Invalid value

            _shorelinesThickness = PluginSettings.Instance.ShorelinesThickness;
            _shorelinesBrush = PluginSettings.Instance.ShorelinesBrush;
            _waterFill = PluginSettings.Instance.ShorelinesWaterFill;
            _landFill = PluginSettings.Instance.ShorelinesLandFill;
            _useWaterForBackground = PluginSettings.Instance.ShorelinesUseWaterForBackground;
            _backgroundFill = PluginSettings.Instance.ShorelinesBackground;
            _resolution = PluginSettings.Instance.Resolution;
            _useCurvedLines = PluginSettings.Instance.UseCurvedLines;

            _container = new ShorelineContainer(this);
            _container.MouseMove += mouseMove;
            resetBackground();

            this.Name = ShorelineLayer.LayerName;
            this.Items = null;
        }

        #endregion

        #region Properties

        private double _shorelinesThickness;
        [Description("The thickness of the contour of the shorelines.")]
        [DisplayName("Shorelines Thickness")]
        [Category("Shorelines")]
        [PropertyOrder(0)]
        public double ShorelinesThickness
        {
            get { return _shorelinesThickness; }
            set
            {
                if (_shorelinesThickness != value)
                {
                    _shorelinesThickness = value;
                    redraw(false);
                    onPropetyChanged();
                }
            }
        }

        private Brush _shorelinesBrush;
        [Description("The brush used to draw the contour of the shorelines.")]
        [DisplayName("Shorelines Brush")]
        [Category("Shorelines")]
        [PropertyOrder(1)]
        public Brush ShorelinesBrush
        {
            get { return _shorelinesBrush; }
            set
            {
                if (value.CanFreeze) value.Freeze();
                if (_shorelinesBrush != value)
                {
                    _shorelinesBrush = value;
                    redraw(false);
                    onPropetyChanged();
                }
            }
        }

        private Brush _waterFill;
        [Description("The brush used to fill water areas.")]
        [DisplayName("Water Fill")]
        [Category("Shorelines")]
        [PropertyOrder(2)]
        public Brush WaterFill
        {
            get { return _waterFill; }
            set
            {
                if (value.CanFreeze) value.Freeze();
                if (_waterFill != value)
                {
                    _waterFill = value;
                    resetBackground();
                    redraw(false);
                    onPropetyChanged();
                }
            }
        }

        private Brush _landFill;
        [Description("The brush used to fill land areas.")]
        [DisplayName("Land Fill")]
        [Category("Shorelines")]
        [PropertyOrder(3)]
        public Brush LandFill
        {
            get { return _landFill; }
            set
            {
                if (value.CanFreeze) value.Freeze();
                if (_landFill != value)
                {
                    _landFill = value;
                    redraw(false);
                    onPropetyChanged();
                }
            }
        }

        private bool _useWaterForBackground;
        [Description("Choose this option if you want the background to always match that of the water.")]
        [DisplayName("Background Is Water")]
        [Category("Shorelines")]
        [PropertyOrder(4)]
        public bool UseWaterForBackground
        {
            get { return _useWaterForBackground; }
            set
            {
                if (_useWaterForBackground != value)
                {
                    _useWaterForBackground = value;
                    resetBackground();
                    redraw(false);
                    onPropetyChanged();
                }
            }
        }

        private Brush _backgroundFill;
        [Description("The brush used to fill the background.")]
        [DisplayName("Background")]
        [Category("Shorelines")]
        [PropertyOrder(5)]
        public Brush BackgroundFill
        {
            get { return _backgroundFill; }
            set
            {
                if (_backgroundFill != value)
                {
                    _backgroundFill = value;
                    resetBackground();
                    redraw(false);
                    onPropetyChanged();
                }
            }
        }

        private Resolution _resolution;
        [Description("The resolution to use.")]
        [DisplayName("Resolution")]
        [Category("Shorelines")]
        [PropertyOrder(6)]
        public Resolution Resolution
        {
            get { return _resolution; }
            set
            {
                if (value != _resolution)
                {
                    _resolution = value;
                    redraw(true);
                    onPropetyChanged();
                }
            }
        }

        private bool _useCurvedLines;
        [Description("When enabled, this feature display curved (bezier) lines instead of straight lines.")]
        [DisplayName("Curved Lines")]
        [Category("Shorelines")]
        [PropertyOrder(7)]
        public bool UseCurvedLines
        {
            get { return _useCurvedLines; }
            set
            {
                if (value != _useCurvedLines)
                {
                    _useCurvedLines = value;
                    redraw(true);
                    onPropetyChanged();
                }
            }
        }

        #endregion

        #region ALayer

        public override UIElement Container { get { return _container; } }

        public override void SetLayerAdded(int layerIndex)
        {
            if (layerIndex > 0) this.UseWaterForBackground = false;
        }

        public override void Draw(IDrawContext context, CancellationToken cancellation)
        {
            resetMapData(context, cancellation);
            
            if (cancellation.IsCancellationRequested) return;

            if (context.RedrawType != RedrawType.Redraw)
            {
                lock (_drawLocker)
                {
                    if (context.RedrawType == RedrawType.Scale && (_polygonObjects?.Any() ?? false))
                    {
                        foreach (var polygonObject in _polygonObjects.Values)
                        {
                            if (cancellation.IsCancellationRequested) return;

                            updatePoints(polygonObject, context.RedrawType, context.Transform);
                        }
                    }
                    else
                    {
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
                                if (cancellation.IsCancellationRequested) return;

                                var id = item.Key;
                                var polygonObject = _polygonObjects.ContainsKey(id) ? _polygonObjects[id] : null;
                                // TODO: lazy read the files and add a function: LoadIfIntersects(context.Boundaries, context.Projection)
                                if (item.Value.Intersects(context.Boundaries, context.Projection))
                                {
                                    if (polygonObject != null)
                                    {
                                        toRemove.Remove(id);
                                    }
                                    else
                                    {
                                        polygonObject = new ShorelinePolygonObject()
                                        {
                                            Id = id,
                                            Polygon = item.Value,
                                            IsActive = true,
                                            Layer = this,
                                            Name = "Polygon #" + id
                                        };
                                        _polygonObjects.Add(id, polygonObject);
                                    }
                                    updatePoints(polygonObject, context.RedrawType, context.Transform);
                                }
                            }
                        }

                        removeChildren(toRemove);

                        _container.Polygons = new Dictionary<int, ShorelinePolygonObject>(_polygonObjects);
                    }
                }
            }

            if (cancellation.IsCancellationRequested) return;

            switch (context.RedrawType)
            {
                case RedrawType.Reset:
                case RedrawType.Redraw:
                case RedrawType.Zoom:
                case RedrawType.Scale:
                    _container.DrawContext = context;
                    _container.IsDirty = true;
                    _container.SafeInvalidate();
                    break;
            }
        }

        public override IEnumerable<IMapObject> GetObjectsAt(Point at)
        {
            return new IMapObject[] { this };
        }

        #endregion

        #region Event Handlers

        private void mouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var point = e.GetPosition(_container.Parent as IInputElement);
            var latLon = _map.Projection.PixelToLatLon(point);
            Utils.Instance.SetStatus(latLon.ToString());
        }

        #endregion

        #region Tools

        private void resetBackground()
        {
            _container.Background = this.UseWaterForBackground ? this.WaterFill : this.BackgroundFill;
        }

        private void redraw(bool reset)
        {
            this.DrawAsync(new DrawContext(_container, reset ? RedrawType.Reset : RedrawType.Redraw, null, null, _map));
        }

        private void resetMapData(IDrawContext context, CancellationToken cancellation)
        {
            lock (_readerLocker)
            {
                var mapsDir = PluginSettings.Instance.MapsDirectory;
                if (this.Resolution != _loadedResolution || mapsDir != _mapsDir)
                {
                    if (_polygonObjects != null)
                    {
                        removeChildren(new List<int>(_polygonObjects.Keys));
                    }
                
                    _mapsDir = mapsDir;

                    Utils.Instance.SetStatus("Loading map...", true, true);
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    var reader = new GSHHG2Reader();
                    // TODO: lazy load
                    // TODO: handle cancellation
                    _polygons = reader.Read(PluginSettings.Instance.MapsDirectory, PolygonType.ShoreLine, this.Resolution).Polygons;
                    var time = watch.ElapsedMilliseconds;
                    Utils.Instance.HideStatus();
                    _polygonObjects = new Dictionary<int, ShorelinePolygonObject>();
                    _loadedResolution = this.Resolution;
                }
            }
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
            }
        }

        private void updatePoints(ShorelinePolygonObject polygonObject, RedrawType redrawType, Transform transform)
        {
            const RedrawType transformRedrawTypes = RedrawType.Translate | RedrawType.Scale | RedrawType.AnimationStepChanged | RedrawType.DisplayTypeChanged | RedrawType.Redraw;
            if ((redrawType & ~transformRedrawTypes) == RedrawType.None && (polygonObject.PixelPoints?.Any() ?? false))
            {
                foreach (var point in polygonObject.PixelPoints)
                {
                    var newPoint = transform.Transform(point);
                    point.X = newPoint.X;
                    point.Y = newPoint.Y;
                }
            }
            else
            {
                polygonObject.PixelPoints = getPoints(polygonObject);
            }
        }

        private IEnumerable<PixelCoordinates> getPoints(ShorelinePolygonObject polygonObject)
        {
            return polygonObject.Polygon.Points?.Select(point => _map.Projection.LatLonToPixel(point)).ToList();
        }

        #endregion
    }

    public class ShorelineContainer : FrameworkElement
    {
        private ShorelineLayer _layer;

        public ShorelineContainer(ShorelineLayer layer)
        {
            _layer = layer;
        }

        public Brush Background { get; set; }

        public bool IsDirty { get; set; }

        public IDrawContext DrawContext { get; set; }

        public IDictionary<int, ShorelinePolygonObject> Polygons { get; set; }

        private IDictionary<ShorelinePolygonObject, Geometry> Geometries { get; set; }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            var background = this.Background;
            if (background != null && background != Brushes.Transparent)
            {
                drawingContext.DrawRectangle(background, null, new Rect(0, 0, this.Width, this.Height));
            }

            var polygons = this.Polygons;
            if (polygons == null) return;

            if (this.IsDirty || this.Geometries == null)
            {
                var geometries = new Dictionary<ShorelinePolygonObject, Geometry>();

                foreach (var item in polygons)
                {
                    PathSegment segment;
                    var points = new PointCollection(item.Value.PixelPoints.Select(point => (Point)point));
                    if (_layer.UseCurvedLines)
                    {
                        segment = new PolyBezierSegment() { Points = points };
                    }
                    else
                    {
                        segment = new PolyLineSegment() { Points = points };
                    }
                    segment.IsSmoothJoin = true;
                    segment.IsStroked = true;

                    var geometry = new PathGeometry(new PathFigureCollection()
                    {
                        new PathFigure()
                        {
                            IsClosed = true,
                            IsFilled = true,
                            Segments = new PathSegmentCollection() { segment },
                            StartPoint = points.First()
                        }
                    });
                    geometry.Freeze();
                    geometries.Add(item.Value, geometry);
                }

                this.Geometries = geometries;
                this.IsDirty = false;
            }

            var pen = new Pen(_layer.ShorelinesBrush, _layer.ShorelinesThickness);
            pen.Freeze();
            var isCurve = _layer.UseCurvedLines;

            foreach (var item in this.Geometries)
            {
                var fill = item.Key.Polygon.Header.IsLand ? _layer.LandFill : _layer.WaterFill;
                drawingContext.DrawGeometry(fill, pen, item.Value);
            }
        }
    }

    public class ShorelinePolygonObject : MapLeafObject, ILayerItem
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
        public IEnumerable<PixelCoordinates> PixelPoints { get; internal set; }

        #endregion
    }
}
