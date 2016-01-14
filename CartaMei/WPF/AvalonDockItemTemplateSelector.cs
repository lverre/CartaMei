using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CartaMei.WPF
{
    public class AvalonDockItemTemplateSelector : DataTemplateSelector
    {
        public IDictionary<Type, DataTemplate> Templates { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate template = null;
            var type = item?.GetType();
            return item != null && this.Templates.TryGetValue((Type)type, out template) ? template : null;
        }
    }
}
