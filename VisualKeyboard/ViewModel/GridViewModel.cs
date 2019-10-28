namespace VisualKeyboard.ViewModel
{
    using Microsoft.Win32;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interop;
    using VisualKeyboard.Models;
    using VisualKeyboard.Utilities;

    class GridViewModel : INotifyPropertyChanged
    {
        private bool NoActive;
        private GridModel grid;
        private InputSender sender;

        public GridModel GridModel
        {
            get { return grid; }
            set
            {
                grid = value;
                OnPropertyChanged("GridModel");
            }
        }

        public Window Window { get; set; }

        #region Commands
        public RelayCommand OpenDialogCommand { get; private set; }
        public RelayCommand ButtonActionCommand { get; private set; }
        public RelayCommand DebugViewModelCommand { get; private set; }
        public RelayCommand ToggleWindowActiveCommand { get; private set; }
        public RelayCommand LoadDefaultConfigurationCommand { get; private set; }
        #endregion

        public GridViewModel()
        {
            this.GridModel = new GridModel();
            this.sender = new InputSender();
            OpenDialogCommand = new RelayCommand(param => this.RequestFileDialog());
            ButtonActionCommand = new RelayCommand(param => this.ButtonAction(param));
            DebugViewModelCommand = new RelayCommand(param => this.DebugViewModel());
            ToggleWindowActiveCommand = new RelayCommand(param => this.ToggleWindowActive(), param => this.Window != null);
            LoadDefaultConfigurationCommand = new RelayCommand(param =>this.LoadDefaults());

                        NoActive = false;
        }

        public void LoadDefaults()
        {
            (int, int, int, int, ushort)[] ps = new (int, int, int, int, ushort)[] 
            { 
                (0, 0, 1, 1, 0x24), 
                (1, 0, 2, 2, 0x24), 
                (0, 1, 1, 1, 0x24), 
                (0, 2, 1, 1, 0x24), 
                (5, 2, 2, 1, 0x24), 
                (1, 2, 4, 1, 0x24), 
                (2, 3, 1, 1, 0x24), 
            };
            ObservableCollection<ButtonModel> buttonModels = new ObservableCollection<ButtonModel>();

            foreach (var (row, col, rowSpan, colSpan, actionParam) in ps)
            {
                buttonModels.Add(new ButtonModel()
                {
                    Label = "button " + row + " " + col,
                    RowCoord = row,
                    ColumnCoord = col,
                    RowSpan = rowSpan,
                    ColSpan = colSpan,
                    ActionParam = actionParam
                });
            }

            GridModel = new GridModel()
            {
                DefinedColumns = "*,*,*,*",
                DefinedRows = "*,*,*,*,*,*,*,*,*,*",
                ButtonModels = buttonModels
            };
        }
        private void RequestFileDialog()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                FileName = "", // Default file name
                DefaultExt = ".yml", // Default file extension
                Filter = "Text documents (.yml)|*.yml" // Filter files by extension
            };

            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result != true)
            {
                return;
            }

            string filename = dialog.FileName;
            FileOps fo = new FileOps();
            var yml = new YmlFileManager();

            var fileRead = fo.Load(filename);
            var graph = yml.GetObject(fileRead);
            this.GridModel = (GridModel)graph;

        }

        private void WriteFileDialog()
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                DefaultExt = ".yml", // Default file extension
                FileName = "NewYMLDocument" // Default file name
            };
        }

        private void DebugViewModel()
        {
            Debug.WriteLine(this.GridModel.DefinedColumns);
            Debug.WriteLine(this.GridModel.DefinedRows);
            foreach (var item in this.GridModel.ButtonModels)
            {
                Debug.WriteLine(item.Label);
            }
        }

        private void ButtonAction(object param)
        {
            var parameter = (ButtonModel)param;
            Debug.WriteLine(parameter.ActionParam);
            sender.SendScanKeyUp(new ushort[] { (ushort)parameter.ActionParam });
            sender.SendScanKeyDown(new ushort[] { (ushort)parameter.ActionParam });
        }

        #region Extended Window Styles members

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        private const int GWL_EXSTYLE = (-20);
        private const int WS_EX_NOACTIVATE = 0x08000000;

        #endregion
        private void ToggleWindowActive()
        {
            WindowInteropHelper helper = new WindowInteropHelper(this.Window);
            
            int dwExStyle = GetWindowLong(helper.Handle, GWL_EXSTYLE);
            if (!NoActive)
            {
                dwExStyle |= WS_EX_NOACTIVATE;
            }
            else
            {
                dwExStyle &= ~WS_EX_NOACTIVATE;
            }
            Debug.Write($"toggling noactivate, was {NoActive}, ");
            NoActive = !NoActive;
            Debug.WriteLine($"now {NoActive}");
            SetWindowLong(helper.Handle, GWL_EXSTYLE, dwExStyle);

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
