using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartaMei.Common
{
    public static class Current
    {
        #region Map

        private static IMap _map;

        public static IMap Map
        {
            get { return _map; }
            set
            {
                if (_map != value)
                {
                    _map = value;
                    Current.MapChanged?.Invoke(null, null);
                }
            }
        }

        public static event EventHandler MapChanged;

        #endregion
    }
}
