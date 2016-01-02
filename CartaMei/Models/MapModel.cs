using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartaMei.Models
{
    public class MapModel : NotifyPropertyChangedBase
    {
        #region Properties

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    onPropetyChanged();
                }
            }
        }

        #endregion

    }
}
