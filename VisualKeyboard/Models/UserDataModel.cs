
namespace VisualKeyboard.Models
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    public class UserDataModel : BaseModel, INotifyPropertyChanged
    {
        public UserDataModel()
        {

        }
        public System.Collections.ObjectModel.ObservableCollection<string> FileHistoryCollection
        {
            get
            {
                // set if not initialized
                Properties.User.Default.FileHistoryCollection ??= new ObservableCollection<string>();
                Properties.User.Default.FileHistoryCollection.CollectionChanged -= Persist;
                Properties.User.Default.FileHistoryCollection.CollectionChanged += Persist;
                return Properties.User.Default.FileHistoryCollection;
            }
        }
        
        public double PositionX
        {
            get => Properties.User.Default.PositionX;
            set
            {
                Properties.User.Default.PositionX = value;
                OnPropertyChanged(nameof (PositionX));
                Persist(null, null);
            }
        }
        public double PositionY
        {
            get => Properties.User.Default.PositionY;
            set
            {
                Properties.User.Default.PositionY = value;
                OnPropertyChanged(nameof (PositionY));
                Persist(null, null);
            }
        }public double WindowSizeX
        {
            get => Properties.User.Default.WindowSizeX;
            set
            {
                Properties.User.Default.WindowSizeX = value;
                OnPropertyChanged(nameof (WindowSizeX));
                Persist(null, null);
            }
        }
        public double WindowSizeY
        {
            get => Properties.User.Default.WindowSizeY;
            set
            {
                Properties.User.Default.WindowSizeY = value;
                OnPropertyChanged(nameof (WindowSizeY));
                Persist(null, null);
            }
        } public bool RememberLastPosition
        {
            get => Properties.User.Default.RememberLastPosition;
            set
            {
                Properties.User.Default.RememberLastPosition = value;
                OnPropertyChanged(nameof (RememberLastPosition));
                Persist(null, null);
            }
        }
        private void Persist(object sender, EventArgs eventArgs)
        {
            Properties.User.Default.Save();
        }
    }
}
