using CartaMei.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartaMei.Models
{
    public class PropertiesPanelModel : NotifyPropertyChangedBase, IAnchorableTool
    {
        #region Constructor

        public PropertiesPanelModel()
        {
            Current.MapChanged += delegate (CurrentPropertyChangedEventArgs<IMap> e1)
            {
                if (e1.NewValue != null)
                {
                    e1.NewValue.PropertyChanged += delegate (object s2, System.ComponentModel.PropertyChangedEventArgs e2)
                    {
                        if (e2.PropertyName == nameof(MapModel.ActiveObject))
                        {
                            onPropetyChanged(nameof(PropertiesPanelModel.SelectedObject));
                        }
                    };
                }
                onPropetyChanged(nameof(PropertiesPanelModel.SelectedObject));
            };
        }
        
        #endregion

        #region IAnchorableTool

        public string Name
        {
            get { return "Properties"; }
        }

        #endregion

        #region Properties

        public object SelectedObject { get { return Current.Map?.ActiveObject; } }

        #endregion
    }
}
