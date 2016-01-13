using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartaMei.Common
{
    public class Current
    {
        #region Owner

        private static Current _instance;
        private Current() { }

        public static Current Create()
        {
            if (_instance == null)
            {
                _instance = new Current();
                return _instance;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Map

        private IMap _map;

        public void SetMap(IMap value)
        {
            if (_map != value)
            {
                _map = value;
                Current.MapChanged?.Invoke(null, null);
            }
        }

        public static IMap Map { get { return _instance._map; } }

        public static event EventHandler MapChanged;

        #endregion
    }
}
