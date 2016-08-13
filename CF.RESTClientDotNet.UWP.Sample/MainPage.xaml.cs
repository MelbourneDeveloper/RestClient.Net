using Atlassian;
using groupkt;
using System;
using System.IO;
using System.Net;
using System.Text;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CF.RESTClientDotNet.UWP.Sample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private RESTClient _BitbucketClient;

        public MainPage()
        {
            InitializeComponent();
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
                //Ensure the client is ready to go
                GetBitBucketClient();

                //Download the repository data
                var repos = (await _BitbucketClient.GetAsync<RepositoryList>()).Data;

                //Put it in the List Box
                ReposBox.ItemsSource = repos.values;
            }
            catch
            {

            }
            Ring2.IsActive = false;

        }

        private async void ChangeRepoDescription_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRepo = ReposBox.SelectedItem as Repository;
                if (selectedRepo == null)
                {
                    return;
                }

                //Ensure the client is ready to go
                GetBitBucketClient();

                selectedRepo.description = DescriptionBox.Text;

                //Post the change
                var retVal = await _BitbucketClient.PutAsync<object, Repository, string>(selectedRepo, selectedRepo.name);
            }
            catch (Exception ex)
            {
                ErrorModel errorModel = null;
                var rex = ex as RESTException;

                if (rex != null)
                {
                    errorModel = rex.Error as ErrorModel;
                }

                string message = "An error occurred trying to change the repository description.";

                if (errorModel != null)
                {
                    message += "\r\n" + errorModel.error.message;
                }

                var dialog = new MessageDialog(message);
                await dialog.ShowAsync();
            }

        }

        private void GetBitBucketClient()
        {

            if (_BitbucketClient != null)
            {
                return;
            }

            string url = "https://api.bitbucket.org/2.0/repositories/" + UsernameBox.Text;
            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(UsernameBox.Text + ":" + ThePasswordBox.Password));
            _BitbucketClient = new RESTClient(new NewtonsoftSerializationAdapter(), new Uri(url));
            _BitbucketClient.Headers.Add("Authorization", "Basic " + credentials);
            _BitbucketClient.ErrorType = typeof(ErrorModel);
        }

        private void ReposBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedRepo = ReposBox.SelectedItem as Repository;
            if (selectedRepo == null)
            {
                return;
            }
            DescriptionBox.Text = selectedRepo.description;
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
