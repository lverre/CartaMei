using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CartaMei.Common
{
    public interface IMap : IDockElement, INotifyPropertyChanged
    {
        #region Properties

        #region Metadata
        
        string Description { get; set; }

        string Author { get; set; }

        string CreatedOn { get; set; }

        string License { get; set; }

        string Version { get; set; }

        #endregion

        #region Map

        Datum Datum { get; set; }

        IProjection Projection { get; set; }

        ObservableCollection<ILayer> Layers { get; set; }

        LatLonBoundaries Boundaries { get; set; }

        PixelSize Size { get; set; }
        
        object ActiveObject { get; set; }

        #endregion

        #region Rendering

        bool UseAntiAliasing { get; set; }

        #endregion

        #region File

        string FileName { get; set; }

        bool IsDirty { get; set; }

        #endregion

        #endregion
    }
}
