using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CartaMei.WPF
{
    public class DictionaryTemplateSelector : DataTemplateSelector
    {
        public IDictionary<Type, DataTemplate> Templates { get; set; }

        public DataTemplate DefaultTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
            {
                return null;
            }

            DataTemplate template = null;
            var type = item?.GetType();
            if (this.Templates != null)
            {
                if (this.Templates.TryGetValue((Type)type, out template))
                {
                    return template;
                }
                else
                {
                    template = (from pair in this.Templates where type.IsAssignableFrom(pair.Key) select pair.Value).FirstOrDefault();
                    return template ?? this.DefaultTemplate;
                }
            }
            else
            {
                return this.DefaultTemplate;
            }
        }
    }
}
