namespace VisualKeyboard.Models
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    class GridModel
    {
        private string definedRows;
        private string definedColumns;
        private ObservableCollection<ButtonModel> buttonModels;

        public GridModel()
        {
            definedColumns = "*";
            definedRows = "*";
            buttonModels = new ObservableCollection<ButtonModel>();
        }
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value;
                OnPropertyChanged("Name");
            }
        }

        public string DefinedRows
        {
            get { return definedRows; }
            set
            {
                definedRows = value;
                OnPropertyChanged("DefinedRow");
            }
        }

        public string DefinedColumns
        {
            get { return definedColumns; }
            set
            {
                definedColumns = value;
                OnPropertyChanged("DefinedColumn");
            }
        }

        public ObservableCollection<ButtonModel> ButtonModels
        {
            get { return buttonModels; }
            set
            {
                buttonModels = value;
                OnPropertyChanged("ButtonModels");
            }
        }

        public void LoadData(object data)
        {
            var newGridData = (GridModel)data;

            this.Name = newGridData.Name;
            this.ButtonModels = newGridData.ButtonModels;
            this.DefinedColumns = newGridData.DefinedColumns;
            this.DefinedRows = newGridData.DefinedRows;

        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
