using System.Windows;

namespace CartaMei.Dialogs
{
    public partial class Options : Window
    {
        public Options()
        {
            InitializeComponent();
        }

        private void selectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            (this.DataContext as Models.OptionsModel).SelectedObject = (_treeView.SelectedItem as Models.OptionItem)?.Properties;
        }

        private void onOk(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
