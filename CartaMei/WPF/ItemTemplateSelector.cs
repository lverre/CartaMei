using System.Windows;
using System.Windows.Controls;

namespace CartaMei.WPF
{
    public class ItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ButtonTemplate { get; set; }
        public DataTemplate SeparatorTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var button = item as Common.ButtonModel;
            return button != null && button.IsSeparator ? SeparatorTemplate : ButtonTemplate;
        }
    }
}
