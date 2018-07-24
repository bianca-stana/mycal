using System.Diagnostics;
using System.Windows.Navigation;

namespace MyCal
{
    /// <summary>
    ///     Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow
    {
        private readonly ViewModel _model = new ViewModel();

        public AboutWindow()
        {
            InitializeComponent();
            DataContext = _model;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}