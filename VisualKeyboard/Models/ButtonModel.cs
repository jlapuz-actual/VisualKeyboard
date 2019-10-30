namespace VisualKeyboard.Models
{
    using System.ComponentModel;
    class ButtonModel : INotifyPropertyChanged
    {
        private string label;
        private int colSpan;
        private int rowSpan;
        private int columnCoord;
        private int rowCoord;
        private string actionParam;

        public string Label
        {
            get { return label; }
            set
            {
                label = value;
                OnPropertyChanged("Content");
            }
        }

        public int ColumnCoord
        {
            get { return columnCoord; }
            set
            {
                columnCoord = value;
                OnPropertyChanged("ColumnCoord");
            }
        }

        public int RowCoord
        {
            get { return rowCoord; }
            set
            {
                rowCoord = value;
                OnPropertyChanged("RowCoord");
            }
        }

        public int ColSpan
        {
            get { return colSpan; }
            set
            {
                colSpan = value;
                OnPropertyChanged("ColSpan");
            }
        }

        public int RowSpan
        {
            get { return rowSpan; }
            set
            {
                rowSpan = value;
                OnPropertyChanged("RowSpan");
            }
        }

        public string ActionParam
        {
            get { return actionParam; }
            set
            {
                actionParam = value;
                OnPropertyChanged("ActionParam");
            }
        }

        public ButtonModel()
        {
            label = "default";
            colSpan = 1;
            rowSpan = 1;
            columnCoord = 0;
            rowCoord = 0;
            actionParam = "";
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
