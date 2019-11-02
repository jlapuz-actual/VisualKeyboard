namespace VisualKeyboard.ViewModel
{
    using Microsoft.Win32;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
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
        private string CurrentFile;
        private string SafeCurrentFile;

        public GridModel GridModel
        {
            get { return grid; }
            set
            {
                grid = value;
                OnPropertyChanged(nameof( GridModel ) );
            }
        }

        public Window Window { get; set; }

        #region Commands
        public RelayCommand OpenDialogCommand { get; private set; }
        public RelayCommand WriteDialogCommand { get; private set; }
        public RelayCommand ButtonActionCommand { get; private set; }
        public RelayCommand DebugViewModelCommand { get; private set; }
        public RelayCommand ReloadFileFromDiskCommand { get; private set; }
        public RelayCommand ToggleWindowActiveCommand { get; private set; }
        public RelayCommand LoadDefaultConfigurationCommand { get; private set; }
        #endregion

        public GridViewModel()
        {
            this.GridModel = new GridModel();
            this.sender = new InputSender();
            OpenDialogCommand = new RelayCommand(param => this.RequestFileDialog());
            WriteDialogCommand = new RelayCommand(param => this.WriteFileDialog());
            ButtonActionCommand = new RelayCommand(param => this.ButtonAction(param));
            DebugViewModelCommand = new RelayCommand(param => this.DebugViewModel());
            ReloadFileFromDiskCommand = new RelayCommand(param => this.ReloadFileFromDisk());
            ToggleWindowActiveCommand = new RelayCommand(param => this.ToggleWindowActive(), param => this.Window != null);
            LoadDefaultConfigurationCommand = new RelayCommand(param => this.LoadDefaults());

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

            if(SetGridModelFromFile(dialog.FileName) != 0)
            {
                return;
            }

            SafeCurrentFile = dialog.SafeFileName;
            CurrentFile= dialog.FileName;
        }

        private void ReloadFileFromDisk() 
        {
            SetGridModelFromFile(this.CurrentFile);
        }

        private int SetGridModelFromFile(string file)
        {
            FileOps fo = new FileOps();
            var yml = new YmlFileManager();
            object graph;
            try
            {
                var fileRead = fo.Load(file);
                graph = yml.GetObject(fileRead);
            }
            catch (Exception ex)
            {
                int returnValue = 2;
                switch (ex)
                {
                    case YamlDotNet.Core.YamlException YMLex:
                        Debug.WriteLine($"exception thrown");
                        Debug.WriteLine($"message: {YMLex.Message}");
                        Debug.WriteLine($"source: {YMLex.Source}");
                        Debug.WriteLine($"start: {YMLex.Start}");
                        Debug.WriteLine($"start: {YMLex.Start}");
                        returnValue = 1;
                        break;
                    case FileNotFoundException FNFex:
                        Debug.WriteLine($"exception thrown");
                        Debug.WriteLine($"message: {FNFex.Message}");
                        break;
                    case DirectoryNotFoundException DNFex:
                        Debug.WriteLine($"exception thrown");
                        Debug.WriteLine($"message: {DNFex.Message}");
                        break;
                    case OutOfMemoryException OOMex:
                        Debug.WriteLine($"exception thrown");
                        Debug.WriteLine($"message: {OOMex.Message}");
                        break;
                    case IOException IOex:
                        Debug.WriteLine($"exception thrown");
                        Debug.WriteLine($"message: {IOex.Message}");
                        break;

                    default:
                        break;
                }
                return returnValue;
            }
            this.GridModel = (GridModel)graph;
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
            FileOps fo = new FileOps();
            var yml = new YmlFileManager();

            var graph = yml.GetYML(this.GridModel);
            fo.Write(graph,filename);
            
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
            sender.SendScanKeyUp(InputSender.PlainTextToScanCodes[parameter.ActionParam]);
            sender.SendScanKeyDown(InputSender.PlainTextToScanCodes[parameter.ActionParam]);
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
