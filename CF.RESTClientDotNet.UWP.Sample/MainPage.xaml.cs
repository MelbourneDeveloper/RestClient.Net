using Atlassian;
using groupkt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using ThomasBayer;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CF.RESTClientDotNet.UWP.Sample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            //var restClient = new RESTClient(new XmlSerializationAdapter(), new Uri("http://www.thomas-bayer.com/sqlrest/CUSTOMER"));
            //Strongly typed, primitive query string
            //var theCustomer = await restClient.GetAsync<CUSTOMER, int>(5);

            //More generic base Uri
            //var restClient2 = new RESTClient(new XmlSerializationAdapter(), new Uri("http://www.thomas-bayer.com/sqlrest"));
            //theCustomer = await restClient2.GetAsync<CUSTOMER, string>("CUSTOMER/5");
            //var theProduct = await restClient2.GetAsync<PRODUCT, string>("PRODUCT/5");

            //var restClient3 = new RESTClient(new NewtonsoftSerializationAdapter(), new Uri("http://services.groupkt.com/country/get/all"));

            //var countryData = await restClient3.GetAsync<groupktResult<CountriesResult>>();


            var restClient3 = new RESTClient(new NewtonsoftSerializationAdapter(), new Uri("https://api.bitbucket.org/2.0/repositories/1team/%7B21fa9bf8-b5b2-4891-97ed-d590bad0f871%7D"));

            var bla = await restClient3.GetAsync<RootObject>();

        }
    }
}
