using Atlassian;
using groupkt;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

#if (!SILVERLIGHT)
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows.Controls;
using System.Windows;
#endif


#if (SILVERLIGHT)
namespace CF.RESTClientDotNet.Silverlight.Sample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : UserControl
#else
namespace CF.RESTClientDotNet.UWP.Sample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
#endif
    {
        #region Fields
        private RESTClient _BitbucketClient;
        #endregion

        #region Constructror
        public MainPage()
        {
            InitializeComponent();
        }
        #endregion

        #region Event Handlers

        private async void CountryCodeGrid_Loaded(object sender, RoutedEventArgs e)
        {
            ToggleBusy(true);
            var countryCodeClient = new RESTClient(new NewtonsoftSerializationAdapter(), new Uri("http://services.groupkt.com/country/get/all"));

            var countryData = await countryCodeClient.GetAsync<groupktResult<CountriesResult>>();
            CountryCodeList.ItemsSource = countryData.RestResponse.result;
            ToggleBusy(false);
        }

        private async void GetRepos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ToggleBusy(true);

                //Ensure the client is ready to go
                GetBitBucketClient();

                //Download the repository data
                var repos = (await _BitbucketClient.GetAsync<RepositoryList>());

                //Put it in the List Box
                ReposBox.ItemsSource = repos.values;
            }
            catch
            {

            }

            ToggleBusy(false);

        }

        private async void ChangeRepoDescription_Click(object sender, RoutedEventArgs e)
        {
            ToggleBusy(true);

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
                var retVal = await _BitbucketClient.PutAsync<object, Repository, string>(selectedRepo, selectedRepo.full_name.Split('/')[1]);
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

            ToggleBusy(false);

        }
        private async void CallLocalGet_Click(object sender, RoutedEventArgs e)
        {
            var restClient = new RESTClient(new NewtonsoftSerializationAdapter(), new Uri("http://localhost:49901/api/values"));
            var test = await restClient.GetAsync<List<string>>();
        }

        private static string GetDataFromResponseStream(HttpWebResponse response, bool readToEnd = false)
        {
            var responseStream = response.GetResponseStream();
            byte[] responseBuffer = null;

            if (!readToEnd)
            {
                if (responseStream.Length == -1)
                {
                    throw new Exception("An error occurred while getting data from the server. Please contact support");
                }

                //Read the stream in to a buffer
                responseBuffer = new byte[responseStream.Length];

                //Read from the stream (complete)
                var responseLength = responseStream.Read(responseBuffer, 0, (int)responseStream.Length);
            }
            else
            {
                var reader = new StreamReader(responseStream);
                return reader.ReadToEnd();
            }

            //Convert the response from bytes to json string
            var data = Encoding.UTF8.GetString(responseBuffer, 0, responseBuffer.Length);
            return data;
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

        #endregion

        #region Private Methods
        private void GetBitBucketClient()
        {

            if (_BitbucketClient != null)
            {
                return;
            }

#if (SILVERLIGHT)
            string url = "http://localhost:49901/api/values/" + UsernameBox.Text;
#else
            string url = "https://api.bitbucket.org/2.0/repositories/" + UsernameBox.Text;
#endif
            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(UsernameBox.Text + ":" + ThePasswordBox.Password));
            _BitbucketClient = new RESTClient(new NewtonsoftSerializationAdapter(), new Uri(url));
            _BitbucketClient.Headers.Add("Authorization", "Basic " + credentials);
            _BitbucketClient.ErrorType = typeof(ErrorModel);
        }

        private void ToggleBusy(bool isBusy)
        {
#if (WINDOWS_UWP)
            Ring2.IsActive = isBusy;
#else
            Ring2.IsIndeterminate = isBusy;
#endif

        }
#endregion
    }
}
