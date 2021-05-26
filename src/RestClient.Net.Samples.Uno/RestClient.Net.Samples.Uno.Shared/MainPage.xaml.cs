using RestClient.Net.Abstractions.Extensions;
using RestClient.Net.Samples.Uno.Shared;
using RestClientNetSamples;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using client = RestClient.Net.Client;

#pragma warning disable Uno0001 // Uno type or member is not implemented


namespace RestClient.Net.Samples.Uno
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region Fields
        private client _BitbucketClient;
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

            _BitbucketClient = new client(
                new NewtonsoftSerializationAdapter(),
                baseUri: new Uri(url),
                createHttpClient: new UnoSampleHttpClientFactory().CreateClient);

            _BitbucketClient.SetJsonContentTypeHeader();

            if (!string.IsNullOrEmpty(password))
            {
                _BitbucketClient.SetBasicAuthenticationHeader(UsernameBox.Text, password);
            }
        }

        private async void OnGetReposClick()
        {
            ToggleBusy(true);

            try
            {
                ReposBox.ItemsSource = null;
                ReposBox.IsEnabled = false;

                //Ensure the client is ready to go
                GetBitBucketClient(GetPassword());

                //Download the repository data
                RepositoryList repos = await _BitbucketClient.GetAsync<RepositoryList>();

                //Put it in the List Box
                ReposBox.ItemsSource = repos.values;
                ReposBox.SelectedItem = repos.values.FirstOrDefault();
                ReposBox.IsEnabled = true;
            }
            catch
            {
                await HandleException($"An error occurred while attempting to get repos.");
            }

            ToggleBusy(false);
        }

        private async Task OnSavedClicked()
        {
            ToggleBusy(true);

            try
            {
                if (!(ReposBox.SelectedItem is Repository selectedRepo))
                {
                    return;
                }

                //Ensure the client is ready to go
                GetBitBucketClient(GetPassword());

                var requestUri = $"{UsernameBox.Text}/{selectedRepo.full_name.Split('/')[1]}";

                //Put the change
                var retVal = await _BitbucketClient.PutAsync<Repository, Repository>(selectedRepo, requestUri);

                await DisplayAlert("Saved", "Your repo was updated.");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.InnerException.Message);

                //await HandleException($"Save error. Please ensure you entered your credentials.\r\n{ex.Message}");
            }

            ToggleBusy(false);
        }

        private async Task HandleException(string message) => await DisplayAlert("Error", message);

        private string GetPassword() =>

#if __WASM__
            ThePasswordBox.Text;
#else
            ThePasswordBox.Password;
#endif


        private async Task DisplayAlert(string title, string message)
        {
            var messageDialog = new MessageDialog(message, title);
            await messageDialog.ShowAsync();
        }

        private void ToggleBusy(bool isBusy) => ReposActivityIndicator.Visibility = isBusy ? Visibility.Visible : Visibility.Collapsed;

        private void AttachEventHandlers()
        {
            SaveButton.Click += SaveButton_Clicked;
            GetReposButton.Click += GetReposButton_Clicked;
            ReposBox.SelectionChanged += ReposBox_ItemSelected;
        }
        #endregion

        #region Event Handlers
        private void ReposBox_ItemSelected(object sender, SelectionChangedEventArgs e) => ReposPage.DataContext = ReposBox.SelectedItem as Repository;


        private async void SaveButton_Clicked(object sender, RoutedEventArgs e) => await OnSavedClicked();

        private void GetReposButton_Clicked(object sender, RoutedEventArgs e) => OnGetReposClick();
        #endregion
    }
}

#pragma warning restore Uno0001 // Uno type or member is not implemented
