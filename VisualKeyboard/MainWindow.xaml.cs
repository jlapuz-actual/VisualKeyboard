
namespace VisualKeyboard
{
    using System.Windows;
    using VisualKeyboard.ViewModel;
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            GridViewModel vm = new GridViewModel
            {
                Window = this
            };

            //vm.LoadDefaults();
            DataContext = vm;

            InitializeComponent();
        }
    }
}
