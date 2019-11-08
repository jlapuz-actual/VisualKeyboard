namespace VisualKeyboard.ViewModel
{
    using Microsoft.Win32;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Windows;
    using System.Windows.Interop;
    using VisualKeyboard.Models;
    using VisualKeyboard.Utilities;
    using static VisualKeyboard.Utilities.Native.NativeMethods;

    class GridViewModel : INotifyPropertyChanged
    {
        private bool NoActive;
        private GridModel grid;
        private string CurrentFile;
        private string SafeCurrentFile;

        public GridModel GridModel
        {
            get { return grid; }
            set
            {
                grid = value;
                OnPropertyChanged( nameof( GridModel ) );
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
        public RelayCommand LoadDefaultConfigurationCommand { get; private set; }
        #endregion

        public GridViewModel()
        {
            this.GridModel = new GridModel();
            OpenDialogCommand = new RelayCommand( param => this.RequestFileDialog() );
            WriteDialogCommand = new RelayCommand( param => this.WriteFileDialog() );
            ButtonActionCommand = new RelayCommand( param => this.ButtonAction( param ) );
            DebugViewModelCommand = new RelayCommand( param => this.DebugViewModel() );
            ReloadFileFromDiskCommand = new RelayCommand( param => this.ReloadFileFromDisk() );
            ToggleWindowActiveCommand = new RelayCommand( param => this.ToggleWindowActive(), param => this.Window != null );
            LoadDefaultConfigurationCommand = new RelayCommand( param => this.LoadDefaults() );

            NoActive = false;
        }

        public void LoadDefaults()
        {
            (string, int, int, int, int, string) [] ps = new (string, int, int, int, int, string) []
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

            foreach ( var (label, row, col, rowSpan, colSpan, actionParam) in ps )
            {
                buttonModels.Add( new ButtonModel()
                {
                    Label = label,
                    RowCoord = row,
                    ColumnCoord = col,
                    RowSpan = rowSpan,
                    ColSpan = colSpan,
                    ActionParam = actionParam
                } );
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
            if ( result != true )
            {
                return;
            }

            if ( SetGridModelFromFile( dialog.FileName ) != 0 )
            {
                return;
            }

            SafeCurrentFile = dialog.SafeFileName;
            CurrentFile = dialog.FileName;
        }

        private void ReloadFileFromDisk()
        {
            SetGridModelFromFile( this.CurrentFile );
        }

        private int SetGridModelFromFile( string file )
        {
            var yml = new YmlFileManager();
            object graph;
            try
            {
                var fileRead = FileOps.Load( file );
                graph = yml.GetObject( fileRead );
            }
            catch ( Exception ex )
            {
                int returnValue = 2;
                switch ( ex )
                {
                    case YamlDotNet.Core.YamlException YMLex:
                        Debug.WriteLine( $"exception thrown" );
                        Debug.WriteLine( $"message: {YMLex.Message}" );
                        Debug.WriteLine( $"source: {YMLex.Source}" );
                        Debug.WriteLine( $"start: {YMLex.Start}" );
                        Debug.WriteLine( $"start: {YMLex.Start}" );
                        returnValue = 1;
                        break;
                    case FileNotFoundException FNFex:
                        Debug.WriteLine( $"exception thrown" );
                        Debug.WriteLine( $"message: {FNFex.Message}" );
                        break;
                    case DirectoryNotFoundException DNFex:
                        Debug.WriteLine( $"exception thrown" );
                        Debug.WriteLine( $"message: {DNFex.Message}" );
                        break;
                    case OutOfMemoryException OOMex:
                        Debug.WriteLine( $"exception thrown" );
                        Debug.WriteLine( $"message: {OOMex.Message}" );
                        break;
                    case IOException IOex:
                        Debug.WriteLine( $"exception thrown" );
                        Debug.WriteLine( $"message: {IOex.Message}" );
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

            if ( result != true )
            {
                return;
            }

            string filename = dialog.FileName;
            var yml = new YmlFileManager();

            var graph = yml.GetYML( this.GridModel );
            FileOps.Write( graph, filename );

        }

        private void DebugViewModel()
        {
            Debug.WriteLine( this.GridModel.DefinedColumns );
            Debug.WriteLine( this.GridModel.DefinedRows );
            foreach ( var item in this.GridModel.ButtonModels )
            {
                Debug.WriteLine( item.Label );
            }
            if ( IntPtr.Size == 8 )
            {
                Debug.WriteLine( "x64 build" );
            }
            else
            {
                Debug.WriteLine( "x32 build" );
            }
        }

        private void ButtonAction( object param )
        {
            var parameter = (ButtonModel)param;
            Debug.WriteLine( parameter.ActionParam );
            InputSender.SendScanKeyUp( InputSender.PlainTextToScanCodes [ parameter.ActionParam ] );
            InputSender.SendScanKeyDown( InputSender.PlainTextToScanCodes [ parameter.ActionParam ] );
        }

        private void ToggleWindowActive()
        {
            WindowInteropHelper helper = new WindowInteropHelper( this.Window );

            IntPtr dwExStyle = GetWindowLongPtr( helper.Handle, GWL.EXSTYLE );
            //Debug.WriteLine( $"style value before {dwExStyle.ToInt64()}" );
            if ( !NoActive )
            {
                dwExStyle = IntPtr.Add( dwExStyle, WS_EX_NOACTIVATE );
            }
            else
            {
                dwExStyle = IntPtr.Subtract( dwExStyle, WS_EX_NOACTIVATE );
            }
            //Debug.WriteLine( $"style value after {dwExStyle.ToInt64()}" );
            //Debug.Write( $"toggling value:NoActive, was {NoActive}, " );
            NoActive = !NoActive;
            //Debug.WriteLine( $"now {NoActive}" );
            SetWindowLongPtr( helper.Handle, GWL.EXSTYLE, dwExStyle );

        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged( string propertyName )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
        }

        #endregion
    }
}
