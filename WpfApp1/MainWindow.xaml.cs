using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WpfApp1
{
    public static class Statics
    {
        public static IEnumerable<WindowStyle> WindowStyles = Enum.GetValues<WindowStyle>().AsEnumerable();
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private acrylic TestWindow => (FindResource("TestWindow") as acrylic)!;

        private void HideClick(object? sender, RoutedEventArgs e) => TestWindow.Hide();
        private void ShowClick(object? sender, RoutedEventArgs e) => TestWindow.Show();
        private void CloseClick(object? sender, RoutedEventArgs e) => Close();
    }
}
