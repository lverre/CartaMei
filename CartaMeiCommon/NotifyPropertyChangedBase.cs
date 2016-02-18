using System.ComponentModel;
using System.Xml.Serialization;

namespace CartaMei.Common
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

        #region Tools

        protected void onAllPropertiesChanged()
        {
            foreach (var property in this.GetType().GetPublicSetters())
            {
                onPropetyChanged(property.Name);
            }
        }

        #endregion
    }
}
