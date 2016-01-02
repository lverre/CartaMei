using System.ComponentModel;

namespace CartaMei.Models
{
    public class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void onPropetyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
