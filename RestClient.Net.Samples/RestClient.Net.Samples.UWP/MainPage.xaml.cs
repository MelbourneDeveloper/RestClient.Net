namespace RestClient.Net.Samples.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new RestClient.Net.Samples.App());
        }
    }
}
