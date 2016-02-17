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
            var model = this.DataContext as Models.OptionsModel;
            var selectedItem = _treeView.SelectedItem as Models.OptionItem;
            model.SelectedObject = selectedItem?.Properties;
            model.SelectedName = selectedItem.Name;
        }

        private void onOk(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
