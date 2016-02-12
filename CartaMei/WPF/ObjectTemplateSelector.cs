using CartaMei.Common;
using System.Windows;
using System.Windows.Controls;

namespace CartaMei.WPF
{
    public class ObjectTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MapTemplate { get; set; }
        public DataTemplate MapObjectTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return item as IMap != null ? this.MapTemplate : this.MapObjectTemplate;
        }
    }
}
