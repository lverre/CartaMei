using CartaMei.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartaMei.Models
{
    public class MapModel : NotifyPropertyChangedBase, IMap
    {
        #region IMap

        #region Properties

        #region Metadata

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
        
        private string _description;
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
        
        public ICollection<ILayer> Layers
        {
            get { return this.OLayers; }
            set
            {
                if (this.OLayers != value)
                {
                    var olayer = value as ObservableCollection<ILayer>;
                    if (olayer == null && value != null)
                    {
                        olayer = new ObservableCollection<ILayer>(value);
                    }
                    this.OLayers = olayer;
                }
            }
        }

        private LatLonBoundaries _boundaries;
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

        #endregion

        #region Misc

        private string _fileName;
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

        #endregion

        #endregion

        #endregion

        #region Properties

        private ObservableCollection<ILayer> _layers;
        public ObservableCollection<ILayer> OLayers
        {
            get { return _layers; }
            set
            {
                if (_layers != value)
                {
                    _layers = value;
                    onPropetyChanged();
                    onPropetyChanged("Layers");
                }
            }
        }

        private object _activeObject;
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
    }

    public class NewMapModel : NotifyPropertyChangedBase
    {
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

        public IEnumerable<Datum> Datums { get { return PluginManager.Instance.Datums; } }

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

        public IEnumerable<PluginItemProvider<IProjection>> Projections { get { return PluginManager.Instance.ProjectionProviders; } }

        #endregion

        #region Functions

        public MapModel CreateMap()
        {
            return new MapModel()
            {
                ActiveObject = null,
                Author = Environment.UserName,
                Boundaries = new LatLonBoundaries(-90, 90, -180, 180),
                CreatedOn = DateTime.Now.ToShortDateString(),
                Datum = this.Datum,
                Description = null,
                Layers = new List<ILayer>(),
                License = null,
                Name = this.Name,
                Projection = this.Projection.Create(),
                Version = "1.0"
            };
        }

        #endregion
    }
}
