using CartaMei.Common;
using CartaMei.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartaMei.Tools
{
    internal static class Utils
    {
        static Utils()
        {
            Utils.Current = Current.Create();
        }

        internal static void Touch() { }

        internal static Current Current { get; private set; }

        internal static MainWindow MainWindow { get; set; }

        internal static MainWindowModel MainWindowModel { get; set; }
    }
}
