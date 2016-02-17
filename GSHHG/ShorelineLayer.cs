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

        private IShorelineContainer _container = null;
        
        private readonly object _drawLocker = new object();

        #endregion

        #region Constructor

        public ShorelineLayer(IMap map)
            : base(map)
        {
            _loadedResolution = (Resolution)int.MaxValue;// Invalid value

            _shorelinesThickness = PluginSettings.Instance.ShorelinesThickness;
            _shorelinesBrush = PluginSettings.Instance.ShorelinesBrush.GetFrozenCopy();
            _waterFill = PluginSettings.Instance.ShorelinesWaterFill.GetFrozenCopy();
            _landFill = PluginSettings.Instance.ShorelinesLandFill.GetFrozenCopy();
            _useWaterForBackground = PluginSettings.Instance.ShorelinesUseWaterForBackground;
            _backgroundFill = PluginSettings.Instance.ShorelinesBackground.GetFrozenCopy();
            _resolution = PluginSettings.Instance.Resolution;
            _useCurvedLines = PluginSettings.Instance.UseCurvedLines;

            var container = new GdiShorelineContainer(this);
            container.MouseMove += mouseMove;
            _container = container;
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
                value.SafeFreeze();
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
                value.SafeFreeze();
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
                value.SafeFreeze();
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
                value.SafeFreeze();
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

        public override UIElement Container { get { return _container as UIElement; } }

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
                    const RedrawType noNewPointTypes = RedrawType.Scale | RedrawType.Zoom | RedrawType.AnimationStepChanged | RedrawType.DisplayTypeChanged | RedrawType.Redraw;
                    if ((_polygonObjects?.Any() ?? false) && ((context.RedrawType & ~noNewPointTypes) == RedrawType.None))
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
                    _container.IsDirty = true;
                    _container.Update(context);
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
            var point = e.GetPosition(((FrameworkElement)_container).Parent as IInputElement);
            var latLon = this.Map.Projection.PixelToLatLon(point);
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
            this.DrawAsync(new DrawContext(_container as UIElement, reset ? RedrawType.Reset : RedrawType.Redraw, null, null, this.Map));
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

                    var ownsStatus = Utils.Instance.SetStatus("Loading map...", true, true, 0, true);
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    var reader = new GSHHG2Reader();
                    // TODO: lazy load
                    // TODO: handle cancellation
                    _polygons = reader.Read(PluginSettings.Instance.MapsDirectory, PolygonType.ShoreLine, this.Resolution).Polygons;
                    var time = watch.ElapsedMilliseconds;
                    if (ownsStatus) Utils.Instance.HideStatus();
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
            return polygonObject.Polygon.Points?.Select(point => this.Map.Projection.LatLonToPixel(point)).ToList();
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
        public IEnumerable<PixelCoordinates> PixelPoints { get; internal set; }

        #endregion
    }

    public interface IShorelineContainer
    {
        Brush Background { get; set; }

        bool IsDirty { get; set; }
        
        IDictionary<int, ShorelinePolygonObject> Polygons { get; set; }

        void Update(IDrawContext drawContext);
    }

    /// <summary>
    /// Shoreline container that draws using OnRender / Geometry.
    /// </summary>
    public class ShorelineContainer : FrameworkElement, IShorelineContainer
    {
        #region Fields

        private ShorelineLayer _layer;

        #endregion

        #region Constructor

        public ShorelineContainer(ShorelineLayer layer)
        {
            _layer = layer;
        }

        #endregion

        #region IShorelineContainer

        public Brush Background { get; set; }

        public bool IsDirty { get; set; }
        
        public IDictionary<int, ShorelinePolygonObject> Polygons { get; set; }

        public void Update(IDrawContext drawContext)
        {
            this.SafeInvalidate();
        }

        #endregion

        #region Properties

        private IDictionary<ShorelinePolygonObject, Geometry> Geometries { get; set; }

        #endregion

        #region Overrides

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
        
        #endregion
    }

    /// <summary>
    /// Shoreline container that draws into an image using GDI+.
    /// </summary>
    /// <remarks>
    /// <para>Adapted from <see href="http://www.newyyz.com/blog/2012/02/14/fast-line-rendering-in-wpf/">Sean Hopen's solution</see>.</para>
    /// <para>This solution does most of the work in another thread, which should make the UI much more responsive (it just has to display a bitmap).</para>
    /// </remarks>
    public class GdiShorelineContainer : Grid, IShorelineContainer
    {
        #region Fields
        
        private ShorelineLayer _layer;

        private Image _image;

        private BackgroundWorker _renderWorker;
        private readonly object _rendererLocker = new object();

        #region GDI

        private IDictionary<ShorelinePolygonObject, System.Drawing.Drawing2D.GraphicsPath> _paths;

        private bool _isBitmapInitialized;
        private bool _isBitmapInvalid;

        private System.Drawing.Bitmap _gdiBitmap;
        private System.Drawing.Graphics _gdiGraphics;
        private System.Windows.Interop.InteropBitmap _interopBitmap;

        private IntPtr _mapFileHandle;
        private IntPtr _mapViewPtr;

        private int _width;
        private int _height;

        #region Constants

        private const uint FILE_MAP_ALL_ACCESS = 0xF001F;
        private const uint PAGE_READWRITE = 0x04;

        private const System.Drawing.Imaging.PixelFormat _gdiPixelFormat = System.Drawing.Imaging.PixelFormat.Format32bppPArgb;
        private static readonly PixelFormat _pixelFormat = PixelFormats.Pbgra32;
        private static readonly int _bytesPerPixel = _pixelFormat.BitsPerPixel / 8;

        #endregion

        #endregion
        
        #endregion

        #region Constructors

        public GdiShorelineContainer(ShorelineLayer layer)
        {
            _layer = layer;

            _image = new Image()
            {
                Stretch = Stretch.None
            };
            this.Children.Add(_image);

            _renderWorker = new BackgroundWorker() { WorkerSupportsCancellation = true };
            _renderWorker.DoWork += doRender;
            _renderWorker.RunWorkerCompleted += renderFinished;

            _isBitmapInitialized = false;
            _isBitmapInvalid = true;
            _mapFileHandle = (IntPtr)(-1);
            _mapViewPtr = (IntPtr)(-1);

            _width = 0;
            _height = 0;

            _layer.Map.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(IMap.UseAntiAliasing))
                {
                    this.Update(null);
                }
            };
        }

        #endregion

        #region IShorelineContainer

        public bool IsDirty { get; set; }
        
        public IDictionary<int, ShorelinePolygonObject> Polygons { get; set; }

        public void Update(IDrawContext drawContext)
        {
            if (drawContext?.RedrawType.HasFlag(RedrawType.Scale) ?? false)
            {
                _isBitmapInvalid = true;
            }
            
            this.Dispatcher.InvokeAsync(delegate ()
            {
                lock (_rendererLocker)
                {
                    if (!_renderWorker.IsBusy) _renderWorker.RunWorkerAsync();
                }
            });
        }

        #endregion
        
        #region Events

        private void doRender(object sender, DoWorkEventArgs e)
        {
            var polygons = this.Polygons;
            if (!(polygons?.Any() ?? false))
            {
                _image.Source = null;
                return;
            }

            _width = _layer.Map.Size.Width;
            _height = _layer.Map.Size.Height;

            initialize();

            if (this.IsDirty || _paths == null)
            {
                var paths = new Dictionary<ShorelinePolygonObject, System.Drawing.Drawing2D.GraphicsPath>();
                foreach (var item in polygons)
                {
                    var path = new System.Drawing.Drawing2D.GraphicsPath()
                    {
                        FillMode = System.Drawing.Drawing2D.FillMode.Winding
                    };
                    var points = item.Value.PixelPoints.Select(p => p.AsGdiPointF()).ToArray();
                    if (_layer.UseCurvedLines)
                    {
                        path.AddCurve(points);
                    }
                    else
                    {
                        path.AddLines(points);
                    }
                    paths[item.Value] = path;
                }
                _paths = paths;
            }
            
            var pen = _layer.ShorelinesBrush != null && _layer.ShorelinesBrush != Brushes.Transparent && _layer.ShorelinesThickness > 0
                ? new System.Drawing.Pen(_layer.ShorelinesBrush.AsGdiBrush(), (float)_layer.ShorelinesThickness)
                : null;
            var land = _layer.LandFill != null && _layer.LandFill != Brushes.Transparent ? _layer.LandFill.AsGdiBrush() : null;
            var water = _layer.WaterFill != null && _layer.WaterFill != Brushes.Transparent ? _layer.WaterFill.AsGdiBrush() : null;
            foreach (var item in _paths)
            {
                var path = item.Value;
                var fill = item.Key.Polygon.Header.IsLand ? land : water;
                if (fill != null) _gdiGraphics.FillPath(fill, path);
                if (pen != null) _gdiGraphics.DrawPath(pen, path);
            }

            _interopBitmap.Invalidate();
            _interopBitmap.Freeze();
            e.Result = _interopBitmap;
        }

        private void renderFinished(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled) return;

            var bitmapSource = (e.Result as System.Windows.Media.Imaging.BitmapSource);
            if (bitmapSource != null && bitmapSource.CheckAccess())
            {
                _image.Source = bitmapSource;
            }
        }

        #endregion

        #region Tools

        protected void initialize()
        {
            if (_isBitmapInitialized || _isBitmapInvalid)
            {
                deallocate();
            }
            
            var byteCount = (uint)(_width * _height * _bytesPerPixel);
            
            _mapFileHandle = NativeMethods.CreateFileMapping(new IntPtr(-1), IntPtr.Zero, PAGE_READWRITE, 0, byteCount, null);
            _mapViewPtr = NativeMethods.MapViewOfFile(_mapFileHandle, FILE_MAP_ALL_ACCESS, 0, 0, byteCount);
            
            _interopBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromMemorySection(
                _mapFileHandle, _width, _height, _pixelFormat, _width * _bytesPerPixel, 0) 
                as System.Windows.Interop.InteropBitmap;
            _gdiGraphics = getGdiGraphics(_mapViewPtr);

            _isBitmapInitialized = true;
            _isBitmapInvalid = false;
        }

        private void deallocate()
        {
            if (_gdiGraphics != null)
            {
                _gdiGraphics.Dispose();
            }
            try
            {
                if (_mapViewPtr != (IntPtr)(-1))
                {
                    NativeMethods.UnmapViewOfFile(_mapViewPtr);
                }
                if (_mapFileHandle != (IntPtr)(-1))
                {
                    NativeMethods.CloseHandle(_mapFileHandle);
                }
            }
            finally
            {
                _mapViewPtr = (IntPtr)(-1);
                _mapFileHandle = (IntPtr)(-1);
                _isBitmapInitialized = false;
            }
        }

        private System.Drawing.Graphics getGdiGraphics(IntPtr mapPointer)
        {
            _gdiBitmap = new System.Drawing.Bitmap(_width, _height, _width * _bytesPerPixel, _gdiPixelFormat, mapPointer);

            var gdiGraphics = System.Drawing.Graphics.FromImage(_gdiBitmap);
            gdiGraphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            if (_layer.Map.UseAntiAliasing)
            {
                gdiGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                gdiGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            }
            else
            {
                gdiGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                gdiGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            }

            return gdiGraphics;
        }
        #endregion
    }

    internal class NativeMethods
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr CreateFileMapping(IntPtr hFile,
        IntPtr lpFileMappingAttributes,
        uint flProtect,
        uint dwMaximumSizeHigh,
        uint dwMaximumSizeLow,
        string lpName);

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject,
        uint dwDesiredAccess,
        uint dwFileOffsetHigh,
        uint dwFileOffsetLow,
        uint dwNumberOfBytesToMap);

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int CloseHandle(IntPtr hObject);

        [System.Runtime.InteropServices.DllImport("gdi32")]
        internal static extern int DeleteObject(IntPtr o);
    }

    public static class DrawingExtensions
    {
        public static System.Drawing.Color AsGdiColor(this System.Windows.Media.Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static System.Drawing.SolidBrush AsGdiBrush(this System.Windows.Media.Color color)
        {
            return new System.Drawing.SolidBrush(color.AsGdiColor());
        }

        public static System.Drawing.Brush AsGdiBrush(this System.Windows.Media.Brush brush)
        {
            var solidColorBrush = brush as System.Windows.Media.SolidColorBrush;
            return solidColorBrush?.Color.AsGdiBrush();
        }

        public static System.Drawing.Pen AsGdiPen(this System.Windows.Media.Pen pen)
        {
            var brush = pen.Brush as SolidColorBrush;
            brush = brush ?? Brushes.Transparent;
            var color = brush.Color.AsGdiColor();
            return new System.Drawing.Pen(color, (float)pen.Thickness);
        }

        public static System.Drawing.PointF AsGdiPointF(this PixelCoordinates point)
        {
            return new System.Drawing.PointF((float)point.X, (float)point.Y);
        }

        public static System.Drawing.Point AsGdiPoint(this PixelCoordinates point)
        {
            return new System.Drawing.Point((int)Math.Round(point.X), (int)Math.Round(point.Y));
        }
    }
}
