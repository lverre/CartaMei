using CartaMei.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CartaMei.Templates
{
    public partial class ObjectsTemplate : UserControl
    {
        public ObjectsTemplate()
        {
            InitializeComponent();
        }

        private void selectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var map = Current.Map;
            if (map != null) map.ActiveObject = _treeView.SelectedItem;
        }
    }
}
