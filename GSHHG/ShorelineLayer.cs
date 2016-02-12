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

        private Canvas _container = null;

        #endregion

        #region Constructor

        public ShorelineLayer(IMap map)
        {
            _map = map;

            _container = new Canvas();
            _container.MouseMove += mouseMove;

            this.ShorelinesThickness = PluginSettings.Instance.ShorelinesThickness;
            this.ShorelinesBrush = PluginSettings.Instance.ShorelinesBrush;
            this.ShorelinesWaterFill = PluginSettings.Instance.ShorelinesWaterFill;
            this.ShorelinesLandFill = PluginSettings.Instance.ShorelinesLandFill;
            this.UseAutoResolution = PluginSettings.Instance.UseAutoResolution;
            this.Resolution = PluginSettings.Instance.Resolution;

            this.Name = ShorelineLayer.LayerName;
            this.Items = new ObservableCollection<IMapObject>();
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
                    redraw();
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
                    redraw();
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
                    redraw();
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
                    redraw();
                    onPropetyChanged();
                }
            }
        }

        private bool _useAutoResolution;
        [Description("When enabled, this feature will use the best resolution given the map boundaries.")]
        [DisplayName("Auto Resolution")]
        [PropertyOrder(4)]
        public bool UseAutoResolution
        {
            get { return _useAutoResolution; }
            set
            {
                if (value != _useAutoResolution)
                {
                    _useAutoResolution = value;
                    this.Resolution = getResolution(_map.Boundaries);
                    onPropetyChanged();
                }
            }
        }

        private Resolution _resolution;
        [Description("The resolution to use.")]
        [DisplayName("Resolution")]
        [PropertyOrder(5)]
        public Resolution Resolution
        {
            get { return _resolution; }
            set
            {
                if (value != _resolution)
                {
                    _resolution = value;
                    redraw();
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
                case RedrawType.Resize:
                case RedrawType.Translation:
                case RedrawType.Zoom:
                    break;
                default:
                    // We don't handle those
                    return;
            }

            resetMapData(context);
            
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
                            updatePolygonPoints(polygonObject, context);
                        }
                        else
                        {
                            var visualPolygon = new System.Windows.Shapes.Path()
                            {
                                Stroke = this.ShorelinesBrush,
                                StrokeThickness = this.ShorelinesThickness,
                                Fill = item.Value.Header.IsLand ? this.ShorelinesLandFill : this.ShorelinesWaterFill,
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
                            // If we add it to the children, the objects panel will get very crowded and slow
                            // this.Items.Add(polygonObject);
                        }
                    }
                }
            }

            removeChildren(toRemove);
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

        private void redraw()
        {
            this.Draw(new DrawContext(_container, RedrawType.Reset, _map));
        }

        private void resetMapData(IDrawContext context)
        {
            var resolution = getResolution(context.Boundaries);
            var mapsDir = PluginSettings.Instance.MapsDirectory;
            if (resolution != this.Resolution || mapsDir != _mapsDir)
            {
                if (_polygonObjects != null)
                {
                    removeChildren(new List<int>(_polygonObjects.Keys));
                }

                this.Resolution = resolution;
                _mapsDir = mapsDir;

                Utils.Instance.SetStatus("Loading map...", true, true);
                var reader = new GSHHG2Reader();
                _polygons = reader.Read(PluginSettings.Instance.MapsDirectory, PolygonType.ShoreLine, Resolution.Crude).Polygons;
                Utils.Instance.HideStatus();
                _polygonObjects = new Dictionary<int, ShorelinePolygonObject>();
            }
        }

        private Resolution getResolution(LatLonBoundaries boundaries)
        {
            if (this.UseAutoResolution)
            {
                return Resolution.Crude;// TODO: calc from boundaries
            }
            else
            {
                return this.Resolution;
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
                //this.Items.Remove(item);
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
        public System.Windows.Shapes.Shape VisualShape { get; set; }

        #endregion
    }
}
