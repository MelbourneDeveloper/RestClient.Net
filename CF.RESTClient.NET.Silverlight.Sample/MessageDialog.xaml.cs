using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CF.RESTClientDotNet.Silverlight.Sample
{
    public partial class MessageDialog : ChildWindow
    {
        public MessageDialog(string message)
        {
            InitializeComponent();
            Content = new TextBlock { Text = message };
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        internal async Task ShowAsync()
        {
            await Task.Factory.StartNew(Show);
        }
    }
}

