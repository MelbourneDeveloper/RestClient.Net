using Atlassian;
using groupkt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
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
        }

        private async void CountryCodeGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var countryCodeClient = new RESTClient(new NewtonsoftSerializationAdapter(), new Uri("http://services.groupkt.com/country/get/all"));

            var countryData = await countryCodeClient.GetAsync<groupktResult<CountriesResult>>();
            CountryCodeList.ItemsSource = countryData.Data.RestResponse.result;
            Ring1.IsActive = false;
        }

        private async void GetRepos_Click(object sender, RoutedEventArgs e)
        {
            Ring2.IsActive = true;
            try
            {
                string url = "https://api.bitbucket.org/2.0/repositories/" + UsernameBox.Text;
                string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(UsernameBox.Text + ":" + ThePasswordBox.Password));

                var bitbucketClient = new RESTClient(new NewtonsoftSerializationAdapter(), new Uri(url));
                bitbucketClient.Headers.Add("Authorization", "Basic " + credentials);
                var repos = (await bitbucketClient.GetAsync<RepositoryList>()).Data;

                ReposBox.ItemsSource = repos.values;
            }
            catch
            {

            }
            Ring2.IsActive = false;

        }


        //private async void button_Click(object sender, RoutedEventArgs e)
        //{
        //    var restClient = new RESTClient(new XmlSerializationAdapter(), new Uri("http://www.thomas-bayer.com/sqlrest/CUSTOMER"));
        //    Strongly typed, primitive query string
        //    var theCustomer = await restClient.GetAsync<CUSTOMER, int>(5);

        //    More generic base Uri
        //    var restClient2 = new RESTClient(new XmlSerializationAdapter(), new Uri("http://www.thomas-bayer.com/sqlrest"));
        //    theCustomer = await restClient2.GetAsync<CUSTOMER, string>("CUSTOMER/5");
        //    var theProduct = await restClient2.GetAsync<PRODUCT, string>("PRODUCT/5");

        //}
    }
}
