using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace CartaMei.WPF
{
    public partial class SplitButton : Button
    {
        public SplitButton()
        {
            InitializeComponent();
        }

        protected override void OnClick()
        {
            if (this.HasChildren)
            {
                this.ContextMenu.IsEnabled = true;
                this.ContextMenu.PlacementTarget = this;
                this.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                this.ContextMenu.IsOpen = true;
            }
            else
            {
                base.OnClick();
            }
        }

        [Bindable(true)]
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { this.SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource",
            typeof(IEnumerable),
            typeof(SplitButton),
            new PropertyMetadata(default(IEnumerable), onItemsSourcePropertyChanged));

        private static void onItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var me = d as SplitButton;
            me.ContextMenu.ItemsSource = me.ItemsSource;
        }

        [Bindable(true)]
        public Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { this.SetValue(ItemContainerStyleProperty, value); }
        }
        
        public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register(
            "ItemContainerStyle",
            typeof(Style),
            typeof(SplitButton),
            new PropertyMetadata(default(IEnumerable), onItemContainerStylePropertyChanged));

        private static void onItemContainerStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var me = d as SplitButton;
            me.ContextMenu.ItemContainerStyle = me.ItemContainerStyle;
        }

        private bool HasChildren
        {
            get { return this.ItemsSource?.GetEnumerator()?.MoveNext() ?? false; }
        }
    }
}
