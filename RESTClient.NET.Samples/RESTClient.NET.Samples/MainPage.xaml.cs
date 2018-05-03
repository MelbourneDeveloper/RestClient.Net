using Atlassian;
using CF.RESTClientDotNet;
using groupkt;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using ThomasBayer;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using restclientdotnet = CF.RESTClientDotNet;

namespace CF.RESTClient.NET.Sample
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
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

        private async Task<CUSTOMER> GetCustomer(int customerId)
        {
            CUSTOMER retVal = null;

            ToggleBusy(true);
            try
            {
                var countryCodeClient = new restclientdotnet.RESTClient(new XmlSerializationAdapter(), new Uri("http://www.thomas-bayer.com/sqlrest/CUSTOMER/" + customerId));
                retVal = await countryCodeClient.GetAsync<CUSTOMER>();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            ToggleBusy(false);
            return retVal;
        }

        private void GetBitBucketClient(string password, bool isGet)
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
            ToggleBusy(true);

            try
            {
                ReposBox.ItemsSource = null;
                ReposBox.IsEnabled = false;

                //Ensure the client is ready to go
                GetBitBucketClient(GetPassword(), true);

                var tracer = new BasicTracer();
                _BitbucketClient.Tracer = tracer;

                //Download the repository data
                var repos = (await _BitbucketClient.GetAsync<RepositoryList>());

                //Put it in the List Box
                ReposBox.ItemsSource = repos.values;
                ReposBox.IsEnabled = true;

                foreach (var traceInfo in tracer.TraceInfos)
                {
                    Debug.WriteLine($"Operation: {traceInfo.OperationName} Milliseconds: {(traceInfo.CompleteTime - traceInfo.StartTime).TotalMilliseconds}");
                }
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
                GetBitBucketClient(GetPassword(), false);

                //var repoSlug = selectedRepo.full_name.Split('/')[1];
                var requestUri = $"https://api.bitbucket.org/2.0/repositories/{UsernameBox.Text}/{selectedRepo.full_name.Split('/')[1]}";

                //Post the change
                var retVal = await _BitbucketClient.PutAsync<Repository, Repository>(selectedRepo, requestUri);

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
            ToggleBusy(true);

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

        private string GetPassword()
        {
            return ThePasswordBox.Text;
        }

        private async Task DisplayAlert(string title, string message)
        {
            await DisplayAlert(title, message, "OK");
        }

        private void ToggleBusy(bool isBusy)
        {
            ReposActivityIndicator.IsVisible = isBusy;
            CountryCodesActivityIndicator.IsVisible = isBusy;
            ReposActivityIndicator.IsRunning = isBusy;
            CountryCodesActivityIndicator.IsRunning = isBusy;
        }

        private void AttachEventHandlers()
        {
            SaveButton.Clicked += SaveButton_Clicked;
            CurrentPageChanged += MainPage_CurrentPageChanged;
            GetReposButton.Clicked += GetReposButton_Clicked;
            ReposBox.ItemSelected += ReposBox_ItemSelected;
        }

        #endregion

        #region Event Handlers
        private void MainPage_CurrentPageChanged(object sender, EventArgs e)
        {
            if (CurrentPage == CountryCodesPage)
            {
                OnCountryCodeGridLoaded();
            }
        }

        private void ReposBox_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ReposPage.BindingContext = ReposBox.SelectedItem as Repository;
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            await OnSavedClicked();
        }

        private void GetReposButton_Clicked(object sender, EventArgs e)
        {
            OnGetReposClick();
        }

        #endregion
    }
}
