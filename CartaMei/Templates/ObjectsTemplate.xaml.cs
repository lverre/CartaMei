using CartaMei.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

            PropertyChangedEventHandler mapPropertyChanged = delegate (object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(IMap.ActiveObject))
                {
                    var treeViewItem = getTreeViewItem(Current.Map.ActiveObject, _treeView) as TreeViewItem;
                    if (treeViewItem != null) treeViewItem.IsSelected = true;
                }
            };
            CurrentPropertyChangedEventHandler<IMap> mapChanged = delegate (CurrentPropertyChangedEventArgs<IMap> e)
            {
                if (e.OldValue != null)
                {
                    e.OldValue.PropertyChanged -= mapPropertyChanged;
                }
                if (e.NewValue != null)
                {
                    e.NewValue.PropertyChanged += mapPropertyChanged;
                }
                mapPropertyChanged(e.NewValue, new PropertyChangedEventArgs(nameof(IMap.ActiveObject)));
            };
            Current.MapChanged += mapChanged;
        }

        private DependencyObject getTreeViewItem(object value, ItemsControl container)
        {
            if (value == null || container == null) return null;

            var generator = container.ItemContainerGenerator;
            var item = generator.ContainerFromItem(value);
            if (item != null)
            {
                return item;
            }
            else
            {
                foreach (var child in generator.Items)
                {
                    var childContainer = generator.ContainerFromItem(child);
                    var result = getTreeViewItem(value, childContainer as ItemsControl);
                    if (result != null) return result;
                }
            }

            return null;
        }

        private void selectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var map = Current.Map;
            if (map != null) map.ActiveObject = _treeView.SelectedItem;
        }
    }
}
