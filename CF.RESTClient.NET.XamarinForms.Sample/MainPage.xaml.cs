using Atlassian;
using CF.RESTClientDotNet;
using groupkt;
using System;
using System.Text;
using System.Threading.Tasks;
using restclientdotnet = CF.RESTClientDotNet;

namespace CF.RESTClient.NET.Sample
{
    public partial class MainPage
    {
        #region Fields
        private restclientdotnet.RESTClient _BitbucketClient;
        #endregion

        #region Constructror
        public MainPage()
        {
            InitializeComponent();
            AttachEventHandlers();
        }

        #endregion

        #region Private Methods

        private void GetBitBucketClient(string password)
        {
            var url = "https://api.bitbucket.org/2.0/repositories/" + UsernameBox.Text;
            _BitbucketClient = new restclientdotnet.RESTClient(new NewtonsoftSerializationAdapter(), new Uri(url));

            if (!string.IsNullOrEmpty(password))
            {
                var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(UsernameBox.Text + ":" + password));
                _BitbucketClient.Headers.Add("Authorization", "Basic " + credentials);
            }

            _BitbucketClient.ErrorType = typeof(ErrorModel);
        }

        private async void OnGetReposClick()
        {
            try
            {
                ToggleBusy(true);

                //Ensure the client is ready to go
                GetBitBucketClient(GetPassword());

                //Download the repository data
                var repos = (await _BitbucketClient.GetAsync<RepositoryList>());

                //Put it in the List Box
                ReposBox.ItemsSource = repos.values;
            }
            catch (Exception ex)
            {
                await HandleException(ex);
            }

            ToggleBusy(false);
        }

        private async Task OnSavedClicked()
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
                GetBitBucketClient(GetPassword());

                var repoSlug = selectedRepo.full_name.Split('/')[1];

                //Post the change
                var retVal = await _BitbucketClient.PutAsync<Repository, Repository, string>(selectedRepo, repoSlug);

                await DisplayAlert("Saved", "Your repo was updated.");
            }
            catch (Exception ex)
            {
                await HandleException(ex);
            }

            ToggleBusy(false);
        }

        private async void OnCountryCodeGridLoaded()
        {
            try
            {
                var countryCodeClient = new restclientdotnet.RESTClient(new NewtonsoftSerializationAdapter(), new Uri("http://services.groupkt.com/country/get/all"));
                var countryData = await countryCodeClient.GetAsync<groupktResult<CountriesResult>>();
                CountryCodeList.ItemsSource = countryData.RestResponse.result;
            }
            catch (Exception ex)
            {
                await HandleException(ex);
            }

            ToggleBusy(false);
        }

        private async Task HandleException(Exception ex)
        {
            ErrorModel errorModel = null;

            var rex = ex as RESTException;
            if (rex != null)
            {
                errorModel = rex.Error as ErrorModel;
            }

            string message = $"An error occurred while attempting to use a REST service.\r\nError: {ex.Message}\r\nInner Error: {ex.InnerException?.Message}\r\nInner Inner Error: {ex.InnerException?.InnerException?.Message}";

            if (errorModel != null)
            {
                message += $"\r\n{errorModel.error.message}";
            }

            await DisplayAlert("Error", message);
        }

        #endregion
    }
}
