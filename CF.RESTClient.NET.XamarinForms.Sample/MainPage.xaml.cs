using Atlassian;
using CF.RESTClientDotNet;
using groupkt;
using System;
using System.Text;
using restclientdotnet = CF.RESTClientDotNet;
using System.Threading.Tasks;

#if (!SILVERLIGHT)
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
#endif

namespace CF.RESTClient.NET.Sample
{

#if (!SILVERLIGHT)
    [XamlCompilation(XamlCompilationOptions.Compile)]
#endif
    public partial class MainPage
    {
        #region Fields
        private restclientdotnet.RESTClient _BitbucketClient;
        #endregion

        #region Constructror
        public MainPage()
        {
            InitializeComponent();

#if (!SILVERLIGHT)
            SaveButton.Clicked += SaveButton_Clicked;
            CurrentPageChanged += MainPage_CurrentPageChanged;
            GetReposButton.Clicked += GetReposButton_Clicked;
            ReposBox.ItemSelected += ReposBox_ItemSelected;
#endif
        }

        private void GetReposButton_Clicked(object sender, EventArgs e)
        {
            GetReposClick();
        }

        #endregion

        #region Event Handlers

#if (!SILVERLIGHT)
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
                CountryCodesActivityIndicator.IsRunning = false;
                CountryCodesActivityIndicator.IsVisible = false;
            }
            catch (Exception ex)
            {

            }
        }
#else
        private void GetRepos_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            GetReposClick();
        }
#endif

        private async void GetReposClick()
        {
            try
            {
                ToggleReposBusy(true);

                //Ensure the client is ready to go
                GetBitBucketClient();

                //Download the repository data
                var repos = (await _BitbucketClient.GetAsync<RepositoryList>());

                //Put it in the List Box
                ReposBox.ItemsSource = repos.values;
            }
            catch (Exception ex)
            {
                await HandleException(ex);
            }

            ToggleReposBusy(false);
        }

#if (!SILVERLIGHT)
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

        private async Task HandleException(Exception ex)
        {
            ErrorModel errorModel = null;

            if (ex is RESTException rex)
            {
                errorModel = rex.Error as ErrorModel;
            }

            string message = "An error occurred while attempting to use a REST service.";

            if (errorModel != null)
            {
                message += "\r\n" + errorModel.error.message;
            }

            await DisplayAlert(message, "Error", "OK");
        }
#endif

        #endregion

        #region Private Methods
        private void GetBitBucketClient()
        {
#if (SILVERLIGHT)
            string url = "http://localhost:49902/api/BitBucketRepository/" + UsernameBox.Text + "-" + ThePasswordBox.Password;
#else
            var url = "https://api.bitbucket.org/2.0/repositories/" + UsernameBox.Text;
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(UsernameBox.Text + ":" + ThePasswordBox.Text));
#endif
            _BitbucketClient = new restclientdotnet.RESTClient(new NewtonsoftSerializationAdapter(), new Uri(url));
#if (!SILVERLIGHT)
            _BitbucketClient.Headers.Add("Authorization", "Basic " + credentials);
#endif
            _BitbucketClient.ErrorType = typeof(ErrorModel);
        }

        private void ToggleReposBusy(bool isBusy)
        {

#if (!SILVERLIGHT)
            ReposActivityIndicator.IsVisible = true;
            ReposActivityIndicator.IsRunning = isBusy;
#else
            ReposActivityIndicator.IsIndeterminate = isBusy;
#endif

        }
        #endregion
    }
}
