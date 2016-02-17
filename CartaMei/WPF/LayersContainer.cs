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

        private readonly object _layersLocker = new object();

        private IDictionary<ILayer, UIElement> _childrenDictionary;

        private readonly object _panLocker = new object();
        private Point _panStartPoint;
        private Point _panLastPoint;
        private bool _isMouseDownWheel;
        private bool _isPanning;

        private Transform _currentTransform;
        private MatrixTransform _cumulativeTransform;

        #endregion

        #region Constructor

        public LayersContainer()
        {
            _map = null;
            _boundaries = null;
            _layers = null;
            _childrenDictionary = new Dictionary<ILayer, UIElement>();
            _currentTransform = Transform.Identity;
            _currentTransform.Freeze();
            _cumulativeTransform = new MatrixTransform(Matrix.Identity);
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

            Current.AnimationStepChanged += delegate (CurrentPropertyChangedEventArgs<long> asc)
            {
                if (Current.Map == _map)
                {
                    draw(RedrawType.AnimationStepChanged);
                }
            };
            Current.DisplayTypeChanged += delegate (CurrentPropertyChangedEventArgs<DisplayType> asc)
            {
                if (Current.Map == _map)
                {
                    draw(RedrawType.DisplayTypeChanged);
                }
            };
        }
        
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            _map.Size = sizeInfo.NewSize;
            foreach (var item in _layers)
            {
                var fe = item.Container as FrameworkElement;
                if (fe != null)
                {
                    fe.Width = _map.Size.Width;
                    fe.Height = _map.Size.Height;
                }
            }
            draw(RedrawType.Scale);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            zoom(e.GetPosition(this), Math.Abs(e.Delta) / 120, e.Delta > 0);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            var wasPanning = false;
            if (_isPanning)
            {
                stopPan(e);
                wasPanning = true;
            }

            if (e.ChangedButton == MouseButton.Left && e.ClickCount > 1)
            {
                var toSelect = getObjectsAt(e).FirstOrDefault()?.Item2;
                if (toSelect != null)
                {
                    toSelect.DoubleClick();
                }
            }
            else if (!wasPanning && e.ChangedButton == MouseButton.Left)
            {
                startPan(e, false);
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

            var panPoint = getPanPoint(e);

            if (_isPanning && panPoint != _panStartPoint)
            {
                stopPan(panPoint);
            }
            else
            {
                switch (e.ChangedButton)
                {
                    case MouseButton.Left:
                        var toSelect = getObjectsAt(e).FirstOrDefault()?.Item2;
                        if (toSelect != null)
                        {
                            toSelect.Select();
                            _map.ActiveObject = toSelect;
                        }
                        break;
                    case MouseButton.Middle:
                        startPan(panPoint, true);
                        break;
                    case MouseButton.Right:
                        var selectable = getObjectsAt(e).ToArray();
                        if (selectable.Length > 0)
                        {
                            var menu = new ContextMenu()
                            {
                                Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse
                            };
                            foreach (var item in selectable)
                            {
                                var menuItem = new MenuItem() { Header = item.Item1.Name + " - " + item.Item2.Name };
                                menuItem.Click += (s2, e2) =>
                                {
                                    item.Item2.Select();
                                    Current.Map.ActiveObject = item.Item2;
                                };
                                menu.Items.Add(menuItem);
                            }
                            menu.IsOpen = true;
                        }
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

            lock (_layersLocker)
            {
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

                    for (int i = 0; i < _layers.Count; i++)
                    {
                        var layer = _layers[i];
                        UIElement child;
                        if (children.ContainsKey(layer))
                        {
                            child = children[layer];
                            children.Remove(layer);
                        }
                        else
                        {
                            child = layer.Container;
                            child.ClipToBounds = true;
                            toDraw.Add(layer);
                            layer.SetLayerAdded(i);
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
            }
            
            foreach (var layer in toDraw)
            {
                drawLayer(layer, RedrawType.Reset);
            }
        }

        private void draw(RedrawType redrawType)
        {
            if ((redrawType & ~(RedrawType.Redraw | RedrawType.Scale | RedrawType.Translate)) != RedrawType.None)
            {
                _currentTransform = Transform.Identity;
                _currentTransform.Freeze();
                _cumulativeTransform.Matrix.SetIdentity();
            }

            foreach (var item in _childrenDictionary)
            {
                drawLayer(item.Key, item.Value, redrawType, _map);
            }
        }

        private void drawLayer(ILayer layer, RedrawType redrawType)
        {
            if ((redrawType & layer.HandledRedrawTypes) != RedrawType.None)
            {
                drawLayer(layer, _childrenDictionary[layer], redrawType, _map);
            }
        }

        private void drawLayer(ILayer layer, UIElement container, RedrawType redrawType, IMap map)
        {
            layer.DrawAsync(new DrawContext(container, redrawType, _currentTransform, _cumulativeTransform, map));
        }

        private void zoom(Point center, double factor, bool isZoomIn)
        {
            var latLonFactor = factor * (isZoomIn ? 2d : .5d);
            _currentTransform = new ScaleTransform(latLonFactor, latLonFactor);
            _currentTransform.Freeze();
            _cumulativeTransform.Matrix.Scale(latLonFactor, latLonFactor);

            var xFactor = center.X / this.RenderSize.Width;
            var yFactor = center.Y / this.RenderSize.Height;
            var latLonCenter = _map.Projection.PixelToLatLon(center);
            var lonSpan = Math.Min(_map.Boundaries.LongitudeSpan / latLonFactor, LatLonBoundaries.MaxLongitudeSpan);
            var latSpan = Math.Min(_map.Boundaries.LatitudeSpan / latLonFactor, LatLonBoundaries.MaxLatitudeSpan);
            _map.Boundaries = _map.Projection.BoundMap(latLonCenter.Latitude, latLonCenter.Longitude, latSpan, lonSpan);
        }

        private Point getPanPoint(MouseEventArgs e)
        {
            return e.GetPosition(this.Parent as IInputElement);
        }

        private void startPan(MouseEventArgs e, bool isWheelButton)
        {
            startPan(getPanPoint(e), isWheelButton);
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

        private void stopPan(MouseEventArgs e)
        {
            stopPan(getPanPoint(e));
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
                _currentTransform = new TranslateTransform(diff.X, diff.Y);
                _currentTransform.Freeze();
                _cumulativeTransform.Matrix.Translate(diff.X, diff.Y);
                _map.Boundaries = _map.Projection.BoundMap(newCenterLatLon.Latitude, newCenterLatLon.Longitude, _map.Boundaries.LatitudeSpan, _map.Boundaries.LongitudeSpan);
                this.VisualTransform = null;
            }
            else if ((_panLastPoint - point).Length >= 2)// Prevents flicker
            {
                _panLastPoint = point;
                this.VisualTransform = new TranslateTransform(diff.X, diff.Y);
            }
        }

        private IEnumerable<Tuple<ILayer, IMapObject>> getObjectsAt(MouseButtonEventArgs e)
        {
            return getObjectsAt(e.GetPosition(this));
        }

        private IEnumerable<Tuple<ILayer, IMapObject>> getObjectsAt(Point at)
        {
            ILayer[] layers;
            lock (_layersLocker)
            {
                layers = _layers.ToArray();
            }
            for (int i = layers.Length - 1; i >= 0; i--)
            {
                var layer = layers[i];
                var items = layer.GetObjectsAt(at);
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        yield return new Tuple<ILayer, IMapObject>(layer, item);
                    }
                }
            }
            yield break;
        }

        #endregion
    }
}
