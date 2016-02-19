using CartaMei.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace CartaMei.GSHHG
{
    public interface IGshhgLayer : ILayer
    {
        Resolution Resolution { get; }

        bool UseCurvedLines { get; }

        double StrokeThickness { get; }

        Brush StrokeBrush { get; }

        PolygonType PolygonType { get; }
    }

    public abstract class AGshhgLayer : ALayer, IGshhgLayer
    {
        #region Fields
        
        private Resolution _loadedResolution;

        private readonly object _readerLocker = new object();

        private string _mapsDir = null;

        protected IDictionary<int, IPolygon<GSHHG2PolygonHeader, LatLonCoordinates>> _polygons = null;

        protected IDictionary<int, PolygonObject> _polygonObjects = null;

        protected GshhgContainer _container = null;
        
        private readonly object _drawLocker = new object();

        #endregion

        #region Constructor

        public AGshhgLayer(IMap map)
            : base(map)
        {
            _loadedResolution = (Resolution)int.MaxValue;// Invalid value

            _useCurvedLines = PluginSettings.Instance.UseCurvedLines;
            _resolution = PluginSettings.Instance.Resolution;
            foreach (var layer in this.Map.Layers)
            {
                var gshhgLayer = layer as IGshhgLayer;
                if (gshhgLayer != null)
                {
                    _useCurvedLines = gshhgLayer.UseCurvedLines;
                    _resolution = gshhgLayer.Resolution;
                }
            }

            _strokeThickness = PluginSettings.Instance.StrokeThickness;
            _strokeBrush = PluginSettings.Instance.StrokeBrush.GetFrozenCopy();

            _container = new GshhgContainer(this);
        }

        #endregion

        #region Properties

        [Browsable(false)]
        protected virtual bool ClosePolygons { get { return false; } }

        private Resolution _resolution;
        [Description("The resolution to use.")]
        [DisplayName("Resolution")]
        [Category("Look")]
        [PropertyOrder(0)]
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
        [Category("Look")]
        [PropertyOrder(1)]
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
        
        private double _strokeThickness;
        [Description("The thickness of the contour of the objects.")]
        [DisplayName("Stroke Thickness")]
        [Category("Look")]
        [PropertyOrder(1000)]
        public double StrokeThickness
        {
            get { return _strokeThickness; }
            set
            {
                if (_strokeThickness != value)
                {
                    _strokeThickness = value;
                    redraw(false);
                    onPropetyChanged();
                }
            }
        }

        private Brush _strokeBrush;
        [Description("The brush used to draw the contour of the objects.")]
        [DisplayName("Stroke Brush")]
        [Category("Look")]
        [PropertyOrder(1001)]
        public Brush StrokeBrush
        {
            get { return _strokeBrush; }
            set
            {
                value.SafeFreeze();
                if (_strokeBrush != value)
                {
                    _strokeBrush = value;
                    redraw(false);
                    onPropetyChanged();
                }
            }
        }
        
        #endregion

        #region ALayer

        public override UIElement Container { get { return _container as UIElement; } }

        public override void Draw(IDrawContext context, CancellationToken cancellation)
        {
            resetMapData(context, cancellation);
            
            if (cancellation.IsCancellationRequested) return;

            if (context.RedrawType != RedrawType.Redraw)
            {
                lock (_drawLocker)
                {
                    const RedrawType noNewPointTypes = RedrawType.Scale | RedrawType.ZoomIn | RedrawType.AnimationStepChanged | RedrawType.DisplayTypeChanged | RedrawType.Redraw;
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
                                if (context.Projection.IsInMap(item.Value.Header.Boundaries))
                                {
                                    if (polygonObject != null)
                                    {
                                        toRemove.Remove(id);
                                    }
                                    else
                                    {
                                        polygonObject = getNewPolygonObject(id, item.Value);
                                        updateVisual(polygonObject);
                                        _polygonObjects.Add(id, polygonObject);
                                    }
                                    updatePoints(polygonObject, context.RedrawType, context.Transform);
                                }
                            }
                        }

                        removeChildren(toRemove);

                        _container.Polygons = new Dictionary<int, PolygonObject>(_polygonObjects);
                    }
                }
            }

            if (cancellation.IsCancellationRequested) return;

            switch (context.RedrawType)
            {
                case RedrawType.Reset:
                case RedrawType.Redraw:
                case RedrawType.ZoomIn:
                case RedrawType.ZoomOut:
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

        #region Abstract

        [Browsable(false)]
        public abstract PolygonType PolygonType { get; }
        
        #endregion

        #region Tools

        protected void redraw(bool reset)
        {
            if (!reset)
            {
                lock (_drawLocker)
                {
                    var polygons = _polygonObjects?.Values;
                    if (polygons != null)
                    {
                        foreach (var item in _polygonObjects.Values)
                        {
                            updateVisual(item);
                        }
                    }
                }
            }
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

                    var ownsStatus = Utils.Instance.SetStatus("Loading " + this.PolygonType.ToString().ToLower() +  " data...", true, true, 0, true);
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    var reader = new GSHHG2Reader();
                    // TODO: lazy load
                    // TODO: handle cancellation
                    _polygons = reader.Read(PluginSettings.Instance.MapsDirectory, this.PolygonType, this.Resolution, this.ClosePolygons).Polygons;
                    var time = watch.ElapsedMilliseconds;
                    if (ownsStatus) Utils.Instance.HideStatus();
                    _polygonObjects = new Dictionary<int, PolygonObject>();
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
                removeChild(item);
            }
        }

        protected virtual void removeChild(PolygonObject item) { }

        private void updatePoints(PolygonObject polygonObject, RedrawType redrawType, Transform transform)
        {
            var duplicate = false;
            var boundaries = polygonObject.Polygon.Header.Boundaries;
            var boundaryCenter = boundaries.Center;
            if (this.Map.RotateReference) boundaryCenter = this.Map.Boundaries.Center.GetNewCoordinates(boundaryCenter);
            var crossesLeftToRight = boundaryCenter.Longitude < 180 && boundaryCenter.Longitude + boundaries.LongitudeHalfSpan > this.Map.Boundaries.RightLongitude && boundaryCenter.Longitude + boundaries.LongitudeHalfSpan - 360 > -180;
            var crossesRightToLeft = boundaryCenter.Longitude > -180 && boundaryCenter.Longitude - boundaries.LongitudeHalfSpan < this.Map.Boundaries.LeftLongitude && boundaryCenter.Longitude - boundaries.LongitudeHalfSpan + 360 < 180;
            if (crossesLeftToRight || crossesRightToLeft)
            {
                duplicate = true;
            }

            var useTransform = true;
            if (duplicate)
            {
                useTransform = false;
            }
            else if (polygonObject.PixelPoints?.Any() ?? false)
            {
                useTransform = false;
            }
            else
            {
                const RedrawType transformRedrawTypes = RedrawType.Translate | RedrawType.Scale | RedrawType.AnimationStepChanged | RedrawType.DisplayTypeChanged | RedrawType.Redraw;
                switch (redrawType & ~transformRedrawTypes)
                {
                    case RedrawType.None:
                        break;
                    case RedrawType.Translate:
                        useTransform = this.Map.Projection.CanUseTransformForTranslate;
                        break;
                    default:
                        useTransform = false;
                        break;
                }
            }
            if (useTransform)
            {
                foreach (var pixelPoints in polygonObject.PixelPoints)
                {
                    foreach (var point in pixelPoints)
                    {
                        var newPoint = transform.Transform(point);
                        point.X = newPoint.X;
                        point.Y = newPoint.Y;
                    }
                }
            }
            else
            {
                polygonObject.PixelPoints = getPoints(polygonObject, crossesLeftToRight, crossesRightToLeft);
            }
        }

        private IEnumerable<IEnumerable<PixelCoordinates>> getPoints(PolygonObject polygonObject, bool crossesLeftToRight, bool crossesRightToLeft)
        {
            var points = polygonObject.Polygon.Points;
            if (points == null)
            {
                return new IEnumerable<PixelCoordinates>[0];
            }
            else if (!crossesLeftToRight && !crossesRightToLeft)
            {
                return new IEnumerable<PixelCoordinates>[]
                {
                    points.Select(point => this.Map.Projection.LatLonToPixel(new LatLonCoordinates() { Latitude = point.Latitude, Longitude = point.Longitude.FixCoordinate(false) })).ToList()
                };
            }
            else
            {
                var left = new PixelCoordinates[points.Length];
                var right = new PixelCoordinates[points.Length];
                for (int i = 0; i < points.Length; i++)
                {
                    var point = points[i];
                    var rightPoint = crossesLeftToRight ? point : new LatLonCoordinates() { Latitude = point.Latitude, Longitude = point.Longitude + 360 };
                    var leftPoint = crossesRightToLeft ? point : new LatLonCoordinates() { Latitude = point.Latitude, Longitude = point.Longitude - 360 };
                    right[i] = this.Map.Projection.LatLonToPixel(rightPoint);
                    left[i] = this.Map.Projection.LatLonToPixel(leftPoint);
                }
                return new IEnumerable<PixelCoordinates>[] { right, left };
            }
        }

        protected virtual PolygonObject getNewPolygonObject(int id, IPolygon<GSHHG2PolygonHeader, LatLonCoordinates> polygon)
        {
            return new PolygonObject()
            {
                Id = id,
                Polygon = polygon,
                IsActive = true,
                Layer = this,
                Name = this.PolygonType.ToString() + " #" + id
            };
        }

        protected virtual void updateVisual(PolygonObject item) { }

        #endregion
    }

    public class PolygonObject : MapLeafObject, ILayerItem
    {
        #region Properties

        [Category("GSHHG")]
        [Description("The ID of the polygon.")]
        [DisplayName("ID")]
        [ReadOnly(true)]
        public virtual int Id { get; set; }

        [Browsable(false)]
        public virtual IPolygon<GSHHG2PolygonHeader, LatLonCoordinates> Polygon { get; set; }

        [Browsable(false)]
        public virtual IEnumerable<IEnumerable<PixelCoordinates>> PixelPoints { get; internal set; }

        [ReadOnly(true)]
        public virtual Brush Fill { get; set; }
        
        #endregion
    }
    
    /// <summary>
    /// GSHHG container that draws into an image using GDI+.
    /// </summary>
    /// <remarks>
    /// <para>Adapted from <see href="http://www.newyyz.com/blog/2012/02/14/fast-line-rendering-in-wpf/">Sean Hopen's solution</see>.</para>
    /// <para>This solution does most of the work in another thread, which should make the UI much more responsive (it just has to display a bitmap).</para>
    /// </remarks>
    public class GshhgContainer : Grid
    {
        #region Fields
        
        private AGshhgLayer _layer;

        private Image _image;

        private BackgroundWorker _renderWorker;
        private readonly object _rendererLocker = new object();

        #region GDI

        private IDictionary<PolygonObject, IEnumerable<System.Drawing.Drawing2D.GraphicsPath>> _paths;

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

        public GshhgContainer(AGshhgLayer layer)
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

        #region IGshhgContainer

        public bool IsDirty { get; set; }
        
        public IDictionary<int, PolygonObject> Polygons { get; set; }

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
        
        #region Render

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
                var paths = new Dictionary<PolygonObject, IEnumerable<System.Drawing.Drawing2D.GraphicsPath>>();
                foreach (var item in polygons)
                {
                    var polygonPaths = new List<System.Drawing.Drawing2D.GraphicsPath>();
                    foreach (var pixelPoints in item.Value.PixelPoints)
                    {
                        var path = new System.Drawing.Drawing2D.GraphicsPath()
                        {
                            FillMode = System.Drawing.Drawing2D.FillMode.Alternate
                        };
                        var points = pixelPoints.Select(p => p.AsGdiPointF()).ToArray();
                        if (_layer.UseCurvedLines)
                        {
                            path.AddCurve(points);
                        }
                        else
                        {
                            path.AddLines(points);
                        }
                        polygonPaths.Add(path);
                    }
                    paths[item.Value] = polygonPaths;
                }
                _paths = paths;
                this.IsDirty = false;
            }
            
            var pen = _layer.StrokeBrush != null && _layer.StrokeBrush != Brushes.Transparent && _layer.StrokeThickness > 0
                ? new System.Drawing.Pen(_layer.StrokeBrush.AsGdiBrush(), (float)_layer.StrokeThickness)
                : null;
            foreach (var item in _paths)
            {
                foreach (var path in item.Value)
                {
                    try
                    {
                        var fill = item.Key.Fill != null && item.Key.Fill != Brushes.Transparent ? item.Key.Fill.AsGdiBrush() : null;
                        if (fill != null) _gdiGraphics.FillPath(fill, path);
                        if (pen != null) _gdiGraphics.DrawPath(pen, path);
                    }
                    catch { }
                }
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
                this.RenderTransform = Transform.Identity;
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
