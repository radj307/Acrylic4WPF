using WPFAcrylics;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for TestWindow1.xaml
    /// </summary>
    public partial class TestWindow1 : BasicAcrylWindow
    {
        public TestWindow1()
        {
            InitializeComponent();
            Closing += (s, e) =>
            {
                e.Cancel = true;
                Hide();
            };
        }

        private void CloseClick(object sender, System.Windows.RoutedEventArgs e) => Close();
    }
}
