using CartaMei.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CartaMei.WPF
{
    public class LayersContainer : Canvas
    {
        #region Fields

        private IMap _map;

        private LatLonBoundaries _boundaries;

        private ObservableCollection<ILayer> _layers;

        private IDictionary<ILayer, UIElement> _childrenDictionary;

        private readonly object _panLocker = new object();
        private Point _panStartPoint;
        private Point _panLastPoint;
        private bool _isMouseDownWheel;
        private bool _isPanning;

        #endregion
        
        #region Constructor

        public LayersContainer()
        {
            _map = null;
            _boundaries = null;
            _layers = null;
            _childrenDictionary = new Dictionary<ILayer, UIElement>();
            this.Background = Brushes.Transparent;// Needed to get the mouse events
        }

        #endregion

        #region FrameworkElement
        
        #region Events

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            PropertyChangedEventHandler mapPropertyChanged = delegate (object s2, PropertyChangedEventArgs e2)
            {
                switch (e2.PropertyName)
                {
                    case nameof(IMap.Layers):
                        if (_layers != null)
                        {
                            _layers.CollectionChanged -= onLayersChanged;
                        }
                        _layers = Current.Map?.Layers;
                        if (_layers != null)
                        {
                            _layers.CollectionChanged += onLayersChanged;
                        }
                        onLayersChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                        break;
                    case nameof(IMap.Boundaries):
                        PropertyChangedEventHandler boudariesChanged = delegate (object s3, PropertyChangedEventArgs e3)
                        {
                            draw(RedrawType.Zoom);
                        };
                        if (_boundaries != null)
                        {
                            _boundaries.PropertyChanged -= boudariesChanged;
                        }
                        _boundaries = Current.Map?.Boundaries;
                        if (_layers != null)
                        {
                            _boundaries.PropertyChanged += boudariesChanged;
                        }
                        boudariesChanged(this, null);
                        break;
                }
            };

            _map = this.DataContext as IMap;
            if (_map != null)
            {
                _map.PropertyChanged += mapPropertyChanged;
            }
            mapPropertyChanged(this, new PropertyChangedEventArgs(nameof(IMap.Layers)));
            mapPropertyChanged(this, new PropertyChangedEventArgs(nameof(IMap.Boundaries)));
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            _map.Size = sizeInfo.NewSize;
            draw(RedrawType.Resize);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            zoom(e.GetPosition(this), Math.Abs(e.Delta) / 120, e.Delta > 0);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            var panPoint = e.GetPosition(this.Parent as IInputElement);

            if (_isPanning)
            {
                stopPan(panPoint);
            }
            else if (e.ChangedButton == MouseButton.Left)
            {
                if (e.ClickCount > 1)
                {
                    // TODO: handle double click
                }
                else
                {
                    startPan(panPoint, false);
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_isPanning)
            {
                pan(e.GetPosition(this.Parent as IInputElement), false);
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            var panPoint = e.GetPosition(this.Parent as IInputElement);

            if (_isPanning && panPoint != _panStartPoint)
            {
                stopPan(panPoint);
            }
            else
            {
                switch (e.ChangedButton)
                {
                    case MouseButton.Left:
                        // TODO: select
                        break;
                    case MouseButton.Middle:
                        startPan(panPoint, true);
                        break;
                    case MouseButton.Right:
                        // TODO: context menu
                        break;
                }
            }
        }
        
        #endregion

        #endregion

        #region Tools

        private void onLayersChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            var toDraw = new List<ILayer>();

            this.Children.Clear();
            if ((_layers?.Count ?? 0) == 0)
            {
                if (_childrenDictionary != null)
                {
                    _childrenDictionary.Clear();
                }
            }
            else
            {
                var children = new Dictionary<ILayer, UIElement>(_childrenDictionary);
                _childrenDictionary.Clear();

                foreach (var layer in _layers)
                {
                    UIElement child;
                    if (children.ContainsKey(layer))
                    {
                        child = children[layer];
                        children.Remove(layer);
                    }
                    else
                    {
                        child = layer.CreateContainer();
                        toDraw.Add(layer);
                    }
                    _childrenDictionary[layer] = child;
                    this.Children.Add(child);
                }
                
                while (children.Count > 0)
                {
                    var toRemove = children.First();
                    children.Remove(toRemove.Key);
                }
            }
            
            foreach (var layer in toDraw)
            {
                drawLayer(layer, RedrawType.Reset);
            }
        }

        private void draw(RedrawType redrawType)
        {
            foreach (var item in _childrenDictionary)
            {
                drawLayer(item.Key, item.Value, redrawType, _map.Boundaries, _map.Projection, true, false, 0);
            }
        }

        private void drawLayer(ILayer layer, RedrawType redrawType)
        {
            drawLayer(layer, _childrenDictionary[layer], redrawType, _map.Boundaries, _map.Projection, true, false, 0);
        }

        private void drawLayer(ILayer layer, UIElement container, RedrawType redrawType, LatLonBoundaries boundaries, IProjection projection, bool isDesign, bool isExport, long animationStep)
        {
            layer.Draw(new DrawContext(container, redrawType, boundaries, projection, isDesign, isExport, animationStep));
        }

        private void zoom(Point center, double factor, bool isZoomIn)
        {
            var xFactor = center.X / this.RenderSize.Width;
            var yFactor = center.Y / this.RenderSize.Height;
            var latLonCenter = _map.Projection.PixelToLatLon(center);
            var latLonFactor = factor * (isZoomIn ? 2d : .5d);
            var lonWidth = Math.Min((_map.Boundaries.LonMax - _map.Boundaries.LonMin) / latLonFactor, 360);
            var latHeight = Math.Min((_map.Boundaries.LatMax - _map.Boundaries.LatMin) / latLonFactor, 180);
            var lonMin = latLonCenter.Longitude - lonWidth * xFactor;
            var latMax = latLonCenter.Latitude + latHeight * yFactor;
            _map.Boundaries = getBoundaries(lonMin, latMax, lonWidth, latHeight);
        }

        private void startPan(Point startPoint, bool isWheelButton)
        {
            lock (_panLocker)
            {
                _isPanning = true;
                _isMouseDownWheel = isWheelButton;
                _panStartPoint = startPoint;
                _panLastPoint = startPoint;
                this.Cursor = _isMouseDownWheel ? Cursors.ScrollAll : Cursors.Hand;
            }
        }

        private void stopPan(Point stopPoint)
        {
            lock (_panLocker)
            {
                pan(stopPoint, true);
                _isPanning = false;
                this.Cursor = Cursors.Arrow;

            }
        }

        private void pan(Point point, bool isFinished)
        {
            var diff = point - _panStartPoint;
            if (isFinished)
            {
                var newCenterLatLon = _map.Projection.PixelToLatLon(new Point(
                    this.RenderSize.Width / 2 - diff.X, 
                    this.RenderSize.Height / 2 - diff.Y
                    ));
                var lonWidth = _map.Boundaries.LonMax - _map.Boundaries.LonMin;
                var latHeight = _map.Boundaries.LatMax - _map.Boundaries.LatMin;
                this.VisualTransform = null;
                _map.Boundaries = getBoundaries(newCenterLatLon.Longitude - lonWidth / 2, newCenterLatLon.Latitude + latHeight / 2, lonWidth, latHeight);
            }
            else if ((_panLastPoint - point).Length >= 2)// Prevents flicker
            {
                Console.WriteLine(_panStartPoint + " - " + _panLastPoint + " - " + point + " - " + (_panLastPoint - point).Length + " - " + diff);
                _panLastPoint = point;
                this.VisualTransform = new TranslateTransform(diff.X, diff.Y);
            }
        }

        private LatLonBoundaries getBoundaries(double lonMin, double latMax, double lonWidth, double latHeight)
        {
            if (latMax > 90)
            {
                latMax = 90;
            }
            var latMin = latMax - latHeight;
            if (latMin < -90)
            {
                latMin = -90;
                latMax = latMin + latHeight;
            }

            if (lonMin < -180)
            {
                lonMin = -180;
            }
            var lonMax = lonMin + lonWidth;
            if (lonMax > 180)
            {
                lonMax = 180;
                lonMin = lonMax - lonWidth;
            }

            return new LatLonBoundaries() { LatMax = latMax, LatMin = latMin, LonMax = lonMax, LonMin = lonMin };
        }

        #endregion
    }
}
