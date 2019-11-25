
namespace VisualKeyboard
{
    using System.Windows;
    using VisualKeyboard.Models;
    using VisualKeyboard.ViewModel;
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //vm.LoadDefaults();
            DataContext = new GridViewModel
            {
                Window = this
            };

            InitializeComponent();
        }
    }
}
