using System.Windows;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WPFAcrylics.BasicAcrylWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void CloseClick(object? sender, RoutedEventArgs e) => Close();

        private TestWindow1 Basic => (FindResource("BasicAcrylWindow") as TestWindow1)!;
        private void ShowBasic(object? sender, RoutedEventArgs e) => Basic.Show();
        private void HideBasic(object? sender, RoutedEventArgs e) => Basic.Hide();

        private TestWindow2 Normal => (FindResource("AcrylWindow") as TestWindow2)!;
        private void ShowNormal(object? sender, RoutedEventArgs e) => Normal.Show();
        private void HideNormal(object? sender, RoutedEventArgs e) => Normal.Hide();
    }
}
