namespace VisualKeyboard.Models
{
    using System.ComponentModel;
    public class ButtonModel : BaseModel, INotifyPropertyChanged
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
                OnPropertyChanged(nameof(Label));
            }
        }

        public int ColumnCoord
        {
            get { return columnCoord; }
            set
            {
                columnCoord = value;
                OnPropertyChanged(nameof(ColumnCoord));
            }
        }

        public int RowCoord
        {
            get { return rowCoord; }
            set
            {
                rowCoord = value;
                OnPropertyChanged(nameof(RowCoord));
            }
        }

        public int ColSpan
        {
            get { return colSpan; }
            set
            {
                colSpan = value;
                OnPropertyChanged(nameof(ColSpan));
            }
        }

        public int RowSpan
        {
            get { return rowSpan; }
            set
            {
                rowSpan = value;
                OnPropertyChanged(nameof(RowSpan));
            }
        }

        public string ActionParam
        {
            get { return actionParam; }
            set
            {
                actionParam = value;
                OnPropertyChanged(nameof(ActionParam));
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
    }
}
