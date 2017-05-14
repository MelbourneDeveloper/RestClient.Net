
using sampleApp = CF.RESTClient.NET.XamarinForms.Sample.App;

namespace CF.RESTClient.NET.Sample.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new sampleApp());
        }
    }
}
