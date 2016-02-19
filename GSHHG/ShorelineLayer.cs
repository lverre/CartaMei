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
    public class ShorelineLayer : AGshhgLayer
    {
        #region Constants

        public const string LayerName = "Shorelines";
        public const string LayerDescription = "Shorelines from GSHHG data";

        #endregion
        
        #region Constructor

        public ShorelineLayer(IMap map)
            : base(map, ShorelineLayer.LayerName, PluginSettings.Instance.ShorelineThickness, PluginSettings.Instance.ShorelineBrush)
        {
            _useWaterForBackground = PluginSettings.Instance.ShorelinesUseWaterForBackground;
            _backgroundFill = PluginSettings.Instance.ShorelinesBackground;
            _waterFill = PluginSettings.Instance.ShorelinesWaterFill;
            _landFill = PluginSettings.Instance.ShorelinesLandFill;
            _antarcticaFill = PluginSettings.Instance.AntarcticaFill;
            _antarcticaIceFrontFill = PluginSettings.Instance.AntarcticaIceFrontFill;
            
            _container.MouseMove += mouseMove;
            resetBackground();
        }

        #endregion

        #region Properties
        
        private bool _useWaterForBackground;
        [Description("Choose this option if you want the background to always match that of the sea.")]
        [DisplayName("Background Is Sea")]
        [Category("Look")]
        [PropertyOrder(998)]
        public bool UseSeaForBackground
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

        private Color _backgroundFill;
        [Description("The brush used to fill the background.")]
        [DisplayName("Background")]
        [Category("Look")]
        [PropertyOrder(999)]
        public Color BackgroundFill
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
        
        private Color _waterFill;
        [Description("The brush used to fill water areas.")]
        [DisplayName("Water Fill")]
        [Category("Look")]
        [PropertyOrder(1002)]
        public Color WaterFill
        {
            get { return _waterFill; }
            set
            {
                if (_waterFill != value)
                {
                    _waterFill = value;
                    resetBackground();
                    redraw(false);
                    onPropetyChanged();
                }
            }
        }

        private Color _landFill;
        [Description("The brush used to fill land areas.")]
        [DisplayName("Land Fill")]
        [Category("Look")]
        [PropertyOrder(1003)]
        public Color LandFill
        {
            get { return _landFill; }
            set
            {
                if (_landFill != value)
                {
                    _landFill = value;
                    redraw(false);
                    onPropetyChanged();
                }
            }
        }

        private Color _antarcticaFill;
        [Description("The brush used to fill Antarctica.")]
        [DisplayName("Antarctica Fill")]
        [Category("Look")]
        [PropertyOrder(1004)]
        public Color AntarcticaFill
        {
            get { return _antarcticaFill; }
            set
            {
                if (_antarcticaFill != value)
                {
                    _antarcticaFill = value;
                    redraw(false);
                    onPropetyChanged();
                }
            }
        }

        private Color _antarcticaIceFrontFill;
        [Description("The brush used to fill the Antarctica ice front.")]
        [DisplayName("Antarctica Ice Front Fill")]
        [Category("Look")]
        [PropertyOrder(1005)]
        public Color AntarcticaIceFrontFill
        {
            get { return _antarcticaIceFrontFill; }
            set
            {
                if (_antarcticaIceFrontFill != value)
                {
                    _antarcticaIceFrontFill = value;
                    redraw(false);
                    onPropetyChanged();
                }
            }
        }

        #endregion
        
        #region ALayer

        public override void SetLayerAdded(int layerIndex)
        {
            if (layerIndex > 0) this.UseSeaForBackground = false;
        }

        #endregion

        #region AGshhgLayer

        public override PolygonType PolygonType { get { return PolygonType.ShoreLine; } }

        protected override bool ClosePolygons { get { return true; } }
        
        protected override void updateVisual(PolygonObject item)
        {
            switch (item.Polygon.Header.ShorelinesFlags)
            {
                case GSHHGFlag.AntarticaGround:
                    item.Fill = this.AntarcticaFill;
                    break;
                case GSHHGFlag.AntarticaIce:
                    item.Fill = this.AntarcticaIceFrontFill;
                    break;
                default:
                    item.Fill = item.Polygon.Header.IsLand ? this.LandFill : this.WaterFill;
                    break;
            }
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
            var container = _container;
            if (container != null) container.Background = new SolidColorBrush(this.UseSeaForBackground ? this.WaterFill : this.BackgroundFill);
        }
        
        #endregion
    }
}
