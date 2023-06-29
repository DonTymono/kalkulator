using System.Windows;

namespace kalkulator
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new Kalkulator();
        }
    }
}
