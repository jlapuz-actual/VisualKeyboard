
namespace VisualKeyboard.Models
{
    using System.ComponentModel;
    public class UserDataModel : BaseModel, INotifyPropertyChanged
    {
        private string lastOpenedFile;

        public string LastOpenedFile
        {
            get { return lastOpenedFile; }
            set
            {
                lastOpenedFile = value;
                OnPropertyChanged( nameof( LastOpenedFile ) );
            }
        }


    }
}
