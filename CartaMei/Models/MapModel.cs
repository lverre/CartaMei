using CartaMei.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using System.Windows.Media;

namespace CartaMei.Models
{
    [CategoryOrder("General", 0)]
    [CategoryOrder("Map", 1)]
    public class MapModel : NotifyPropertyChangedBase, IMap
    {
        #region IMap

        #region Properties

        #region Metadata

        private string _name;
        [Category("General")]
        [Description("The name of the map.")]
        [DisplayName("Name")]
        [PropertyOrder(0)]
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    onPropetyChanged();
                }
            }
        }
        
        private string _description;
        [Category("General")]
        [Description("A description of the map.")]
        [DisplayName("Description")]
        [PropertyOrder(2)]
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    onPropetyChanged();
                }
            }
        }
        
        private string _author;
        [Category("General")]
        [Description("The author of the map.")]
        [DisplayName("Author")]
        [PropertyOrder(3)]
        public string Author
        {
            get { return _author; }
            set
            {
                if (_author != value)
                {
                    _author = value;
                    onPropetyChanged();
                }
            }
        }
        
        private string _createdOn;
        [Category("General")]
        [Description("The date the map was created.")]
        [DisplayName("Created On")]
        [PropertyOrder(4)]
        public string CreatedOn
        {
            get { return _createdOn; }
            set
            {
                if (_createdOn != value)
                {
                    _createdOn = value;
                    onPropetyChanged();
                }
            }
        }
        
        private string _license;
        [Category("General")]
        [Description("The license of map.")]
        [DisplayName("License")]
        [PropertyOrder(5)]
        public string License
        {
            get { return _license; }
            set
            {
                if (_license != value)
                {
                    _license = value;
                    onPropetyChanged();
                }
            }
        }
        
        private string _version;
        [Category("General")]
        [Description("The version of the map.")]
        [DisplayName("Version")]
        [PropertyOrder(6)]
        public string Version
        {
            get { return _version; }
            set
            {
                if (_version != value)
                {
                    _version = value;
                    onPropetyChanged();
                }
            }
        }

        #endregion

        #region Map

        private Datum _datum;
        [Category("Map")]
        [Description("The datum used on this map (if you don't know which one to chose, use WGS84).")]
        [DisplayName("Datum")]
        [ReadOnly(true)]
        [PropertyOrder(4)]
        public Datum Datum
        {
            get { return _datum; }
            set
            {
                if (_datum != value)
                {
                    _datum = value;
                    onPropetyChanged();
                }
            }
        }

        private IProjection _projection;
        [Category("Map")]
        [Description("The projection used on this map (if you don't know which one to chose, use Mercator).")]
        [DisplayName("Projection")]
        [ExpandableObject]
        [PropertyOrder(2)]
        public IProjection Projection
        {
            get { return _projection; }
            set
            {
                if (_projection != value)
                {
                    _projection = value;
                    if (value == null || !value.SupportsReferenceChange) this.RotateReference = false;
                    onPropetyChanged();
                }
            }
        }
        
        private bool _rotateReference;
        [Category("Map")]
        [Description("When this option is used, the reference of sphere will be rotated to the center of the map.\nThis makes the shapes more accurate near the center of the map which is especially useful at high latitudes.")]
        [DisplayName("Rotate Reference")]
        [PropertyOrder(3)]
        public bool RotateReference
        {
            get { return _rotateReference; }
            set
            {
                if (value != _rotateReference && (!value || (this.Projection?.SupportsReferenceChange ?? false)))
                {
                    _rotateReference = value;
                    this.Boundaries = this.Projection.BoundMap(this.Boundaries.CenterLatitude, this.Boundaries.CenterLongitude, this.Boundaries.LatitudeSpan, this.Boundaries.LongitudeSpan);
                    this.Container?.Redraw();
                    onPropetyChanged();
                }
            }
        }

        private ObservableCollection<ILayer> _layers;
        [Browsable(false)]
        public ObservableCollection<ILayer> Layers
        {
            get { return _layers; }
            set
            {
                if (_layers != value)
                {
                    _layers = value;
                    onPropetyChanged();
                }
            }
        }

        private LatLonBoundaries _boundaries;
        [Category("Map")]
        [Description("The boundaries of map (latitude and longitude limits).")]
        [DisplayName("Boundaries")]
        [ExpandableObject]
        [PropertyOrder(0)]
        public LatLonBoundaries Boundaries
        {
            get { return _boundaries; }
            set
            {
                if (_boundaries != value)
                {
                    _boundaries = value;
                    onPropetyChanged();
                }
            }
        }

        private PixelSize _size;
        [Category("Map")]
        [Description("The size of the map (in pixels).")]
        [DisplayName("Size")]
        [ExpandableObject]
        [PropertyOrder(1)]
        public PixelSize Size
        {
            get { return _size; }
            set
            {
                if (_size != value)
                {
                    _size = value;
                    onPropetyChanged();
                }
            }
        }
        
        private object _activeObject;
        [Browsable(false)]
        public object ActiveObject
        {
            get { return _activeObject; }
            set
            {
                if (_activeObject != value)
                {
                    _activeObject = value;
                    onPropetyChanged();
                }
            }
        }

        #endregion

        #region Rendering
        
        [Browsable(false)]
        public IMapContainer Container { get; set; }

        private bool _useAntiAliasing;
        [Category("Rendering")]
        [Description("Use anti-aliasing for this map.")]
        [DisplayName("Use Anti-Aliasing")]
        [PropertyOrder(0)]
        public bool UseAntiAliasing
        {
            get { return _useAntiAliasing; }
            set
            {
                if (value != _useAntiAliasing)
                {
                    _useAntiAliasing = value;
                    onPropetyChanged();
                }
            }
        }

        #endregion

        #region File

        private string _fileName;
        [Category("General")]
        [Description("The path of the file this map is saved in.")]
        [DisplayName("File Name")]
        [ReadOnly(true)]
        public string FileName
        {
            get { return _fileName; }
            set
            {
                if (_fileName != value)
                {
                    _fileName = value;
                    onPropetyChanged();
                }
            }
        }

        private bool _isDirty;
        [Browsable(false)]
        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;
                    onPropetyChanged();
                }
            }
        }

        #endregion

        #endregion

        #endregion
        
        #region Object

        public override string ToString()
        {
            return this.Name;
        }

        #endregion
    }

    public class NewMapModel : NotifyPropertyChangedBase
    {
        #region Constructor

        public NewMapModel()
        {
            this.Name = null;
            this.Projection = this.ProjectionList.FirstOrDefault();
            this.Datum = this.DatumList.FirstOrDefault();
        }

        #endregion

        #region Properties

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    onPropetyChanged();
                }
            }
        }

        private Datum _datum;
        public Datum Datum
        {
            get { return _datum; }
            set
            {
                if (_datum != value)
                {
                    _datum = value;
                    onPropetyChanged();
                }
            }
        }

        public IEnumerable<Datum> DatumList { get { return PluginManager.Instance.Datums; } }

        private PluginItemProvider<IProjection> _projection;
        public PluginItemProvider<IProjection> Projection
        {
            get { return _projection; }
            set
            {
                if (_projection != value)
                {
                    _projection = value;
                    onPropetyChanged();
                }
            }
        }

        public IEnumerable<PluginItemProvider<IProjection>> ProjectionList { get { return PluginManager.Instance.ProjectionProviders; } }

        #endregion

        #region Functions

        public MapModel CreateMap()
        {
            var mapModel = new MapModel()
            {
                ActiveObject = null,
                Author = Environment.UserName,
                Boundaries = new LatLonBoundaries()
                {
                    CenterLatitude = 0,
                    LatitudeSpan = LatLonBoundaries.MaxLatitudeSpan,
                    CenterLongitude = 0,
                    LongitudeSpan = LatLonBoundaries.MaxLongitudeSpan
                },
                CreatedOn = DateTime.Now.ToShortDateString(),
                Datum = this.Datum,
                Description = null,
                Layers = new ObservableCollection<ILayer>(),
                License = null,
                Name = this.Name,
                Size = new PixelSize(),
                UseAntiAliasing = GeneralSettings.Instance.UseAntiAliasing,
                Version = "1.0"
            };
            mapModel.Projection = this.Projection.Create(mapModel);
            mapModel.RotateReference = GeneralSettings.Instance.RotateReference;
            return mapModel;
        }

        #endregion
    }
}
