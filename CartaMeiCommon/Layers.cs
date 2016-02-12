using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
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

        UIElement CreateContainer();

        void Draw(IDrawContext context);

        IEnumerable<IMapObject> GetObjectsAt(Point at);

        #endregion
    }

    public interface IDrawContext
    {
        #region Properties

        UIElement Container { get; }

        RedrawType RedrawType { get; }

        IProjection Projection { get; }

        LatLonBoundaries Boundaries { get; }

        DisplayType DisplayType { get; }
        
        long AnimationStep { get; }
        
        #endregion
    }
    
    public class DrawContext : IDrawContext
    {
        #region Constructor

        public DrawContext(UIElement container, RedrawType redrawType, IMap map)
            : this(container, redrawType, map.Boundaries, map.Projection, Current.DisplayType, Current.AnimationStep)
        { }

        public DrawContext(UIElement container, RedrawType redrawType, LatLonBoundaries boundaries, IProjection projection, DisplayType displayType, long animationStep)
        {
            this.Container = container;
            this.RedrawType = redrawType;
            this.Boundaries = boundaries;
            this.Projection = projection;
            this.DisplayType = displayType;
            this.AnimationStep = animationStep;
        }

        #endregion

        #region Properties

        public UIElement Container { get; private set; }

        public RedrawType RedrawType { get; private set; }

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

    public enum RedrawType
    {
        Reset,
        Translation,
        Resize,
        Zoom,
        DisplayTypeChanged,
        AnimationStepChanged
    }

    public abstract class ALayer : MapObject, ILayer
    {
        #region ILayer
        
        public virtual UIElement CreateContainer()
        {
            return new Canvas();
        }

        public abstract void Draw(IDrawContext context);

        public virtual IEnumerable<IMapObject> GetObjectsAt(Point at) { return null; }

        #endregion
    }

    public class LayerItem : MapObject, ILayerItem { }
}
