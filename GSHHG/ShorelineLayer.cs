﻿using CartaMei.Common;
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
        {
            _map = map;
            _loadedResolution = (Resolution)int.MaxValue;// Invalid value

            _container = new ShorelineContainer(this);
            _container.MouseMove += mouseMove;

            _shorelinesThickness = PluginSettings.Instance.ShorelinesThickness;
            _shorelinesBrush = PluginSettings.Instance.ShorelinesBrush;
            _shorelinesWaterFill = PluginSettings.Instance.ShorelinesWaterFill;
            _shorelinesLandFill = PluginSettings.Instance.ShorelinesLandFill;
            _resolution = PluginSettings.Instance.Resolution;
            _useCurvedLines = PluginSettings.Instance.UseCurvedLines;

            this.Name = ShorelineLayer.LayerName;
            this.Items = null;
            
            redraw(true);
        }

        #endregion

        #region Properties

        private double _shorelinesThickness;
        [Description("The thickness of the contour of the shorelines.")]
        [DisplayName("Shorelines Thickness")]
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
        [PropertyOrder(1)]
        public Brush ShorelinesBrush
        {
            get { return _shorelinesBrush; }
            set
            {
                if (_shorelinesBrush != value)
                {
                    _shorelinesBrush = value;
                    redraw(false);
                    onPropetyChanged();
                }
            }
        }

        private Brush _shorelinesWaterFill;
        [Description("The brush used to fill water areas.")]
        [DisplayName("Water Fill")]
        [PropertyOrder(2)]
        public Brush ShorelinesWaterFill
        {
            get { return _shorelinesWaterFill; }
            set
            {
                if (_shorelinesWaterFill != value)
                {
                    _shorelinesWaterFill = value;
                    _container.Background = value;
                    redraw(false);
                    onPropetyChanged();
                }
            }
        }

        private Brush _shorelinesLandFill;
        [Description("The brush used to fill land areas.")]
        [DisplayName("Land Fill")]
        [PropertyOrder(3)]
        public Brush ShorelinesLandFill
        {
            get { return _shorelinesLandFill; }
            set
            {
                if (_shorelinesLandFill != value)
                {
                    _shorelinesLandFill = value;
                    redraw(false);
                    onPropetyChanged();
                }
            }
        }
        
        private Resolution _resolution;
        [Description("The resolution to use.")]
        [DisplayName("Resolution")]
        [PropertyOrder(4)]
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
        [PropertyOrder(5)]
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

        #region ILayer

        public override UIElement CreateContainer()
        {
            return _container;
        }
        
        public override void Draw(IDrawContext context)
        {
            switch (context.RedrawType)
            {
                case RedrawType.Reset:
                case RedrawType.Redraw:
                case RedrawType.Resize:
                case RedrawType.Translation:
                case RedrawType.Zoom:
                    break;
                default:
                    // We don't handle those
                    return;
            }

            resetMapData(context);

            if (context.RedrawType != RedrawType.Redraw)
            {
                lock (_drawLocker)
                {
                    var toRemove = new List<int>();
                    if (_polygonObjects != null) toRemove.AddRange(_polygonObjects.Keys);

                    if (context.RedrawType == RedrawType.Reset)
                    {
                        removeChildren(toRemove);
                        toRemove.Clear();
                    }

                    if (context.RedrawType == RedrawType.Reset || context.RedrawType == RedrawType.Resize)
                    {
                        _container.Width = _map.Size.Width;
                        _container.Height = _map.Size.Height;
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
                                updatePolygonPoints(polygonObject, context);
                            }
                        }
                    }

                    removeChildren(toRemove);

                    _container.Polygons = new Dictionary<int, ShorelinePolygonObject>(_polygonObjects);
                }
            }

            switch (context.RedrawType)
            {
                case RedrawType.Reset:
                case RedrawType.Redraw:
                case RedrawType.Resize:
                case RedrawType.Zoom:
                    _container.InvalidateVisual();
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

        private void redraw(bool reset)
        {
            this.Draw(new DrawContext(_container, reset ? RedrawType.Reset : RedrawType.Redraw, _map));
        }

        private void resetMapData(IDrawContext context)
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
                    var reader = new GSHHG2Reader();
                    _polygons = reader.Read(PluginSettings.Instance.MapsDirectory, PolygonType.ShoreLine, this.Resolution).Polygons;
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

        private void updatePolygonPoints(ShorelinePolygonObject polygonObject, IDrawContext context)
        {
            var projection = context.Projection;
            var points = polygonObject.Polygon.Points?.Select(point => (Point)projection.LatLonToPixel(point));

            PathSegment segment;
            if (this.UseCurvedLines)
            {
                segment = new PolyBezierSegment() { Points = new PointCollection(points) };
            }
            else
            {
                segment = new PolyLineSegment() { Points = new PointCollection(points) };
            }
            segment.IsSmoothJoin = true;
            segment.IsStroked = true;

            polygonObject.Geometry = new PathGeometry(new PathFigureCollection()
            {
                new PathFigure()
                {
                    IsClosed = true,
                    IsFilled = true,
                    Segments = new PathSegmentCollection() { segment },
                    StartPoint = points.First()
                }
            });
        }
        
        #endregion
    }

    public class ShorelineContainer : UserControl
    {
        private ShorelineLayer _layer;

        public ShorelineContainer(ShorelineLayer layer)
        {
            _layer = layer;
        }

        private readonly object _polygonsLocker = new object();
        private IDictionary<int, ShorelinePolygonObject> _polygons;
        public IDictionary<int, ShorelinePolygonObject> Polygons
        {
            get { return _polygons; }
            set
            {
                lock (_polygonsLocker)
                {
                    _polygons = value;
                }
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            var pen = new Pen(_layer.ShorelinesBrush, _layer.ShorelinesThickness);
            var isCurve = _layer.UseCurvedLines;

            List<ShorelinePolygonObject> polygonsObjects;
            lock (_polygonsLocker)
            {
                polygonsObjects = new List<ShorelinePolygonObject>(this.Polygons.Values);
            }

            foreach (var item in polygonsObjects)
            {
                var fill = item.Polygon.Header.IsLand ? _layer.ShorelinesLandFill : _layer.ShorelinesWaterFill;
                drawingContext.DrawGeometry(fill, pen, item.Geometry);
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
        public PathGeometry Geometry { get; set; }

        #endregion
    }
}
