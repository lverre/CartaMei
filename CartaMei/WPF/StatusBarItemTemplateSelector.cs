using System.Windows;
using CartaMei.Common;

namespace CartaMei.WPF
{
    public class StatusBarItemTemplateSelector : DictionaryTemplateSelector
    {
        public DataTemplate SeparatorTemplate { get; set; }
        public DataTemplate TextTemplate { get; set; }
        public DataTemplate ProgressBarTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var result = base.SelectTemplate(item, container);
            if (result != null)
            {
                return result;
            }
            else if (item as SeparatorStatusItem != null)
            {
                return SeparatorTemplate;
            }
            else if (item as ITextStatusItem != null)
            {
                return TextTemplate;
            }
            else if (item as IProgressBarStatusItem != null)
            {
                return ProgressBarTemplate;
            }
            else
            {
                return null;
            }
        }
    }
}
