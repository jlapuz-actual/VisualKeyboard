namespace VisualKeyboard.Models
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    public class GridModel : BaseModel, INotifyPropertyChanged
    {
        private string name;
        private string definedRows;
        private string definedColumns;
        private ObservableCollection<ButtonModel> buttonModels;

        public GridModel()
        {
            name = "";
            definedColumns = "*";
            definedRows = "*";
            buttonModels = new ObservableCollection<ButtonModel>();
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged( nameof( Name ) );
            }
        }

        public string DefinedRows
        {
            get { return definedRows; }
            set
            {
                definedRows = value;
                OnPropertyChanged( nameof( DefinedRows ) );
            }
        }

        public string DefinedColumns
        {
            get { return definedColumns; }
            set
            {
                definedColumns = value;
                OnPropertyChanged( nameof( DefinedColumns ) );
            }
        }

        public ObservableCollection<ButtonModel> ButtonModels
        {
            get { return buttonModels; }
            set
            {
                buttonModels = value;
                OnPropertyChanged( nameof( ButtonModels ) );
            }
        }

        public void LoadData( object data )
        {
            if ( data is null ) return;
            var newGridData = (GridModel)data;

            this.Name = newGridData.Name;
            this.ButtonModels = newGridData.ButtonModels;
            this.DefinedColumns = newGridData.DefinedColumns;
            this.DefinedRows = newGridData.DefinedRows;

        }
    }
}
