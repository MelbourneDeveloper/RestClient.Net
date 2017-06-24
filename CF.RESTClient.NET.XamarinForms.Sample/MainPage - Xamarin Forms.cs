using Atlassian;
using CF.RESTClientDotNet;
using groupkt;
using System;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using restclientdotnet = CF.RESTClientDotNet;

namespace CF.RESTClient.NET.Sample
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage
    {
        #region Event Handlers
        private void MainPage_CurrentPageChanged(object sender, EventArgs e)
        {
            if (CurrentPage == CountryCodesPage)
            {
                CountryCodeGridLoaded();
            }
        }

        private async void CountryCodeGridLoaded()
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

            CountryCodesActivityIndicator.IsRunning = false;
            CountryCodesActivityIndicator.IsVisible = false;
        }

        private void ReposBox_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var selectedRepo = ReposBox.SelectedItem as Repository;
            ReposPage.BindingContext = selectedRepo;
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            ToggleReposBusy(true);

            try
            {
                var selectedRepo = ReposBox.SelectedItem as Repository;
                if (selectedRepo == null)
                {
                    return;
                }

                //Ensure the client is ready to go
                GetBitBucketClient();

                var repoSlug = selectedRepo.full_name.Split('/')[1];

                //Post the change
                var retVal = await _BitbucketClient.PutAsync<Repository, Repository, string>(selectedRepo, repoSlug);

                await DisplayAlert("Saved", "Your repo was updated.", "OK");
            }
            catch (Exception ex)
            {
                await HandleException(ex);
            }

            ToggleReposBusy(false);
        }

        #endregion

        #region Private Methods

        private async Task DisplayAlert(string message)
        {
            await DisplayAlert("Error", message, "OK");
        }

        private void GetBitBucketClient()
        {
            var url = "https://api.bitbucket.org/2.0/repositories/" + UsernameBox.Text;
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(UsernameBox.Text + ":" + ThePasswordBox.Text));
            _BitbucketClient = new restclientdotnet.RESTClient(new NewtonsoftSerializationAdapter(), new Uri(url));
            _BitbucketClient.ErrorType = typeof(ErrorModel);
        }

        private void ToggleReposBusy(bool isBusy)
        {
            ReposActivityIndicator.IsVisible = true;
            ReposActivityIndicator.IsRunning = isBusy;
        }

        private void AttachEventHandlers()
        {
            SaveButton.Clicked += SaveButton_Clicked;
            CurrentPageChanged += MainPage_CurrentPageChanged;
            GetReposButton.Clicked += GetReposButton_Clicked;
            ReposBox.ItemSelected += ReposBox_ItemSelected;
        }
        #endregion
    }
}
