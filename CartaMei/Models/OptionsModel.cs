using CartaMei.Common;
using System.Collections.Generic;

namespace CartaMei.Models
{
    public class OptionsModel : NotifyPropertyChangedBase
    {
        #region Properties

        private object _selectedObject;
        public object SelectedObject
        {
            get { return _selectedObject; }
            set
            {
                if (value != _selectedObject)
                {
                    _selectedObject = value;
                    onPropetyChanged();
                }
            }
        }
        
        public IEnumerable<OptionItem> Items { get; set; }

        #endregion
    }

    public class OptionItem
    {
        #region Properties

        public string Name { get; set; }

        public object Properties { get; set; }

        public IEnumerable<OptionItem> Children { get; set; }

        #endregion
    }
}
