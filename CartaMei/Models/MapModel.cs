using CartaMei.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

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
        [PropertyOrder(2)]
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
        [ReadOnly(true)]
        [PropertyOrder(1)]
        public IProjection Projection
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
            return new MapModel()
            {
                ActiveObject = null,
                Author = Environment.UserName,
                Boundaries = new LatLonBoundaries()
                {
                    LatMax = 90,
                    LatMin = -90,
                    LonMax = 180,
                    LonMin = -180
                },
                CreatedOn = DateTime.Now.ToShortDateString(),
                Datum = this.Datum,
                Description = null,
                Layers = new ObservableCollection<ILayer>(),
                License = null,
                Name = this.Name,
                Projection = this.Projection.Create(),
                Version = "1.0"
            };
        }

        #endregion
    }
}
