using WPFAcrylics;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for TestWindow2.xaml
    /// </summary>
    public partial class TestWindow2 : AcrylWindow
    {
        public TestWindow2()
        {
            InitializeComponent();
            Closing += (s, e) =>
            {
                e.Cancel = true;
                Hide();
            };
        }
    }
}
