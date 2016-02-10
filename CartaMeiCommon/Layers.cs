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
    public interface ILayerItem : INotifyPropertyChanged
    {
        #region Properties

        string Name { get; set; }

        ObservableCollection<ILayerItem> Children { get; }

        #endregion
    }

    public interface ILayer : ILayerItem, INotifyPropertyChanged
    {
        #region Properties
        
        IMapObject Root { get; set; }
        
        #endregion

        #region Function

        UIElement CreateContainer();

        void Draw(IDrawContext context);

        #endregion
    }

    public interface IDrawContext
    {
        #region Properties

        UIElement Container { get; }

        RedrawType RedrawType { get; }

        IProjection Projection { get; }

        LatLonBoundaries Boundaries { get; }

        bool IsDesign { get; }

        bool IsExport { get; }

        long AnimationStep { get; }
        
        #endregion
    }
    
    public class DrawContext : IDrawContext
    {
        #region Constructor

        public DrawContext(UIElement container, RedrawType redrawType, LatLonBoundaries boundaries, IProjection projection, bool isDesign, bool isExport, long animationStep)
        {
            this.Container = container;
            this.RedrawType = redrawType;
            this.Boundaries = boundaries;
            this.Projection = projection;
            this.IsDesign = IsDesign;
            this.IsExport = IsExport;
            this.AnimationStep = animationStep;
        }

        #endregion

        #region Properties

        public UIElement Container { get; private set; }

        public RedrawType RedrawType { get; private set; }

        public LatLonBoundaries Boundaries { get; private set; }

        public IProjection Projection { get; private set; }

        public bool IsDesign { get; private set; }

        public bool IsExport { get; private set; }

        public long AnimationStep { get; private set; }

        #endregion
    }

    public enum RedrawType
    {
        Reset,
        Translation,
        Resize,
        Zoom
    }

    public abstract class ALayer : NotifyPropertyChangedBase, ILayer
    {
        #region ILayer

        private string _name;
        public virtual string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    onPropetyChanged();
                }
            }
        }
        
        private IMapObject _root;
        [Browsable(false)]
        public virtual IMapObject Root
        {
            get { return _root; }
            set
            {
                if (value != _root)
                {
                    _root = value;
                    onPropetyChanged();
                }
            }
        }

        [Browsable(false)]
        public virtual ObservableCollection<ILayerItem> Children { get { return null; } }

        public virtual UIElement CreateContainer()
        {
            return new Canvas();
        }

        public abstract void Draw(IDrawContext context);
        
        #endregion
    }

    public class LayerItem : NotifyPropertyChangedBase, ILayerItem
    {
        #region ILayerItem

        private ObservableCollection<ILayerItem> _children;
        [Browsable(false)]
        public ObservableCollection<ILayerItem> Children
        {
            get { return _children; }
            set
            {
                if (value != _children)
                {
                    _children = value;
                    onPropetyChanged();
                }
            }
        }

        private string _name;
        [Category("General")]
        [Description("The name of the layer item.")]
        [DisplayName("Name")]
        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    onPropetyChanged();
                }
            }
        }
        
        #endregion
    }
}
