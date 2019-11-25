
namespace VisualKeyboard.Models
{
    using System;
    using System.ComponentModel;
    public class BaseModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members
        [field: NonSerialized()]
        public event PropertyChangedEventHandler PropertyChanged;
        internal void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
