using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CartaMei.Common
{
    public interface ILayerItem : IMapObject, INotifyPropertyChanged { }

    public interface ILayer : ILayerItem, INotifyPropertyChanged
    {
        #region Function

        UIElement Container { get; }

        RedrawType HandledRedrawTypes { get; }

        void SetLayerAdded(int layerIndex);

        Task DrawAsync(IDrawContext context);

        void FastPan(double x, double y);

        IEnumerable<IMapObject> GetObjectsAt(Point at);

        #endregion
    }

    public interface IDrawContext
    {
        #region Properties

        UIElement Container { get; }

        RedrawType RedrawType { get; }

        Transform Transform { get; }

        MatrixTransform CumulativeTransform { get; }

        IProjection Projection { get; }

        LatLonBoundaries Boundaries { get; }

        DisplayType DisplayType { get; }
        
        long AnimationStep { get; }
        
        #endregion
    }
    
    public class DrawContext : IDrawContext
    {
        #region Constructor
        
        public DrawContext(UIElement container, RedrawType redrawType, Transform transform, MatrixTransform cumulativeTransform, IMap map)
            : this(container, redrawType, transform, cumulativeTransform, map.Boundaries, map.Projection, Current.DisplayType, Current.AnimationStep)
        { }

        public DrawContext(UIElement container, RedrawType redrawType, Transform transform, MatrixTransform cumulativeTransform, LatLonBoundaries boundaries, IProjection projection, DisplayType displayType, long animationStep)
        {
            this.Container = container;
            this.RedrawType = redrawType;
            this.Transform = transform;
            this.CumulativeTransform = cumulativeTransform;
            this.Boundaries = boundaries;
            this.Projection = projection;
            this.DisplayType = displayType;
            this.AnimationStep = animationStep;
        }

        #endregion

        #region Properties

        public UIElement Container { get; private set; }

        public RedrawType RedrawType { get; private set; }

        public Transform Transform { get; private set; }

        public MatrixTransform CumulativeTransform { get; private set; }

        public LatLonBoundaries Boundaries { get; private set; }

        public IProjection Projection { get; private set; }

        public DisplayType DisplayType { get; private set; }
        
        public long AnimationStep { get; private set; }

        #endregion
    }

    public enum DisplayType
    {
        Design,
        Export
    }

    [Flags]
    public enum RedrawType
    {
        None = 0,
        Reset = 1,
        Redraw = 2,
        Translate = 4,
        Scale = 8,
        Zoom = 16,
        DisplayTypeChanged = 32,
        AnimationStepChanged = 64
    }

    public abstract class ALayer : MapObject, ILayer
    {
        #region Fields

        private CancellationTokenSource _canceler;

        private readonly object _cancelerLocker = new object();

        #endregion

        #region Constructor

        public ALayer(IMap map)
        {
            this.Map = map;
        }

        #endregion

        #region ILayer

        [Browsable(false)]
        public abstract UIElement Container { get; }

        [Browsable(false)]
        public virtual RedrawType HandledRedrawTypes
        {
            get { return RedrawType.Redraw | RedrawType.Reset | RedrawType.Scale | RedrawType.Translate | RedrawType.Zoom; }
        }

        public virtual void SetLayerAdded(int layerIndex) { }

        public virtual Task DrawAsync(IDrawContext context)
        {
            Console.Write("Called DrawAsync from: " + Environment.NewLine + new System.Diagnostics.StackTrace(true).GetFrames().Where(item => item?.GetMethod()?.DeclaringType.Namespace.StartsWith("CartaMei") ?? false).Take(5).Aggregate("", (acc, item) => acc + "\t" + item.GetMethod().DeclaringType.FullName + "." + item.GetMethod().Name + " from " + item.GetFileName() + " at line " + item.GetFileLineNumber() + " col " + item.GetFileColumnNumber() + Environment.NewLine));

            CancellationToken token;
            lock (_cancelerLocker)
            {
                _canceler?.Cancel();
                _canceler = new CancellationTokenSource();
                token = _canceler.Token;
            }
            return Task.Run(delegate ()
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                if (!this.Map.Size.IsEmpty) this.Draw(context, token);
                watch.Stop();
                Console.WriteLine("Draw finished in " + watch.ElapsedMilliseconds + " ms");
            });
        }

        public virtual void FastPan(double panX, double panY)
        {
            this.Container.RenderTransform = new TranslateTransform(panX, panY);
        }

        public virtual IEnumerable<IMapObject> GetObjectsAt(Point at) { return null; }

        #endregion
        
        #region Abstract

        public abstract void Draw(IDrawContext context, CancellationToken cancellation);

        #endregion

        #region Properties

        public IMap Map { get; private set; }

        #endregion
    }

    public class LayerItem : MapObject, ILayerItem { }
}
