namespace VisualKeyboard.ViewModel
{
    using Microsoft.Win32;
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Windows;
    using System.Windows.Interop;
    using VisualKeyboard.Models;
    using VisualKeyboard.Utilities;
    using static VisualKeyboard.Utilities.Native.NativeMethods;

    class GridViewModel : BaseModel
    {
        private bool noActive;
        private GridModel grid;
        private string CurrentFile;
        private string SafeCurrentFile;
        private UserDataModel udModel;

        public UserDataModel UserDataModel
        {
            get { return udModel; }
            set
            {
                udModel = value;
                OnPropertyChanged(nameof(UserDataModel));
            }
        }

        public bool NoActive
        {
            get => noActive;
            set
            {
                noActive = value;
                OnPropertyChanged(nameof(NoActive));
            }
        }

        public GridModel GridModel
        {
            get { return grid; }
            set
            {
                grid = value;
                OnPropertyChanged(nameof(GridModel));
            }
        }

        public Window Window { get; set; }

        #region Command properties
        public RelayCommand OpenDialogCommand { get; private set; }
        public RelayCommand WriteDialogCommand { get; private set; }
        public RelayCommand ButtonActionCommand { get; private set; }
        public RelayCommand DebugViewModelCommand { get; private set; }
        public RelayCommand ReloadFileFromDiskCommand { get; private set; }
        public RelayCommand ToggleWindowActiveCommand { get; private set; }
        public RelayCommand RefreshConfigurationCommand { get; private set; }
        public RelayCommand ReloadFileFromHistoryCommand { get; private set; }
        public RelayCommand LoadDefaultConfigurationCommand { get; private set; }
        public RelayCommand ClearConfigurationHistoryCommand { get; private set; }
        #endregion

        public GridViewModel()
        {
            this.GridModel = new GridModel();
            this.UserDataModel = new UserDataModel();
            OpenDialogCommand = new RelayCommand(param => this.RequestFileDialog());
            WriteDialogCommand = new RelayCommand(param => this.WriteFileDialog());
            ButtonActionCommand = new RelayCommand(param => this.ButtonAction(param));
            DebugViewModelCommand = new RelayCommand(param => this.DebugViewModel());
            ToggleWindowActiveCommand = new RelayCommand(param => this.ToggleWindowActive(), param => this.Window != null);
            RefreshConfigurationCommand = new RelayCommand(param => RefreshConfiguration(), param => this.CurrentFile != null);
            ReloadFileFromHistoryCommand = new RelayCommand(param => this.ReloadFileFromHistory(param));
            LoadDefaultConfigurationCommand = new RelayCommand(param => this.LoadDefaults());
            ClearConfigurationHistoryCommand = new RelayCommand(param => ClearConfigurationHistory());

            NoActive = false;
        }

        public void LoadDefaults()
        {
            (string, int, int, int, int, string)[] ps = new (string, int, int, int, int, string)[]
            {
                ("A",0, 0, 1, 1, "A"),
                ("S",1, 0, 2, 2, "S"),
                ("D",0, 1, 1, 1, "D"),
                ("F",0, 2, 1, 1, "F"),
                ("G",5, 2, 2, 1, "G"),
                ("H",1, 2, 4, 1, "H"),
                ("J",2, 3, 1, 1, "J"),
                ("UPARROW",8, 2, 1, 1, "UPARROW"),
                ("LEFTARROW",9, 1, 1, 1, "LEFTARROW"),
                ("DOWNARROW",9, 2, 1, 1, "DOWNARROW"),
                ("RIGHTARROW",9, 3, 1, 1, "RIGHTARROW"),
            };
            ObservableCollection<ButtonModel> buttonModels = new ObservableCollection<ButtonModel>();

            foreach (var (label, row, col, rowSpan, colSpan, actionParam) in ps)
            {
                buttonModels.Add(new ButtonModel()
                {
                    Label = label,
                    RowCoord = row,
                    ColumnCoord = col,
                    RowSpan = rowSpan,
                    ColSpan = colSpan,
                    ActionParam = actionParam
                });
            }

            GridModel = new GridModel()
            {
                Name = "ExampleBoard",
                DefinedColumns = "*,*,*,*",
                DefinedRows = "*,*,*,*,*,*,*,*,*,*",
                ButtonModels = buttonModels
            };
        }

        private void RequestFileDialog()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                FileName = SafeCurrentFile, // Default file name
                DefaultExt = ".yml", // Default file extension
                Filter = "Text documents (.yml)|*.yml", // Filter files by extension
                Multiselect = false,
            };

            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result != true)
            {
                return;
            }

            if (SetGridModelFromFile(dialog.FileName) != 0)
            {
                return;
            }

            SafeCurrentFile = dialog.SafeFileName;
            CurrentFile = dialog.FileName;
        }

        private void ClearConfigurationHistory()
        {
            UserDataModel.FileHistoryCollection.Clear();
        }

        private void ReloadFileFromHistory(object file)
        {
            var targetFile = (string)file ?? this.UserDataModel.FileHistoryCollection[0];
            SetGridModelFromFile(targetFile);
        }
        private void RefreshConfiguration()
        {
            SetGridModelFromFile(this.CurrentFile);
        }

        private int SetGridModelFromFile(string file)
        {
            var yml = new YmlFileManager();
            object graph;
            try
            {
                var fileRead = FileOps.Load(file);
                if (fileRead is null)
                {
                    Debug.WriteLine("not loaded");
                    return -1;

                }

                graph = yml.GetObject(fileRead);
            }
            catch (IOException IOex)
            {
                Debug.WriteLine($"exception thrown");
                Debug.WriteLine($"message: {IOex.Message}");
                return 1;
            }
            this.GridModel = (GridModel)graph;
            UserDataModel.FileHistoryCollection.Remove(file); // doesn't matter that the listed item exists or not, it will remove if it can
            UserDataModel.FileHistoryCollection.Insert(0, file);

            return 0;
        }

        private void WriteFileDialog()
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                DefaultExt = ".yml", // Default file extension
                FileName = CurrentFile // Default file name
            };

            var result = dialog.ShowDialog();

            if (result != true)
            {
                return;
            }

            string filename = dialog.FileName;
            var yml = new YmlFileManager();

            var graph = yml.GetYML(this.GridModel);
            FileOps.Write(graph, filename);

        }

        private void DebugViewModel()
        {
            Debug.WriteLine(this.GridModel.DefinedColumns);
            Debug.WriteLine(this.GridModel.DefinedRows);
            foreach (var item in this.GridModel.ButtonModels)
            {
                Debug.WriteLine(item.Label);
            }
            if (IntPtr.Size == 8)
            {
                Debug.WriteLine("x64 build");
            }
            else
            {
                Debug.WriteLine("x32 build");
            }
        }

        private void ButtonAction(object param)
        {
            var parameter = (ButtonModel)param;
            Debug.WriteLine(parameter.ActionParam);
            InputSender.SendScanKeyUp(InputSender.PlainTextToScanCodes[parameter.ActionParam]);
            InputSender.SendScanKeyDown(InputSender.PlainTextToScanCodes[parameter.ActionParam]);
        }

        private void ToggleWindowActive()
        {
            WindowInteropHelper helper = new WindowInteropHelper(this.Window);

            IntPtr dwExStyle = GetWindowLongPtr(helper.Handle, GWL.EXSTYLE);
            //Debug.WriteLine( $"style value before {dwExStyle.ToInt64()}" );
            if (!NoActive)
            {
                dwExStyle = IntPtr.Add(dwExStyle, WS_EX_NOACTIVATE);
            }
            else
            {
                dwExStyle = IntPtr.Subtract(dwExStyle, WS_EX_NOACTIVATE);
            }
            //Debug.WriteLine( $"style value after {dwExStyle.ToInt64()}" );
            //Debug.Write( $"toggling value:NoActive, was {NoActive}, " );
            NoActive = !NoActive;
            //Debug.WriteLine( $"now {NoActive}" );
            SetWindowLongPtr(helper.Handle, GWL.EXSTYLE, dwExStyle);

        }
    }
}
